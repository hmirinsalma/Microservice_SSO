using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _service;

    public RolePermissionsController(IRolePermissionService service)
    {
        _service = service;
    }

    [HttpPost("{roleId:guid}/{permissionId:guid}")]
    public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId)
    {
        await _service.AssignPermissionAsync(roleId, permissionId);
        return NoContent();
    }

    [HttpDelete("{roleId:guid}/{permissionId:guid}")]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId)
    {
        await _service.RemovePermissionAsync(roleId, permissionId);
        return NoContent();
    }

    [HttpGet("role/{roleId:guid}")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions(Guid roleId)
    {
        var permissions = await _service.GetPermissionsByRoleAsync(roleId);
        return Ok(permissions);
    }

    [HttpGet("permission/{permissionId:guid}")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles(Guid permissionId)
    {
        var roles = await _service.GetRolesByPermissionAsync(permissionId);
        return Ok(roles);
    }
}