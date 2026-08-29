using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Maintenance;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;
using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IAppDbContext _db;
    private readonly INotificationService _notifService;

    public MaintenanceService(IAppDbContext db, INotificationService notifService)
    { _db = db; _notifService = notifService; }

    public async Task<PagedResult<MaintenanceListDto>> GetAllAsync(MaintenanceFilterRequest filter, ClaimsPrincipal user)
    {
        var role      = user.GetRole();
        var userId    = user.GetUserId();
        var serviceId = user.GetServiceId();

        var query = _db.Maintenances.AsNoTracking().AsQueryable();

        // RBAC scope
        query = role switch
        {
            UserRole.Chef_de_Service when serviceId.HasValue
                => query.Where(m => m.Equipement.ServiceId == serviceId),
            UserRole.Technicien
                => query.Where(m => m.TechnicienId == userId),
            _ => query
        };

        if (filter.EquipementId.HasValue) query = query.Where(m => m.EquipementId == filter.EquipementId);
        if (filter.TechnicienId.HasValue) query = query.Where(m => m.TechnicienId == filter.TechnicienId);
        if (filter.Type.HasValue)         query = query.Where(m => m.Type == filter.Type);
        if (filter.Statut.HasValue)       query = query.Where(m => m.Statut == filter.Statut);
        if (filter.DateFrom.HasValue)     query = query.Where(m => m.DatePlanifiee >= filter.DateFrom);
        if (filter.DateTo.HasValue)       query = query.Where(m => m.DatePlanifiee <= filter.DateTo);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.DatePlanifiee)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(m => new MaintenanceListDto(
                m.Id, m.EquipementId, m.Equipement.Nom, m.Equipement.Reference,
                m.TechnicienId, m.Technicien.Nom + " " + m.Technicien.Prenom,
                m.Type, m.Statut, m.DatePlanifiee, m.CoutEstime, m.CreatedAt))
            .ToListAsync();

        return new PagedResult<MaintenanceListDto>
        {
            Items = items, TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)filter.PageSize),
            Page = filter.Page, PageSize = filter.PageSize
        };
    }

    public async Task<MaintenanceDetailDto> GetByIdAsync(Guid id, ClaimsPrincipal user)
    {
        var m = await LoadOrThrow(id);
        CheckScope(m, user);
        return MapDetail(m);
    }

    public async Task<MaintenanceDetailDto> CreateAsync(CreateMaintenanceRequest req, ClaimsPrincipal user)
    {
        if (req.DatePlanifiee.Date < DateTime.UtcNow.Date)
            throw new ValidationException(["La date planifiée ne peut pas être antérieure à aujourd'hui."]);

        var role      = user.GetRole();
        var serviceId = user.GetServiceId();

        if (role == UserRole.Chef_de_Service)
        {
            var eq = await _db.Equipements.Select(e => new { e.Id, e.ServiceId })
                .FirstOrDefaultAsync(e => e.Id == req.EquipementId)
                ?? throw new NotFoundException("Équipement introuvable.");
            if (eq.ServiceId != serviceId) throw new ForbiddenException();
        }

        var m = new Maintenance
        {
            Id = Guid.NewGuid(), EquipementId = req.EquipementId, TechnicienId = req.TechnicienId,
            Type = req.Type, Statut = MaintenanceStatut.Planifiee,
            DatePlanifiee = req.DatePlanifiee, DureeMinutes = req.DureeMinutes,
            CoutEstime = req.CoutEstime, Observations = req.Observations,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _db.Maintenances.Add(m);
        await _db.SaveChangesAsync();

        var eqNom = await _db.Equipements.Where(e => e.Id == req.EquipementId).Select(e => e.Nom).FirstOrDefaultAsync();
        await _notifService.CreateAsync("MaintenancePlanifiee",
            $"Maintenance planifiée pour '{eqNom}' le {req.DatePlanifiee:dd/MM/yyyy}.",
            m.Id, "Maintenance", req.TechnicienId);

        return await GetByIdAsync(m.Id, user);
    }

    public async Task<MaintenanceDetailDto> UpdateAsync(Guid id, UpdateMaintenanceRequest req, ClaimsPrincipal user)
    {
        var m = await LoadOrThrow(id);
        CheckScope(m, user);
        m.TechnicienId = req.TechnicienId; m.Type = req.Type; m.Statut = req.Statut;
        m.DatePlanifiee = req.DatePlanifiee; m.DureeMinutes = req.DureeMinutes;
        m.Observations = req.Observations; m.PiecesRemplacees = req.PiecesRemplacees;
        m.CoutEstime = req.CoutEstime; m.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return MapDetail(m);
    }

    public async Task<MaintenanceDetailDto> CloturerAsync(Guid id, CloturerMaintenanceRequest req, ClaimsPrincipal user)
    {
        var m = await LoadOrThrow(id);
        CheckScope(m, user);
        m.Statut = MaintenanceStatut.Terminee; m.EtatAvant = req.EtatAvant;
        m.EtatApres = req.EtatApres; m.Observations = req.Observations;
        m.PiecesRemplacees = req.PiecesRemplacees; m.CoutReel = req.CoutReel;
        m.DateCloture = DateTime.UtcNow; m.ProchaineMaintenance = req.ProchaineMaintenance;
        m.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return MapDetail(m);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal user)
    {
        var m = await _db.Maintenances.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Maintenance {id} introuvable.");
        _db.Maintenances.Remove(m);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateRetardStatusAsync()
    {
        var retards = await _db.Maintenances
            .Include(m => m.Equipement).ThenInclude(e => e.Service)
            .Where(m => m.Statut == MaintenanceStatut.Planifiee && m.DatePlanifiee < DateTime.UtcNow)
            .ToListAsync();

        if (!retards.Any()) return;

        var adminIds = await _db.Users
            .Where(u => u.RoleMetier == UserRole.Admin_Patrimoine && u.IsActive)
            .Select(u => u.Id).ToListAsync();

        var notifs = new List<Notification>();
        foreach (var m in retards)
        {
            m.Statut = MaintenanceStatut.En_retard;
            m.UpdatedAt = DateTime.UtcNow;

            var chefIds = await _db.Users
                .Where(u => u.RoleMetier == UserRole.Chef_de_Service && u.ServiceId == m.Equipement.ServiceId && u.IsActive)
                .Select(u => u.Id).ToListAsync();

            foreach (var uid in adminIds.Concat(chefIds).Distinct())
            {
                notifs.Add(new Notification
                {
                    Id = Guid.NewGuid(), TypeEvenement = "MaintenanceEnRetard",
                    Message = $"Maintenance de '{m.Equipement.Nom}' en retard.",
                    RessourceId = m.Id, RessourceType = "Maintenance",
                    DestinataireId = uid, EstLue = false, CreatedAt = DateTime.UtcNow
                });
            }
        }
        _db.Notifications.AddRange(notifs);
        await _db.SaveChangesAsync();
    }

    private async Task<Maintenance> LoadOrThrow(Guid id) =>
        await _db.Maintenances
            .Include(m => m.Equipement).ThenInclude(e => e.Service)
            .Include(m => m.Technicien)
            .FirstOrDefaultAsync(m => m.Id == id)
        ?? throw new NotFoundException($"Maintenance {id} introuvable.");

    private static void CheckScope(Maintenance m, ClaimsPrincipal user)
    {
        var role      = user.GetRole();
        var userId    = user.GetUserId();
        var serviceId = user.GetServiceId();
        if (role == UserRole.Chef_de_Service && m.Equipement.ServiceId != serviceId) throw new ForbiddenException();
        if (role == UserRole.Technicien && m.TechnicienId != userId) throw new ForbiddenException();
    }

    private static MaintenanceDetailDto MapDetail(Maintenance m) => new(
        m.Id, m.EquipementId, m.Equipement.Nom, m.Equipement.Reference,
        m.TechnicienId, m.Technicien.Nom + " " + m.Technicien.Prenom,
        m.Type, m.Statut, m.DatePlanifiee, m.DateDebut, m.DateCloture, m.DureeMinutes,
        m.EtatAvant, m.EtatApres, m.Observations, m.PiecesRemplacees,
        m.CoutEstime, m.CoutReel, m.ProchaineMaintenance, m.CreatedAt, m.UpdatedAt);
}
