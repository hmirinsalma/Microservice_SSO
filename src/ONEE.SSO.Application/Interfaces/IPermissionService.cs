using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllAsync();

    Task<PermissionDto?> GetByIdAsync(Guid id);

    Task<PermissionDto> CreateAsync(CreatePermissionDto dto);

    Task<PermissionDto> UpdateAsync(Guid id, UpdatePermissionDto dto);

    Task DeleteAsync(Guid id);
}