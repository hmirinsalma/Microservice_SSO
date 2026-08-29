using Microsoft.EntityFrameworkCore;
using TIMS.API.Data;
using TIMS.API.DTOs.Dashboard;
using TIMS.API.DTOs.Intervention;
using TIMS.API.Entities;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;
    public DashboardService(ApplicationDbContext db) { _db = db; }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        var interventions = await _db.Interventions
            .Where(i => !i.IsDeleted)
            .Include(i => i.Equipe).Include(i => i.Service)
            .ToListAsync();

        return new AdminDashboardDto
        {
            TotalInterventions = interventions.Count,
            Nouvelles   = interventions.Count(i => i.Status == InterventionStatus.Nouvelle),
            EnCours     = interventions.Count(i => i.Status == InterventionStatus.EnCours),
            Suspendues  = interventions.Count(i => i.Status == InterventionStatus.Suspendue),
            Terminees   = interventions.Count(i => i.Status == InterventionStatus.Terminee),
            Annulees    = interventions.Count(i => i.Status == InterventionStatus.Annulee),
            Urgentes    = interventions.Count(i => i.Priority == InterventionPriority.Urgente),
            Critiques   = interventions.Count(i => i.Priority == InterventionPriority.Critique),
            ByPriority  = interventions.GroupBy(i => i.Priority)
                .Select(g => new StatItem { Label = g.Key.ToString(), Count = g.Count() }).ToList(),
            ByStatus    = interventions.GroupBy(i => i.Status)
                .Select(g => new StatItem { Label = g.Key.ToString(), Count = g.Count() }).ToList(),
            ByEquipe    = interventions.Where(i => i.Equipe != null).GroupBy(i => i.Equipe!.Name)
                .Select(g => new StatItem { Label = g.Key, Count = g.Count() }).ToList()
        };
    }

    public async Task<DirecteurDashboardDto> GetDirecteurDashboardAsync()
    {
        var interventions = await _db.Interventions
            .Where(i => !i.IsDeleted)
            .Include(i => i.Equipe).Include(i => i.Service)
            .ToListAsync();
        var since30 = DateTime.UtcNow.AddDays(-30);

        return new DirecteurDashboardDto
        {
            TotalInterventions         = interventions.Count,
            InterventionsCritiques     = interventions.Count(i => i.Priority == InterventionPriority.Critique),
            InterventionsCloturees30j  = interventions.Count(i =>
                i.Status == InterventionStatus.Terminee && i.DateCloture >= since30),
            ByStatus  = interventions.GroupBy(i => i.Status)
                .Select(g => new StatItem { Label = g.Key.ToString(), Count = g.Count() }).ToList(),
            ByEquipe  = interventions.Where(i => i.Equipe != null).GroupBy(i => i.Equipe!.Name)
                .Select(g => new StatItem { Label = g.Key, Count = g.Count() }).ToList(),
            ByService = interventions.Where(i => i.Service != null).GroupBy(i => i.Service!.Name)
                .Select(g => new StatItem { Label = g.Key, Count = g.Count() }).ToList()
        };
    }

    public async Task<ChefServiceDashboardDto> GetChefServiceDashboardAsync(int serviceId)
    {
        var interventions = await _db.Interventions
            .Where(i => !i.IsDeleted && i.ServiceId == serviceId).ToListAsync();

        // Techniciens du service
        var techIds = await _db.Users
            .Where(u => u.ServiceId == serviceId && u.IsActive &&
                        u.UserRoles.Any(ur => ur.Role.Name == RoleNames.Technicien))
            .Select(u => u.Id).ToListAsync();

        var occupes = await _db.Interventions
            .Where(i => !i.IsDeleted && i.Status == InterventionStatus.EnCours
                        && i.TechnicienId.HasValue && techIds.Contains(i.TechnicienId.Value))
            .Select(i => i.TechnicienId!.Value).Distinct().CountAsync();

        return new ChefServiceDashboardDto
        {
            TotalServiceInterventions = interventions.Count,
            Urgentes    = interventions.Count(i => i.Priority >= InterventionPriority.Urgente),
            EnAttente   = interventions.Count(i => i.TechnicienId == null && i.Status == InterventionStatus.Nouvelle),
            TechniciensDisponibles = techIds.Count - occupes,
            TechniciensOccupes     = occupes,
            ByStatus = interventions.GroupBy(i => i.Status)
                .Select(g => new StatItem { Label = g.Key.ToString(), Count = g.Count() }).ToList()
        };
    }

    public async Task<TechnicienDashboardDto> GetTechnicienDashboardAsync(int technicienId)
    {
        var interventions = await _db.Interventions
            .Where(i => !i.IsDeleted && i.TechnicienId == technicienId)
            .OrderBy(i => i.DatePrevue).ToListAsync();

        return new TechnicienDashboardDto
        {
            TotalAffectees = interventions.Count,
            EnCours    = interventions.Count(i => i.Status == InterventionStatus.EnCours),
            Terminees  = interventions.Count(i => i.Status == InterventionStatus.Terminee),
            Urgentes   = interventions.Count(i => i.Priority >= InterventionPriority.Urgente),
            Prochaines = interventions
                .Where(i => i.Status != InterventionStatus.Terminee && i.Status != InterventionStatus.Annulee)
                .Take(5)
                .Select(i => new ProchainInterventionDto
                {
                    Id = i.Id, NumeroIntervention = i.NumeroIntervention,
                    Objet = i.Objet, DatePrevue = i.DatePrevue,
                    PriorityLabel = i.Priority.ToString(), StatusLabel = i.Status.ToString()
                }).ToList()
        };
    }
}
