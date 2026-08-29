using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.API.Pages.Roles;

public class IndexModel : PageModel
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public IndexModel(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUserRoleRepository userRoleRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public List<RoleDto> Roles { get; set; } = new();
    public List<Permission> AllPermissions { get; set; } = new();

    public async Task OnGetAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        AllPermissions = (await _permissionRepository.GetAllAsync()).ToList();

        Roles = new List<RoleDto>();
        foreach (var role in roles)
        {
            var userRoles = await _userRoleRepository.GetByRoleIdAsync(role.Id);
            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(role.Id);

            Roles.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = $"Rôle {role.Name}",
                UserCount = userRoles.Count(),
                PermissionCount = rolePermissions.Count(),
                Permissions = rolePermissions.Select(rp => rp.Permission.Name).ToList()
            });
        }
    }

    public async Task<IActionResult> OnPostSaveRoleAsync(Guid? id, string name, string? description)
    {
        if (id.HasValue)
        {
            // Update existing role
            var role = await _roleRepository.GetByIdAsync(id.Value);
            if (role == null)
                return NotFound();

            role.Name = name;
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Rôle modifié avec succès";
        }
        else
        {
            // Create new role
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            await _roleRepository.AddAsync(role);
            
            TempData["SuccessMessage"] = "Rôle créé avec succès";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdatePermissionsAsync(Guid roleId, List<Guid> permissions)
    {
        // Remove existing permissions
        var existingPermissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId);
        foreach (var rp in existingPermissions)
        {
            _rolePermissionRepository.Delete(rp);
        }
        await _rolePermissionRepository.SaveChangesAsync();

        // Add new permissions
        foreach (var permissionId in permissions)
        {
            var rolePermission = new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId
            };
            await _rolePermissionRepository.AddAsync(rolePermission);
        }

        TempData["SuccessMessage"] = "Permissions mises à jour avec succès";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            return NotFound();

        // Delete role permissions first
        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(id);
        foreach (var rp in rolePermissions)
        {
            _rolePermissionRepository.Delete(rp);
        }

        // Delete user roles
        var userRoles = await _userRoleRepository.GetByRoleIdAsync(id);
        foreach (var ur in userRoles)
        {
            _userRoleRepository.Delete(ur);
        }

        // Delete role
        _roleRepository.Delete(role);
        await _roleRepository.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Rôle supprimé avec succès";
        return RedirectToPage();
    }

    public class RoleDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
