using GestionPersonnel.API.Models;

namespace GestionPersonnel.API.Repositories.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<IEnumerable<Service>> GetByDirectionAsync(int directionId);
    Task<Service?> GetByIdAsync(int id);
    Task<Service?> GetByNomAndDirectionAsync(string nom, int directionId);
    Task<Service> CreateAsync(Service service);
    Task<Service> UpdateAsync(Service service);
    Task DeleteAsync(Service service);
    Task<bool> HasEmployesAsync(int id);
}
