using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.DTOs.Dashboard;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _db;

    public DashboardService(IAppDbContext db) => _db = db;

    public async Task<object> GetDashboardAsync(ClaimsPrincipal user)
    {
        var role = user.GetRole();
        return role switch
        {
            UserRole.Admin_Patrimoine => await GetAdminDashboard(),
            UserRole.Directeur => await GetDirecteurDashboard(),
            UserRole.Chef_de_Service => await GetChefDashboard(user.GetServiceId()),
            UserRole.Technicien => await GetTechnicienDashboard(user.GetUserId()),
            _ => new { }
        };
    }

    private async Task<AdminDashboardDto> GetAdminDashboard()
    {
        var total = await _db.Equipements.CountAsync();
        var parEtat = await _db.Equipements
            .GroupBy(e => e.Etat)
            .Select(g => new { Etat = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();
        var planned = await _db.Maintenances.CountAsync(m => m.Statut == MaintenanceStatut.Planifiee);
        var retard  = await _db.Maintenances.CountAsync(m => m.Statut == MaintenanceStatut.En_retard);
        var cout    = await _db.Maintenances
            .Where(m => m.Statut == MaintenanceStatut.Planifiee && m.CoutEstime != null)
            .SumAsync(m => (decimal?)m.CoutEstime) ?? 0;

        return new AdminDashboardDto(total,
            parEtat.ToDictionary(x => x.Etat, x => x.Count),
            planned, retard, cout);
    }

    private async Task<DirecteurDashboardDto> GetDirecteurDashboard()
    {
        var total = await _db.Equipements.CountAsync();

        var parCat = await _db.Equipements
            .GroupBy(e => e.Categorie.Nom)
            .Select(g => new StatItem(g.Key, g.Count()))
            .ToListAsync();

        var parSvc = await _db.Equipements
            .GroupBy(e => e.Service.Nom)
            .Select(g => new StatItem(g.Key, g.Count()))
            .ToListAsync();

        var parEtat = await _db.Equipements
            .GroupBy(e => e.Etat)
            .Select(g => new { Etat = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var alertes = await _db.Equipements
            .Where(e => e.Etat == EquipementEtat.En_panne || e.Etat == EquipementEtat.Hors_service)
            .Select(e => new EquipementAlerte(e.Id, e.Nom, e.Reference, e.Etat.ToString(), e.Service.Nom))
            .Take(20)
            .ToListAsync();

        return new DirecteurDashboardDto(total, parCat, parSvc,
            parEtat.ToDictionary(x => x.Etat, x => x.Count), alertes);
    }

    private async Task<ChefServiceDashboardDto> GetChefDashboard(Guid? serviceId)
    {
        var now = DateTime.UtcNow;
        var total      = await _db.Equipements.CountAsync(e => e.ServiceId == serviceId);
        var aVenir     = await _db.Maintenances.CountAsync(m =>
            m.Equipement.ServiceId == serviceId &&
            m.Statut == MaintenanceStatut.Planifiee &&
            m.DatePlanifiee <= now.AddDays(7));
        var indispos   = await _db.Equipements.CountAsync(e =>
            e.ServiceId == serviceId &&
            (e.Etat == EquipementEtat.En_panne || e.Etat == EquipementEtat.Hors_service || e.Etat == EquipementEtat.En_maintenance));
        var recents    = await _db.Equipements.CountAsync(e =>
            e.ServiceId == serviceId && e.DateInstallation >= now.AddDays(-30));

        return new ChefServiceDashboardDto(total, aVenir, indispos, recents);
    }

    private async Task<TechnicienDashboardDto> GetTechnicienDashboard(Guid userId)
    {
        var now    = DateTime.UtcNow;
        var today  = now.Date;
        var affectes     = await _db.TechnicienEquipements.CountAsync(t => t.TechnicienId == userId);
        var mainAujourd  = await _db.Maintenances.CountAsync(m =>
            m.TechnicienId == userId && m.DatePlanifiee.Date == today);
        var prochaines   = await _db.Maintenances.CountAsync(m =>
            m.TechnicienId == userId &&
            m.Statut == MaintenanceStatut.Planifiee &&
            m.DatePlanifiee <= now.AddDays(7));
        var interventions = await _db.Maintenances.CountAsync(m =>
            m.TechnicienId == userId &&
            m.Statut == MaintenanceStatut.Terminee &&
            m.DateCloture >= now.AddDays(-30));

        return new TechnicienDashboardDto(affectes, mainAujourd, prochaines, interventions);
    }
}
