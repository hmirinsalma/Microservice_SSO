using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Common;
using GestionPersonnel.API.DTOs.Conge;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Services;

public class CongeService : ICongeService
{
    private readonly AppDbContext _db;
    public CongeService(AppDbContext db) => _db = db;

    // ── Créer une demande ────────────────────────────────────
    public async Task<CongeDto> CreateAsync(int employeId, CreateCongeDto dto)
    {
        if (dto.DateFin < dto.DateDebut)
            throw new AppException("La date de fin doit être postérieure à la date de début.");

        var employe = await _db.Employes.Include(e => e.Direction).Include(e => e.Service)
            .FirstOrDefaultAsync(e => e.Id == employeId)
            ?? throw new NotFoundException("Employé", employeId);

        var conge = new Conge
        {
            EmployeId  = employeId,
            DateDebut  = dto.DateDebut,
            DateFin    = dto.DateFin,
            Motif      = dto.Motif,
            Statut     = StatutConge.EnAttente,
            CreatedAt  = DateTime.UtcNow,
        };
        _db.Conges.Add(conge);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(conge.Id);
    }

    // ── Mes congés ───────────────────────────────────────────
    public async Task<IEnumerable<CongeDto>> GetMyCongesAsync(int employeId)
    {
        var list = await _db.Conges
            .Include(c => c.Employe).ThenInclude(e => e.Direction)
            .Include(c => c.Employe).ThenInclude(e => e.Service)
            .Where(c => c.EmployeId == employeId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return list.Select(ToDto);
    }

    // ── Par service ──────────────────────────────────────────
    public async Task<IEnumerable<CongeDto>> GetByServiceAsync(int serviceId, string? statut = null)
    {
        var q = _db.Conges
            .Include(c => c.Employe).ThenInclude(e => e.Direction)
            .Include(c => c.Employe).ThenInclude(e => e.Service)
            .Where(c => c.Employe.ServiceId == serviceId);

        if (!string.IsNullOrEmpty(statut) && Enum.TryParse<StatutConge>(statut, out var s))
            q = q.Where(c => c.Statut == s);

        return (await q.OrderByDescending(c => c.CreatedAt).ToListAsync()).Select(ToDto);
    }

    // ── Par direction (pour directeur — seulement validés chef) ─
    public async Task<IEnumerable<CongeDto>> GetByDirectionAsync(int directionId, string? statut = null)
    {
        var q = _db.Conges
            .Include(c => c.Employe).ThenInclude(e => e.Direction)
            .Include(c => c.Employe).ThenInclude(e => e.Service)
            .Where(c => c.Employe.DirectionId == directionId
                     && (c.Statut == StatutConge.ValideChef
                      || c.Statut == StatutConge.ValideDirecteur
                      || c.Statut == StatutConge.Refuse));

        if (!string.IsNullOrEmpty(statut) && Enum.TryParse<StatutConge>(statut, out var s))
            q = q.Where(c => c.Statut == s);

        return (await q.OrderByDescending(c => c.CreatedAt).ToListAsync()).Select(ToDto);
    }

    // ── Tous (admin) ─────────────────────────────────────────
    public async Task<PagedResultDto<CongeDto>> GetAllAsync(CongeQueryDto query)
    {
        var q = _db.Conges
            .Include(c => c.Employe).ThenInclude(e => e.Direction)
            .Include(c => c.Employe).ThenInclude(e => e.Service)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Statut) && Enum.TryParse<StatutConge>(query.Statut, out var s))
            q = q.Where(c => c.Statut == s);
        if (query.EmployeId.HasValue)
            q = q.Where(c => c.EmployeId == query.EmployeId.Value);
        if (query.DirectionId.HasValue)
            q = q.Where(c => c.Employe.DirectionId == query.DirectionId.Value);
        if (query.ServiceId.HasValue)
            q = q.Where(c => c.Employe.ServiceId == query.ServiceId.Value);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(c => c.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .ToListAsync();

        return new PagedResultDto<CongeDto>
        {
            Data       = items.Select(ToDto),
            TotalCount = total,
            Page       = query.Page,
            PageSize   = query.PageSize,
        };
    }

    // ── Traitement chef ──────────────────────────────────────
    public async Task<CongeDto> TraiterParChefAsync(int congeId, int chefEmployeId, TraiterCongeDto dto)
    {
        var conge = await _db.Conges.Include(c => c.Employe)
            .FirstOrDefaultAsync(c => c.Id == congeId)
            ?? throw new NotFoundException("Congé", congeId);

        if (conge.Statut != StatutConge.EnAttente)
            throw new AppException("Ce congé ne peut plus être traité (statut non valide).");

        conge.ChefServiceId           = chefEmployeId;
        conge.CommentaireChef         = dto.Commentaire;
        conge.DateTraitementChef      = DateTime.UtcNow;
        conge.Statut                  = dto.Accepter ? StatutConge.ValideChef : StatutConge.Refuse;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(conge.Id);
    }

    // ── Traitement directeur ─────────────────────────────────
    public async Task<CongeDto> TraiterParDirecteurAsync(int congeId, int directeurEmployeId, TraiterCongeDto dto)
    {
        var conge = await _db.Conges.Include(c => c.Employe)
            .FirstOrDefaultAsync(c => c.Id == congeId)
            ?? throw new NotFoundException("Congé", congeId);

        if (conge.Statut != StatutConge.ValideChef)
            throw new AppException("Ce congé doit d'abord être validé par le chef de service.");

        conge.DirecteurId                 = directeurEmployeId;
        conge.CommentaireDirecteur        = dto.Commentaire;
        conge.DateTraitementDirecteur     = DateTime.UtcNow;
        conge.Statut                      = dto.Accepter ? StatutConge.ValideDirecteur : StatutConge.Refuse;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(conge.Id);
    }

    // ── Annuler ──────────────────────────────────────────────
    public async Task AnnulerAsync(int congeId, int employeId)
    {
        var conge = await _db.Conges.FirstOrDefaultAsync(c => c.Id == congeId)
            ?? throw new NotFoundException("Congé", congeId);

        if (conge.EmployeId != employeId)
            throw new AppException("Vous ne pouvez annuler que vos propres demandes.", 403);

        if (conge.Statut != StatutConge.EnAttente)
            throw new AppException("Seules les demandes en attente peuvent être annulées.");

        conge.Statut = StatutConge.Annule;
        await _db.SaveChangesAsync();
    }

    // ── Par Id ───────────────────────────────────────────────
    public async Task<CongeDto> GetByIdAsync(int id)
    {
        var conge = await _db.Conges
            .Include(c => c.Employe).ThenInclude(e => e.Direction)
            .Include(c => c.Employe).ThenInclude(e => e.Service)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("Congé", id);
        return ToDto(conge);
    }

    // ── Mapper ───────────────────────────────────────────────
    private static CongeDto ToDto(Conge c) => new()
    {
        Id                        = c.Id,
        DateDebut                 = c.DateDebut,
        DateFin                   = c.DateFin,
        NombreJours               = (int)(c.DateFin - c.DateDebut).TotalDays + 1,
        Motif                     = c.Motif,
        Statut                    = c.Statut.ToString(),
        CommentaireChef           = c.CommentaireChef,
        CommentaireDirecteur      = c.CommentaireDirecteur,
        DateTraitementChef        = c.DateTraitementChef,
        DateTraitementDirecteur   = c.DateTraitementDirecteur,
        CreatedAt                 = c.CreatedAt,
        EmployeId                 = c.EmployeId,
        EmployeNom                = c.Employe?.Nom ?? "",
        EmployePrenom             = c.Employe?.Prenom ?? "",
        EmployeMatricule          = c.Employe?.Matricule ?? "",
        DirectionNom              = c.Employe?.Direction?.Nom ?? "",
        ServiceNom                = c.Employe?.Service?.Nom ?? "",
    };
}
