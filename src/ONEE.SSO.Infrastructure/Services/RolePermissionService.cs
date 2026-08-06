using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RolePermissionService(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task AssignPermissionAsync(Guid roleId, Guid permissionId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        var permission = await _permissionRepository.GetByIdAsync(permissionId);

        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        if (await _rolePermissionRepository.ExistsAsync(roleId, permissionId))
            throw new InvalidOperationException("Permission already assigned to this role.");

        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permissionId
        };

        await _rolePermissionRepository.AddAsync(rolePermission);
        await _rolePermissionRepository.SaveChangesAsync();
    }

    public async Task RemovePermissionAsync(Guid roleId, Guid permissionId)
    {
        var rolePermission = await _rolePermissionRepository.GetAsync(roleId, permissionId);

        if (rolePermission == null)
            throw new KeyNotFoundException("Role permission not found.");

        _rolePermissionRepository.Delete(rolePermission);

        await _rolePermissionRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId)
    {
        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId);

        return rolePermissions.Select(rp => new PermissionDto
        {
            Id = rp.Permission.Id,
            Code = rp.Permission.Code,
            Name = rp.Permission.Name,
            Description = rp.Permission.Description,
            ClientId = rp.Permission.ClientId
        });
    }

    public async Task<IEnumerable<RoleDto>> GetRolesByPermissionAsync(Guid permissionId)
    {
        var rolePermissions = await _rolePermissionRepository.GetByPermissionIdAsync(permissionId);

        return rolePermissions.Select(rp => new RoleDto
        {
            Id = rp.Role.Id,
            Name = rp.Role.Name,
            Description = rp.Role.Description,
            IsSystemRole = rp.Role.IsSystemRole,
            ClientId = rp.Role.ClientId
        });
    }
}