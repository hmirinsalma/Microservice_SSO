using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();

    Task<RoleDto?> GetByIdAsync(Guid id);

    Task<RoleDto> CreateAsync(CreateRoleDto dto);

    Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto dto);
    Task<IEnumerable<RoleDto>> GetByClientAsync(Guid clientId);

    Task DeleteAsync(Guid id);
}