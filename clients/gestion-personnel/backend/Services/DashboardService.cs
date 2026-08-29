using AutoMapper;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Dashboard;
using GestionPersonnel.API.DTOs.Employe;
using GestionPersonnel.API.DTOs.Conge;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public DashboardService(AppDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    // ── Admin RH ─────────────────────────────────────────────
    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        var employes   = await _db.Employes.Include(e => e.Direction).Include(e => e.Service).ToListAsync();
        var directions = await _db.Directions.ToListAsync();
        var services   = await _db.Services.ToListAsync();
        var conges     = await _db.Conges.ToListAsync();

        var derniers = employes.OrderByDescending(e => e.CreatedAt).Take(5).ToList();

        return new AdminDashboardDto
        {
            TotalEmployes          = employes.Count,
            TotalDirections        = directions.Count,
            TotalServices          = services.Count,
            TotalCongesEnAttente   = conges.Count(c => c.Statut == StatutConge.EnAttente),
            TotalConges            = conges.Count,
            DerniersEmployes       = _mapper.Map<IEnumerable<EmployeDto>>(derniers),
            EmployesParDirection   = directions.Select(d => new DirectionStatDto
            {
                Nom              = d.Nom,
                NombreEmployes   = employes.Count(e => e.DirectionId == d.Id),
            }),
        };
    }

    // ── Directeur ────────────────────────────────────────────
    public async Task<DirecteurDashboardDto> GetDirecteurDashboardAsync(int directionId)
    {
        var employes = await _db.Employes
            .Include(e => e.Direction).Include(e => e.Service)
            .Where(e => e.DirectionId == directionId).ToListAsync();

        var services = await _db.Services.Where(s => s.DirectionId == directionId).ToListAsync();

        var congesEnAttente = await _db.Conges
            .CountAsync(c => c.Employe.DirectionId == directionId
                          && c.Statut == StatutConge.ValideChef);

        var direction = await _db.Directions.FindAsync(directionId);
        var derniers  = employes.OrderByDescending(e => e.CreatedAt).Take(5).ToList();

        return new DirecteurDashboardDto
        {
            TotalEmployes      = employes.Count,
            TotalServices      = services.Count,
            DirectionNom       = direction?.Nom ?? "",
            CongesEnAttente    = congesEnAttente,
            DerniersRecrutes   = _mapper.Map<IEnumerable<EmployeDto>>(derniers),
            EmployesParService = services.Select(s => new ServiceStatDto
            {
                Nom            = s.Nom,
                NombreEmployes = employes.Count(e => e.ServiceId == s.Id),
            }),
        };
    }

    // ── Chef de service ──────────────────────────────────────
    public async Task<ChefServiceDashboardDto> GetChefServiceDashboardAsync(int serviceId)
    {
        var employes = await _db.Employes
            .Include(e => e.Direction).Include(e => e.Service)
            .Where(e => e.ServiceId == serviceId).ToListAsync();

        var service = await _db.Services.FindAsync(serviceId);

        var conges = await _db.Conges
            .Where(c => c.Employe.ServiceId == serviceId).ToListAsync();

        return new ChefServiceDashboardDto
        {
            TotalEmployes    = employes.Count,
            ServiceNom       = service?.Nom ?? "",
            CongesEnAttente  = conges.Count(c => c.Statut == StatutConge.EnAttente),
            CongesAcceptes   = conges.Count(c => c.Statut == StatutConge.ValideDirecteur),
            CongesRefuses    = conges.Count(c => c.Statut == StatutConge.Refuse),
            Employes         = _mapper.Map<IEnumerable<EmployeDto>>(employes),
        };
    }

    // ── Employé ──────────────────────────────────────────────
    public async Task<EmployeDashboardDto> GetEmployeDashboardAsync(int employeId)
    {
        var employe = await _db.Employes
            .Include(e => e.Direction).Include(e => e.Service)
            .Include(e => e.Responsable)
            .FirstOrDefaultAsync(e => e.Id == employeId);

        var conges = await _db.Conges
            .Include(c => c.Employe).ThenInclude(e => e.Direction)
            .Include(c => c.Employe).ThenInclude(e => e.Service)
            .Where(c => c.EmployeId == employeId)
            .OrderByDescending(c => c.CreatedAt).ToListAsync();

        return new EmployeDashboardDto
        {
            Profil              = employe != null ? _mapper.Map<EmployeDto>(employe) : null,
            CongesEnAttente     = conges.Count(c => c.Statut == StatutConge.EnAttente),
            CongesAcceptes      = conges.Count(c => c.Statut == StatutConge.ValideDirecteur),
            CongesRefuses       = conges.Count(c => c.Statut == StatutConge.Refuse),
            DernieresDemandesConge = conges.Take(5).Select(c => new CongeDto
            {
                Id = c.Id, DateDebut = c.DateDebut, DateFin = c.DateFin,
                Motif = c.Motif, Statut = c.Statut.ToString(),
                NombreJours = (int)(c.DateFin - c.DateDebut).TotalDays + 1,
                CreatedAt = c.CreatedAt,
                EmployeId = c.EmployeId,
                EmployeNom = c.Employe?.Nom ?? "",
                EmployePrenom = c.Employe?.Prenom ?? "",
                EmployeMatricule = c.Employe?.Matricule ?? "",
            }),
        };
    }
}
