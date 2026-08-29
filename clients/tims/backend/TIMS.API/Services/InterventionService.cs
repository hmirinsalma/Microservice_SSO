using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.Intervention;
using TIMS.API.Entities;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

public class InterventionService : IInterventionService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly INotificationService _notifService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<InterventionService> _logger;

    public InterventionService(ApplicationDbContext db, IMapper mapper,
        INotificationService notifService, IWebHostEnvironment env,
        ILogger<InterventionService> logger)
    {
        _db = db; _mapper = mapper; _notifService = notifService;
        _env = env; _logger = logger;
    }

    private IQueryable<Intervention> BaseQuery() =>
        _db.Interventions
            .Where(i => !i.IsDeleted)
            .Include(i => i.Responsable)
            .Include(i => i.ChefService)
            .Include(i => i.Technicien)
            .Include(i => i.Equipe)
            .Include(i => i.Service)
            .Include(i => i.CreatedBy)
            .Include(i => i.Comments).ThenInclude(c => c.Author)
            .Include(i => i.Attachments).ThenInclude(a => a.UploadedBy);

    public async Task<PagedResult<InterventionListDto>> GetAllAsync(
        InterventionFilterDto filter, int userId, string role, int? serviceId)
    {
        var query = _db.Interventions.Where(i => !i.IsDeleted)
            .Include(i => i.Responsable).Include(i => i.Technicien)
            .Include(i => i.Equipe).Include(i => i.Service).AsQueryable();

        // RBAC scope
        if (role == RoleNames.ChefService && serviceId.HasValue)
            query = query.Where(i => i.ServiceId == serviceId.Value);
        else if (role == RoleNames.Technicien)
            query = query.Where(i => i.TechnicienId == userId);

        // Filters
        if (!string.IsNullOrEmpty(filter.NumeroIntervention))
            query = query.Where(i => i.NumeroIntervention.Contains(filter.NumeroIntervention));
        if (!string.IsNullOrEmpty(filter.Objet))
            query = query.Where(i => i.Objet.Contains(filter.Objet));
        if (filter.TechnicienId.HasValue)
            query = query.Where(i => i.TechnicienId == filter.TechnicienId);
        if (filter.ResponsableId.HasValue)
            query = query.Where(i => i.ResponsableId == filter.ResponsableId);
        if (filter.EquipeId.HasValue)
            query = query.Where(i => i.EquipeId == filter.EquipeId);
        if (filter.Priority.HasValue)
            query = query.Where(i => i.Priority == filter.Priority);
        if (filter.Status.HasValue)
            query = query.Where(i => i.Status == filter.Status);
        if (filter.ServiceId.HasValue)
            query = query.Where(i => i.ServiceId == filter.ServiceId);
        if (filter.DateDebut.HasValue)
            query = query.Where(i => i.CreatedAt >= filter.DateDebut.Value);
        if (filter.DateFin.HasValue)
            query = query.Where(i => i.CreatedAt <= filter.DateFin.Value);

        // Sorting
        query = (filter.SortBy?.ToLower(), filter.SortOrder?.ToLower()) switch
        {
            ("numentervention", "asc") => query.OrderBy(i => i.NumeroIntervention),
            ("numentervention", _) => query.OrderByDescending(i => i.NumeroIntervention),
            ("dateprevue", "asc") => query.OrderBy(i => i.DatePrevue),
            ("dateprevue", _) => query.OrderByDescending(i => i.DatePrevue),
            ("priority", "asc") => query.OrderBy(i => i.Priority),
            ("priority", _) => query.OrderByDescending(i => i.Priority),
            ("status", "asc") => query.OrderBy(i => i.Status),
            ("status", _) => query.OrderByDescending(i => i.Status),
            (_, "asc") => query.OrderBy(i => i.CreatedAt),
            _ => query.OrderByDescending(i => i.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return new PagedResult<InterventionListDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = total, Page = filter.Page, PageSize = filter.PageSize
        };
    }

    public async Task<InterventionDto> GetByIdAsync(int id, int userId, string role, int? serviceId)
    {
        var i = await BaseQuery().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted)
            ?? throw new NotFoundException("Intervention introuvable");
        CheckScope(i, userId, role, serviceId);
        return MapToDto(i);
    }

    public async Task<InterventionDto> CreateAsync(CreateInterventionDto dto, int userId, string role, int? serviceId)
    {
        // Chef de service can only create for his service
        var svcId = role == RoleNames.ChefService ? serviceId : dto.ServiceId;

        var numero = await GenerateNumeroAsync();
        var intervention = new Intervention
        {
            NumeroIntervention = numero,
            Objet = dto.Objet, Description = dto.Description,
            TypeIntervention = dto.TypeIntervention, Categorie = dto.Categorie,
            Localisation = dto.Localisation, Equipement = dto.Equipement,
            DatePrevue = dto.DatePrevue, Priority = dto.Priority,
            Status = InterventionStatus.Nouvelle,
            ServiceId = svcId, EquipeId = dto.EquipeId,
            ResponsableId = dto.ResponsableId, TechnicienId = dto.TechnicienId,
            ChefServiceId = userId, CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Interventions.Add(intervention);
        await _db.SaveChangesAsync();

        await AddHistoryAsync(intervention.Id, userId, HistoryActionType.Creation,
            description: $"Intervention créée : {numero}");

        await _notifService.NotifyInterventionEventAsync(
            await BaseQuery().FirstAsync(x => x.Id == intervention.Id),
            NotificationType.InterventionCreee, $"Nouvelle intervention {numero}");

        return await GetByIdAsync(intervention.Id, userId, RoleNames.AdminTechnique, null);
    }

    public async Task<InterventionDto> UpdateAsync(int id, UpdateInterventionDto dto, int userId, string role, int? serviceId)
    {
        var i = await _db.Interventions.FindAsync(id)
            ?? throw new NotFoundException("Intervention introuvable");
        CheckScope(i, userId, role, serviceId);

        if (i.Status is InterventionStatus.Terminee or InterventionStatus.Annulee)
            throw new ConflictException("Intervention clôturée, modification impossible", "INTERVENTION_CLOSED");

        var changes = new List<string>();
        if (dto.Objet != null && dto.Objet != i.Objet) { AddChange(changes, "Objet", i.Objet, dto.Objet); i.Objet = dto.Objet; }
        if (dto.Description != null && dto.Description != i.Description) { AddChange(changes, "Description", i.Description, dto.Description); i.Description = dto.Description; }
        if (dto.TypeIntervention != null) { AddChange(changes, "Type", i.TypeIntervention, dto.TypeIntervention); i.TypeIntervention = dto.TypeIntervention; }
        if (dto.Categorie != null) { AddChange(changes, "Catégorie", i.Categorie, dto.Categorie); i.Categorie = dto.Categorie; }
        if (dto.Localisation != null) { AddChange(changes, "Localisation", i.Localisation, dto.Localisation); i.Localisation = dto.Localisation; }
        if (dto.Equipement != null) { AddChange(changes, "Équipement", i.Equipement, dto.Equipement); i.Equipement = dto.Equipement; }
        if (dto.DatePrevue.HasValue) { AddChange(changes, "DatePrevue", i.DatePrevue.ToString(), dto.DatePrevue.Value.ToString()); i.DatePrevue = dto.DatePrevue.Value; }

        if (dto.Priority.HasValue && dto.Priority != i.Priority)
        {
            await AddHistoryAsync(id, userId, HistoryActionType.ChangementPriorite,
                "Priority", i.Priority.ToString(), dto.Priority.Value.ToString());
            i.Priority = dto.Priority.Value;
        }
        if (dto.TechnicienId != i.TechnicienId)
        {
            await AddHistoryAsync(id, userId, HistoryActionType.ChangementTechnicien,
                "TechnicienId", i.TechnicienId?.ToString(), dto.TechnicienId?.ToString());
            i.TechnicienId = dto.TechnicienId;
        }
        if (dto.ResponsableId.HasValue && dto.ResponsableId != i.ResponsableId)
        {
            await AddHistoryAsync(id, userId, HistoryActionType.ChangementResponsable,
                "ResponsableId", i.ResponsableId?.ToString(), dto.ResponsableId.Value.ToString());
            i.ResponsableId = dto.ResponsableId;
        }

        i.UpdatedAt = DateTime.UtcNow;
        if (changes.Any())
            await AddHistoryAsync(id, userId, HistoryActionType.Modification, description: string.Join("; ", changes));

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, userId, RoleNames.AdminTechnique, null);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        i.IsDeleted = true; i.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<InterventionDto> ChangeStatusAsync(int id, ChangeStatusDto dto, int userId, string role)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");

        var allowed = GetAllowedTransitions(i.Status);
        if (!allowed.Contains(dto.NewStatus))
            throw new ConflictException(
                $"Transition invalide de {i.Status} vers {dto.NewStatus}. Autorisées: {string.Join(", ", allowed)}",
                "INVALID_STATUS_TRANSITION");

        // Technicien can only set EnCours/Suspendue/Terminee on his own interventions
        if (role == RoleNames.Technicien && i.TechnicienId != userId)
            throw new ForbiddenException();

        var old = i.Status;
        i.Status = dto.NewStatus;
        if (dto.NewStatus == InterventionStatus.Terminee)
            i.DateCloture = DateTime.UtcNow;
        i.UpdatedAt = DateTime.UtcNow;

        await AddHistoryAsync(id, userId, HistoryActionType.ChangementStatut,
            "Status", old.ToString(), dto.NewStatus.ToString());
        await _db.SaveChangesAsync();

        var full = await BaseQuery().FirstAsync(x => x.Id == id);
        await _notifService.NotifyInterventionEventAsync(full, NotificationType.ChangementStatut,
            $"Statut changé : {old} → {dto.NewStatus}");

        if (dto.NewStatus == InterventionStatus.Terminee)
            await _notifService.NotifyInterventionEventAsync(full, NotificationType.InterventionTerminee,
                $"Intervention {full.NumeroIntervention} terminée");

        return MapToDto(full);
    }

    public async Task<InterventionDto> ChangePriorityAsync(int id, ChangePriorityDto dto, int userId, string role)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        var old = i.Priority;
        i.Priority = dto.NewPriority;
        i.UpdatedAt = DateTime.UtcNow;
        await AddHistoryAsync(id, userId, HistoryActionType.ChangementPriorite,
            "Priority", old.ToString(), dto.NewPriority.ToString());
        await _db.SaveChangesAsync();

        var full = await BaseQuery().FirstAsync(x => x.Id == id);
        await _notifService.NotifyInterventionEventAsync(full, NotificationType.ChangementPriorite,
            $"Priorité changée : {old} → {dto.NewPriority}");
        return MapToDto(full);
    }

    public async Task<InterventionDto> AssignTechnicienAsync(int id, AssignTechnicienDto dto, int userId, string role, int? serviceId)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        CheckScope(i, userId, role, serviceId);

        var old = i.TechnicienId;
        i.TechnicienId = dto.TechnicienId;
        i.UpdatedAt = DateTime.UtcNow;

        var actionType = dto.TechnicienId == null ? HistoryActionType.RetraitAffectation :
                         old == null ? HistoryActionType.Affectation : HistoryActionType.ChangementTechnicien;

        await AddHistoryAsync(id, userId, actionType, "TechnicienId",
            old?.ToString(), dto.TechnicienId?.ToString());
        await _db.SaveChangesAsync();

        var full = await BaseQuery().FirstAsync(x => x.Id == id);
        var notifType = old == null ? NotificationType.TechnicienAffecte : NotificationType.ChangementTechnicien;
        await _notifService.NotifyInterventionEventAsync(full, notifType, "Affectation mise à jour");
        return MapToDto(full);
    }

    public async Task<CommentDto> AddCommentAsync(int id, AddCommentDto dto, int userId, string role)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        if (i.Status == InterventionStatus.Annulee)
            throw new ConflictException("Impossible d'ajouter un commentaire à une intervention annulée", "INTERVENTION_CANCELLED");
        if (role == RoleNames.Technicien && i.TechnicienId != userId)
            throw new ForbiddenException();

        var comment = new Comment { Content = dto.Content, InterventionId = id, AuthorId = userId };
        _db.Comments.Add(comment);
        await AddHistoryAsync(id, userId, HistoryActionType.AjoutCommentaire, description: dto.Content[..Math.Min(100, dto.Content.Length)]);
        await _db.SaveChangesAsync();

        var author = await _db.Users.FindAsync(userId);
        return new CommentDto
        {
            Id = comment.Id, Content = comment.Content, CreatedAt = comment.CreatedAt,
            Author = author == null ? null : new UserRefDto { Id = author.Id, FullName = $"{author.FirstName} {author.LastName}", ProfilePhotoPath = author.ProfilePhotoPath }
        };
    }

    public async Task<InterventionDto> UpdateCompteRenduAsync(int id, UpdateCompteRenduDto dto, int userId)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        if (i.TechnicienId != userId) throw new ForbiddenException();
        i.CompteRendu = dto.CompteRendu; i.UpdatedAt = DateTime.UtcNow;
        await AddHistoryAsync(id, userId, HistoryActionType.AjoutCompteRendu, description: "Compte rendu mis à jour");
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, userId, RoleNames.AdminTechnique, null);
    }

    public async Task<List<HistoryDto>> GetHistoryAsync(int id, int userId, string role, int? serviceId)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        CheckScope(i, userId, role, serviceId);
        var history = await _db.InterventionHistories
            .Include(h => h.Author)
            .Where(h => h.InterventionId == id)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
        return history.Select(h => new HistoryDto
        {
            Id = h.Id, ActionType = h.ActionType.ToString(), FieldChanged = h.FieldChanged,
            OldValue = h.OldValue, NewValue = h.NewValue, Description = h.Description,
            CreatedAt = h.CreatedAt,
            Author = h.Author == null ? null : new UserRefDto { Id = h.Author.Id, FullName = $"{h.Author.FirstName} {h.Author.LastName}" }
        }).ToList();
    }

    public async Task<AttachmentDto> AddAttachmentAsync(int id, IFormFile file, int userId, string role)
    {
        var i = await _db.Interventions.FindAsync(id) ?? throw new NotFoundException("Intervention introuvable");
        if (role == RoleNames.Technicien && i.TechnicienId != userId) throw new ForbiddenException();

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
        if (!allowed.Contains(file.ContentType))
            throw new AppException("Type de fichier non autorisé", 415, "INVALID_FILE_TYPE");
        if (file.Length > 10 * 1024 * 1024)
            throw new AppException("Fichier trop volumineux (max 10 Mo)", 413, "FILE_TOO_LARGE");

        var uploadDir = Path.Combine(_env.ContentRootPath, "Uploads", "attachments");
        Directory.CreateDirectory(uploadDir);
        var stored = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(uploadDir, stored);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var attachment = new Attachment
        {
            OriginalFileName = file.FileName, StoredFileName = stored,
            ContentType = file.ContentType, FileSize = file.Length,
            InterventionId = id, UploadedById = userId
        };
        _db.Attachments.Add(attachment);
        await AddHistoryAsync(id, userId, HistoryActionType.AjoutPieceJointe, description: file.FileName);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);
        return new AttachmentDto
        {
            Id = attachment.Id, OriginalFileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType, FileSize = attachment.FileSize,
            CreatedAt = attachment.CreatedAt,
            UploadedBy = user == null ? null : new UserRefDto { Id = user.Id, FullName = $"{user.FirstName} {user.LastName}" }
        };
    }

    public async Task DeleteAttachmentAsync(int attachmentId, int userId)
    {
        var a = await _db.Attachments.FindAsync(attachmentId) ?? throw new NotFoundException("Pièce jointe introuvable");
        a.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    public async Task<string> GetAttachmentUrlAsync(int attachmentId, int userId, string role, int? serviceId)
    {
        var a = await _db.Attachments.Include(x => x.Intervention)
            .FirstOrDefaultAsync(x => x.Id == attachmentId && !x.IsDeleted)
            ?? throw new NotFoundException("Pièce jointe introuvable");
        CheckScope(a.Intervention, userId, role, serviceId);
        // Return a relative URL; in production, generate a signed URL
        return $"/api/attachments/{attachmentId}/download";
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static List<InterventionStatus> GetAllowedTransitions(InterventionStatus current) =>
        current switch
        {
            InterventionStatus.Nouvelle  => [InterventionStatus.EnCours, InterventionStatus.Annulee],
            InterventionStatus.EnCours   => [InterventionStatus.Suspendue, InterventionStatus.Terminee, InterventionStatus.Annulee],
            InterventionStatus.Suspendue => [InterventionStatus.EnCours, InterventionStatus.Annulee],
            _                            => []
        };

    private static void CheckScope(Intervention i, int userId, string role, int? serviceId)
    {
        if (role == RoleNames.ChefService && i.ServiceId != serviceId)
            throw new ForbiddenException("Accès limité à votre service");
        if (role == RoleNames.Technicien && i.TechnicienId != userId)
            throw new ForbiddenException("Accès limité à vos interventions");
    }

    private async Task AddHistoryAsync(int interventionId, int authorId, HistoryActionType action,
        string? field = null, string? oldVal = null, string? newVal = null, string? description = null)
    {
        _db.InterventionHistories.Add(new InterventionHistory
        {
            InterventionId = interventionId, AuthorId = authorId,
            ActionType = action, FieldChanged = field,
            OldValue = oldVal, NewValue = newVal, Description = description
        });
        await _db.SaveChangesAsync();
    }

    private async Task<string> GenerateNumeroAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _db.Interventions
            .CountAsync(i => i.NumeroIntervention.StartsWith($"INT-{today}"));
        return $"INT-{today}-{(count + 1):D4}";
    }

    private static void AddChange(List<string> changes, string field, string? oldV, string? newV)
        => changes.Add($"{field}: «{oldV}» → «{newV}»");

    private static InterventionListDto MapToListDto(Intervention i) => new()
    {
        Id = i.Id, NumeroIntervention = i.NumeroIntervention, Objet = i.Objet,
        TypeIntervention = i.TypeIntervention, CreatedAt = i.CreatedAt,
        DatePrevue = i.DatePrevue, DateCloture = i.DateCloture,
        Priority = i.Priority, PriorityLabel = i.Priority.ToString(),
        Status = i.Status, StatusLabel = i.Status.ToString(),
        Responsable = UserRef(i.Responsable), Technicien = UserRef(i.Technicien),
        Equipe = i.Equipe == null ? null : new RefDto { Id = i.Equipe.Id, Name = i.Equipe.Name },
        Service = i.Service == null ? null : new RefDto { Id = i.Service.Id, Name = i.Service.Name }
    };

    private static InterventionDto MapToDto(Intervention i) => new()
    {
        Id = i.Id, NumeroIntervention = i.NumeroIntervention, Objet = i.Objet,
        Description = i.Description, TypeIntervention = i.TypeIntervention,
        Categorie = i.Categorie, Localisation = i.Localisation, Equipement = i.Equipement,
        CreatedAt = i.CreatedAt, DatePrevue = i.DatePrevue, DateCloture = i.DateCloture,
        Priority = i.Priority, PriorityLabel = i.Priority.ToString(),
        Status = i.Status, StatusLabel = i.Status.ToString(),
        CompteRendu = i.CompteRendu,
        Responsable = UserRef(i.Responsable), ChefService = UserRef(i.ChefService),
        Technicien = UserRef(i.Technicien), CreatedBy = UserRef(i.CreatedBy),
        Equipe = i.Equipe == null ? null : new RefDto { Id = i.Equipe.Id, Name = i.Equipe.Name },
        Service = i.Service == null ? null : new RefDto { Id = i.Service.Id, Name = i.Service.Name },
        Comments = i.Comments.Select(c => new CommentDto
        {
            Id = c.Id, Content = c.Content, CreatedAt = c.CreatedAt, Author = UserRef(c.Author)
        }).ToList(),
        Attachments = i.Attachments.Where(a => !a.IsDeleted).Select(a => new AttachmentDto
        {
            Id = a.Id, OriginalFileName = a.OriginalFileName, ContentType = a.ContentType,
            FileSize = a.FileSize, CreatedAt = a.CreatedAt, UploadedBy = UserRef(a.UploadedBy)
        }).ToList()
    };

    private static UserRefDto? UserRef(Entities.User? u) => u == null ? null : new UserRefDto
        { Id = u.Id, FullName = $"{u.FirstName} {u.LastName}", ProfilePhotoPath = u.ProfilePhotoPath };
}
