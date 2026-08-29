using GestionPersonnel.API.DTOs.Service;

namespace GestionPersonnel.API.Services.Interfaces;

public interface IServiceService
{
    Task<IEnumerable<ServiceDto>> GetAllAsync();
    Task<IEnumerable<ServiceDto>> GetByDirectionAsync(int directionId);
    Task<ServiceDto> GetByIdAsync(int id);
    Task<ServiceDto> CreateAsync(CreateServiceDto dto);
    Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto);
    Task DeleteAsync(int id);
}
