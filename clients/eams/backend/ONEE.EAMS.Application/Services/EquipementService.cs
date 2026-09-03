using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Equipement;
using ONEE.EAMS.Application.DTOs.Historique;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;
using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.Services;

public class EquipementService : IEquipementService
{
    private readonly IAppDbContext _db;
    private readonly IReferenceGeneratorService _refGen;
    private readonly IFileStorageService _fileStorage;
    private readonly INotificationService _notifService;

    public EquipementService(IAppDbContext db, IReferenceGeneratorService refGen,
        IFileStorageService fileStorage, INotificationService notifService)
    {
        _db = db; _refGen = refGen; _fileStorage = fileStorage; _notifService = notifService;
    }

    public async Task<PagedResult<EquipementListDto>> GetAllAsync(EquipementFilterRequest filter, ClaimsPrincipal user)
    {
        var role      = user.GetRole();
        var userId    = user.GetUserId();
        var serviceId = user.GetServiceId();

        var query = _db.Equipements.AsNoTracking().AsQueryable();

        // Périmètre RBAC
        query = role switch
        {
            UserRole.Chef_de_Service when serviceId.HasValue
                => query.Where(e => e.ServiceId == serviceId),
            UserRole.Technicien
                => query.Where(e => e.Techniciens.Any(t => t.TechnicienId == userId)),
            _ => query
        };

        // Filtres
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(e =>
                e.Nom.ToLower().Contains(s) ||
                e.Reference.ToLower().Contains(s) ||
                e.NumeroSerie.ToLower().Contains(s) ||
                e.Marque.ToLower().Contains(s) ||
                e.Modele.ToLower().Contains(s));
        }
        if (filter.CategorieId.HasValue)    query = query.Where(e => e.CategorieId == filter.CategorieId);
        if (!string.IsNullOrWhiteSpace(filter.Type)) query = query.Where(e => e.Type == filter.Type);
        if (filter.Etat.HasValue)           query = query.Where(e => e.Etat == filter.Etat);
        if (filter.ServiceId.HasValue && (role == UserRole.Admin_Patrimoine || role == UserRole.Directeur))
            query = query.Where(e => e.ServiceId == filter.ServiceId);
        if (filter.ResponsableId.HasValue)  query = query.Where(e => e.ResponsableId == filter.ResponsableId);
        if (filter.TechnicienId.HasValue)   query = query.Where(e => e.Techniciens.Any(t => t.TechnicienId == filter.TechnicienId));
        if (!string.IsNullOrWhiteSpace(filter.Localisation)) query = query.Where(e => e.Localisation.Contains(filter.Localisation));
        if (filter.DateInstallationFrom.HasValue) query = query.Where(e => e.DateInstallation >= filter.DateInstallationFrom);
        if (filter.DateInstallationTo.HasValue)   query = query.Where(e => e.DateInstallation <= filter.DateInstallationTo);

        // Tri
        query = filter.SortBy?.ToLower() switch
        {
            "reference"        => filter.SortDesc ? query.OrderByDescending(e => e.Reference)        : query.OrderBy(e => e.Reference),
            "dateinstallation" => filter.SortDesc ? query.OrderByDescending(e => e.DateInstallation) : query.OrderBy(e => e.DateInstallation),
            "etat"             => filter.SortDesc ? query.OrderByDescending(e => e.Etat)             : query.OrderBy(e => e.Etat),
            _                  => filter.SortDesc ? query.OrderByDescending(e => e.Nom)              : query.OrderBy(e => e.Nom)
        };

        var total = await query.CountAsync();

        // Projection SQL directe — JOIN sans chargement des entités complètes
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(e => new EquipementListDto(
                e.Id, e.Reference, e.Nom,
                e.Categorie.Nom, e.Categorie.Code, e.Categorie.Couleur,
                e.Type, e.Marque, e.Modele, e.NumeroSerie, e.Localisation,
                e.Service.Nom, e.Responsable.Nom + " " + e.Responsable.Prenom,
                e.DateInstallation, e.Etat, e.DateFinGarantie, e.ValeurAcquisition))
            .ToListAsync();

        return new PagedResult<EquipementListDto>
        {
            Items = items, TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)filter.PageSize),
            Page = filter.Page, PageSize = filter.PageSize
        };
    }

    public async Task<EquipementDetailDto> GetByIdAsync(Guid id, ClaimsPrincipal user)
    {
        var eq = await LoadEquipementOrThrow(id);
        CheckScope(eq, user);
        return MapDetail(eq);
    }

    public async Task<EquipementDetailDto> CreateAsync(CreateEquipementRequest req, ClaimsPrincipal user)
    {
        if (await _db.Equipements.AnyAsync(e => e.NumeroSerie == req.NumeroSerie))
            throw new ConflictException($"Numéro de série '{req.NumeroSerie}' déjà utilisé.");

        var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == req.CategorieId)
            ?? throw new NotFoundException("Catégorie introuvable.");

        var reference = await _refGen.GenerateAsync(cat.Code);
        var userId    = user.GetUserId();

        var eq = new Equipement
        {
            Id = Guid.NewGuid(), Reference = reference, Nom = req.Nom,
            CategorieId = req.CategorieId, Type = req.Type, Marque = req.Marque,
            Modele = req.Modele, NumeroSerie = req.NumeroSerie, Localisation = req.Localisation,
            ServiceId = req.ServiceId, ResponsableId = req.ResponsableId,
            DateInstallation = req.DateInstallation, DateMiseEnService = req.DateMiseEnService,
            Etat = req.Etat, DateFinGarantie = req.DateFinGarantie,
            ValeurAcquisition = req.ValeurAcquisition, Fournisseur = req.Fournisseur,
            Description = req.Description, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _db.Equipements.Add(eq);
        AddHistorique(eq.Id, "Equipement", "Creation", null,
            JsonSerializer.Serialize(new { eq.Nom, eq.Reference }), userId);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(eq.Id, user);
    }

    public async Task<EquipementDetailDto> UpdateAsync(Guid id, UpdateEquipementRequest req, ClaimsPrincipal user)
    {
        var eq     = await LoadEquipementOrThrow(id);
        var userId = user.GetUserId();

        if (eq.Etat != req.Etat)
        {
            AddHistorique(id, "Equipement", "ChangementEtat", eq.Etat.ToString(), req.Etat.ToString(), userId);
            await TriggerEtatNotificationsAsync(eq, req.Etat, userId);
        }
        if (eq.ResponsableId != req.ResponsableId)
            AddHistorique(id, "Equipement", "ChangementResponsable",
                eq.ResponsableId.ToString(), req.ResponsableId.ToString(), userId);
        if (eq.Localisation != req.Localisation)
            AddHistorique(id, "Equipement", "ChangementLocalisation",
                eq.Localisation, req.Localisation, userId);

        eq.Nom = req.Nom; eq.CategorieId = req.CategorieId; eq.Type = req.Type;
        eq.Marque = req.Marque; eq.Modele = req.Modele; eq.Localisation = req.Localisation;
        eq.ServiceId = req.ServiceId; eq.ResponsableId = req.ResponsableId;
        eq.DateInstallation = req.DateInstallation; eq.DateMiseEnService = req.DateMiseEnService;
        eq.Etat = req.Etat; eq.DateFinGarantie = req.DateFinGarantie;
        eq.ValeurAcquisition = req.ValeurAcquisition; eq.Fournisseur = req.Fournisseur;
        eq.Description = req.Description; eq.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, user);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal user)
    {
        var eq = await _db.Equipements.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new NotFoundException($"Équipement {id} introuvable.");
        _db.Equipements.Remove(eq);
        await _db.SaveChangesAsync();
    }

    public async Task<EquipementDetailDto> UpdateEtatAsync(Guid id, UpdateEtatRequest req, ClaimsPrincipal user)
    {
        var eq     = await LoadEquipementOrThrow(id);
        CheckScope(eq, user);
        var userId = user.GetUserId();
        AddHistorique(id, "Equipement", "ChangementEtat", eq.Etat.ToString(), req.Etat.ToString(), userId);
        await TriggerEtatNotificationsAsync(eq, req.Etat, userId);
        eq.Etat = req.Etat; eq.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, user);
    }

    public async Task<DocumentDto> UploadDocumentAsync(Guid id, IFormFile file, ClaimsPrincipal user)
    {
        if (!await _db.Equipements.AnyAsync(e => e.Id == id))
            throw new NotFoundException($"Équipement {id} introuvable.");
        var url = await _fileStorage.UploadAsync(file, $"{id}/documents");
        var doc = new EquipementDocument
        {
            Id = Guid.NewGuid(), EquipementId = id, NomFichier = file.FileName,
            Url = url, Extension = Path.GetExtension(file.FileName).ToLower(),
            TailleOctets = file.Length, UploadedAt = DateTime.UtcNow,
            UploadedById = user.GetUserId()
        };
        _db.EquipementDocuments.Add(doc);
        await _db.SaveChangesAsync();
        return new DocumentDto(doc.Id, doc.NomFichier, doc.Url, doc.Extension, doc.TailleOctets, doc.UploadedAt);
    }

    public async Task<PhotoDto> UploadPhotoAsync(Guid id, IFormFile file, ClaimsPrincipal user)
    {
        if (!await _db.Equipements.AnyAsync(e => e.Id == id))
            throw new NotFoundException($"Équipement {id} introuvable.");
        var url    = await _fileStorage.UploadAsync(file, $"{id}/photos");
        var isMain = !await _db.EquipementPhotos.AnyAsync(p => p.EquipementId == id);
        var photo  = new EquipementPhoto
        {
            Id = Guid.NewGuid(), EquipementId = id, Url = url, IsMain = isMain,
            UploadedAt = DateTime.UtcNow, UploadedById = user.GetUserId()
        };
        _db.EquipementPhotos.Add(photo);
        await _db.SaveChangesAsync();
        return new PhotoDto(photo.Id, photo.Url, photo.IsMain, photo.UploadedAt);
    }

    public async Task DeleteDocumentAsync(Guid equipementId, Guid documentId, ClaimsPrincipal user)
    {
        var doc = await _db.EquipementDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.EquipementId == equipementId)
            ?? throw new NotFoundException("Document introuvable.");
        await _fileStorage.DeleteAsync(doc.Url);
        _db.EquipementDocuments.Remove(doc);
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResult<HistoriqueEntryDto>> GetHistoriqueAsync(Guid id, int page, int pageSize, ClaimsPrincipal user)
    {
        if (!await _db.Equipements.AnyAsync(e => e.Id == id))
            throw new NotFoundException($"Équipement {id} introuvable.");

        var query = _db.HistoriqueEntries.AsNoTracking()
            .Where(h => h.EntiteId == id)
            .OrderByDescending(h => h.HorodatageUtc);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(h => new HistoriqueEntryDto(
                h.Id, h.EntiteType, h.TypeEvenement, h.ValeurAvant, h.ValeurApres,
                h.Auteur.Nom + " " + h.Auteur.Prenom, h.HorodatageUtc))
            .ToListAsync();

        return new PagedResult<HistoriqueEntryDto>
        {
            Items = items, TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Page = page, PageSize = pageSize
        };
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<Equipement> LoadEquipementOrThrow(Guid id) =>
        await _db.Equipements
            .Include(e => e.Categorie).Include(e => e.Service)
            .Include(e => e.Responsable).Include(e => e.Documents)
            .Include(e => e.Photos).Include(e => e.Techniciens)
            .FirstOrDefaultAsync(e => e.Id == id)
        ?? throw new NotFoundException($"Équipement {id} introuvable.");

    private static void CheckScope(Equipement eq, ClaimsPrincipal user)
    {
        var role      = user.GetRole();
        var userId    = user.GetUserId();
        var serviceId = user.GetServiceId();
        if (role == UserRole.Chef_de_Service && eq.ServiceId != serviceId) throw new ForbiddenException();
        if (role == UserRole.Technicien && !eq.Techniciens.Any(t => t.TechnicienId == userId)) throw new ForbiddenException();
    }

    private void AddHistorique(Guid entiteId, string entiteType, string typeEvenement,
        string? avant, string? apres, Guid auteurId)
    {
        _db.HistoriqueEntries.Add(new HistoriqueEntry
        {
            Id = Guid.NewGuid(), EntiteId = entiteId, EntiteType = entiteType,
            TypeEvenement = typeEvenement, ValeurAvant = avant, ValeurApres = apres,
            AuteurId = auteurId, HorodatageUtc = DateTime.UtcNow
        });
    }

    private async Task TriggerEtatNotificationsAsync(Equipement eq, EquipementEtat newEtat, Guid actorId)
    {
        if (newEtat != EquipementEtat.En_panne &&
            !(eq.Etat == EquipementEtat.En_panne && newEtat == EquipementEtat.Disponible)) return;

        var type = newEtat == EquipementEtat.En_panne ? "EquipementEnPanne" : "EquipementRemisEnService";
        var msg  = newEtat == EquipementEtat.En_panne
            ? $"L'équipement '{eq.Nom}' est en panne."
            : $"L'équipement '{eq.Nom}' est remis en service.";

        var admins = await _db.Users.Where(u => u.Role == "Admin_Patrimoine" && u.IsActive).Select(u => u.Id).ToListAsync();
        var chefs  = await _db.Users.Where(u => u.Role == "Chef_de_Service" && u.ServiceId == eq.ServiceId && u.IsActive).Select(u => u.Id).ToListAsync();

        foreach (var uid in admins.Concat(chefs).Distinct())
            await _notifService.CreateAsync(type, msg, eq.Id, "Equipement", uid);
    }

    private static EquipementDetailDto MapDetail(Equipement e) => new(
        e.Id, e.Reference, e.Nom,
        e.CategorieId, e.Categorie.Nom, e.Categorie.Code, e.Categorie.Couleur, e.Categorie.Icone,
        e.Type, e.Marque, e.Modele, e.NumeroSerie, e.Localisation,
        e.ServiceId, e.Service.Nom, e.ResponsableId,
        e.Responsable.Nom + " " + e.Responsable.Prenom,
        e.DateInstallation, e.DateMiseEnService, e.Etat,
        e.DateFinGarantie, e.ValeurAcquisition, e.Fournisseur, e.Description,
        e.CreatedAt, e.UpdatedAt,
        e.Documents.Select(d => new DocumentDto(d.Id, d.NomFichier, d.Url, d.Extension, d.TailleOctets, d.UploadedAt)),
        e.Photos.Select(p => new PhotoDto(p.Id, p.Url, p.IsMain, p.UploadedAt)));
}
