using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Persistence;

namespace ONEE.SSO.Infrastructure.Repositories;

public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<RolePermission?> GetAsync(Guid roleId, Guid permissionId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .FirstOrDefaultAsync(rp =>
                rp.RoleId == roleId &&
                rp.PermissionId == permissionId);
    }

    public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId)
    {
        return await _context.RolePermissions.AnyAsync(rp =>
            rp.RoleId == roleId &&
            rp.PermissionId == permissionId);
    }

    public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(Guid permissionId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .Where(rp => rp.PermissionId == permissionId)
            .ToListAsync();
    }
}