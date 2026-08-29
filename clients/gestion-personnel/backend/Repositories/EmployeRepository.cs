using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Employe;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Repositories;

public class EmployeRepository : IEmployeRepository
{
    private readonly AppDbContext _context;

    public EmployeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Employe> Items, int TotalCount)> GetPagedAsync(EmployeQueryDto query)
    {
        var q = _context.Employes
            .Include(e => e.Direction)
            .Include(e => e.Service)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            q = q.Where(e =>
                e.Nom.ToLower().Contains(search) ||
                e.Prenom.ToLower().Contains(search) ||
                e.Matricule.ToLower().Contains(search) ||
                e.Email.ToLower().Contains(search) ||
                e.Poste.ToLower().Contains(search));
        }

        if (query.DirectionId.HasValue)
            q = q.Where(e => e.DirectionId == query.DirectionId.Value);

        if (query.ServiceId.HasValue)
            q = q.Where(e => e.ServiceId == query.ServiceId.Value);

        if (!string.IsNullOrWhiteSpace(query.Statut) &&
            Enum.TryParse<StatutEmploye>(query.Statut, out var statut))
            q = q.Where(e => e.Statut == statut);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(e => e.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Employe?> GetByIdAsync(int id)
        => await _context.Employes
            .Include(e => e.Direction)
            .Include(e => e.Service)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employe?> GetByMatriculeAsync(string matricule)
        => await _context.Employes.FirstOrDefaultAsync(e => e.Matricule.ToLower() == matricule.ToLower());

    public async Task<Employe?> GetByEmailAsync(string email)
        => await _context.Employes.FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());

    public async Task<Employe> CreateAsync(Employe employe)
    {
        _context.Employes.Add(employe);
        await _context.SaveChangesAsync();
        return employe;
    }

    public async Task<Employe> UpdateAsync(Employe employe)
    {
        employe.UpdatedAt = DateTime.UtcNow;
        _context.Employes.Update(employe);
        await _context.SaveChangesAsync();
        return employe;
    }

    public async Task DeleteAsync(Employe employe)
    {
        _context.Employes.Remove(employe);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Employe>> GetLastAddedAsync(int count)
        => await _context.Employes
            .Include(e => e.Direction)
            .Include(e => e.Service)
            .OrderByDescending(e => e.CreatedAt)
            .Take(count)
            .ToListAsync();
}
