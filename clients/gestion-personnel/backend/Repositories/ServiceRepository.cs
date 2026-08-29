using GestionPersonnel.API.Data;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
        => await _context.Services
            .Include(s => s.Direction)
            .Include(s => s.Employes)
            .OrderBy(s => s.Direction.Nom)
            .ThenBy(s => s.Nom)
            .ToListAsync();

    public async Task<IEnumerable<Service>> GetByDirectionAsync(int directionId)
        => await _context.Services
            .Include(s => s.Direction)
            .Include(s => s.Employes)
            .Where(s => s.DirectionId == directionId)
            .OrderBy(s => s.Nom)
            .ToListAsync();

    public async Task<Service?> GetByIdAsync(int id)
        => await _context.Services
            .Include(s => s.Direction)
            .Include(s => s.Employes)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Service?> GetByNomAndDirectionAsync(string nom, int directionId)
        => await _context.Services.FirstOrDefaultAsync(s =>
            s.Nom.ToLower() == nom.ToLower() && s.DirectionId == directionId);

    public async Task<Service> CreateAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<Service> UpdateAsync(Service service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task DeleteAsync(Service service)
    {
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasEmployesAsync(int id)
        => await _context.Employes.AnyAsync(e => e.ServiceId == id);
}
