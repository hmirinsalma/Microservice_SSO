using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IRolePermissionRepository : IRepository<RolePermission>
{
    Task<RolePermission?> GetAsync(Guid roleId, Guid permissionId);

    Task<bool> ExistsAsync(Guid roleId, Guid permissionId);

    Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId);

    Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(Guid permissionId);
}