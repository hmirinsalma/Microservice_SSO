using GestionPersonnel.API.Data;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Repositories;

public class DirectionRepository : IDirectionRepository
{
    private readonly AppDbContext _context;

    public DirectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Direction>> GetAllAsync()
        => await _context.Directions
            .Include(d => d.Services)
            .Include(d => d.Employes)
            .OrderBy(d => d.Nom)
            .ToListAsync();

    public async Task<Direction?> GetByIdAsync(int id)
        => await _context.Directions
            .Include(d => d.Services)
            .Include(d => d.Employes)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Direction?> GetByNomAsync(string nom)
        => await _context.Directions.FirstOrDefaultAsync(d => d.Nom.ToLower() == nom.ToLower());

    public async Task<Direction> CreateAsync(Direction direction)
    {
        _context.Directions.Add(direction);
        await _context.SaveChangesAsync();
        return direction;
    }

    public async Task<Direction> UpdateAsync(Direction direction)
    {
        _context.Directions.Update(direction);
        await _context.SaveChangesAsync();
        return direction;
    }

    public async Task DeleteAsync(Direction direction)
    {
        _context.Directions.Remove(direction);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasServicesAsync(int id)
        => await _context.Services.AnyAsync(s => s.DirectionId == id);

    public async Task<bool> HasEmployesAsync(int id)
        => await _context.Employes.AnyAsync(e => e.DirectionId == id);
}
