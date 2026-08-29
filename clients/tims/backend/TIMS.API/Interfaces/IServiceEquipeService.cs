using TIMS.API.DTOs.User;

namespace TIMS.API.Interfaces;

public interface IServiceEquipeService
{
    Task<List<ServiceDto>> GetAllServicesAsync();
    Task<ServiceDto> GetServiceByIdAsync(int id);
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto);
    Task<ServiceDto> UpdateServiceAsync(int id, CreateServiceDto dto);
    Task<List<EquipeDto>> GetAllEquipesAsync();
    Task<List<EquipeDto>> GetEquipesByServiceAsync(int serviceId);
    Task<EquipeDto> CreateEquipeAsync(CreateEquipeDto dto);
    Task<EquipeDto> UpdateEquipeAsync(int id, CreateEquipeDto dto);
}
