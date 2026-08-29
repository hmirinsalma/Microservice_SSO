using GestionPersonnel.API.DTOs.Direction;

namespace GestionPersonnel.API.Services.Interfaces;

public interface IDirectionService
{
    Task<IEnumerable<DirectionDto>> GetAllAsync();
    Task<DirectionDto> GetByIdAsync(int id);
    Task<DirectionDto> CreateAsync(CreateDirectionDto dto);
    Task<DirectionDto> UpdateAsync(int id, UpdateDirectionDto dto);
    Task DeleteAsync(int id);
}
