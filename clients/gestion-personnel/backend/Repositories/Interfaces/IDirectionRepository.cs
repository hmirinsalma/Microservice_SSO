using GestionPersonnel.API.Models;

namespace GestionPersonnel.API.Repositories.Interfaces;

public interface IDirectionRepository
{
    Task<IEnumerable<Direction>> GetAllAsync();
    Task<Direction?> GetByIdAsync(int id);
    Task<Direction?> GetByNomAsync(string nom);
    Task<Direction> CreateAsync(Direction direction);
    Task<Direction> UpdateAsync(Direction direction);
    Task DeleteAsync(Direction direction);
    Task<bool> HasServicesAsync(int id);
    Task<bool> HasEmployesAsync(int id);
}
