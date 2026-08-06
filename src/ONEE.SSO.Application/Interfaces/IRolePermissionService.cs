using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IRolePermissionService
{
    Task AssignPermissionAsync(Guid roleId, Guid permissionId);

    Task RemovePermissionAsync(Guid roleId, Guid permissionId);

    Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId);

    Task<IEnumerable<RoleDto>> GetRolesByPermissionAsync(Guid permissionId);
}