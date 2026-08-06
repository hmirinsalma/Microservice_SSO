using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IUserRoleService
{
    Task AssignRoleAsync(Guid userId, Guid roleId);

    Task RemoveRoleAsync(Guid userId, Guid roleId);

    Task<IEnumerable<RoleDto>> GetRolesByUserAsync(Guid userId);

    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId);
}