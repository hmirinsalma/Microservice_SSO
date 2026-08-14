using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _service;

    public UserRolesController(IUserRoleService service)
    {
        _service = service;
    }

    [HttpPost("{userId:guid}/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
    {
        await _service.AssignRoleAsync(userId, roleId);

        return NoContent();
    }

    [HttpDelete("{userId:guid}/{roleId:guid}")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
    {
        await _service.RemoveRoleAsync(userId, roleId);

        return NoContent();
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRolesByUser(Guid userId)
    {
        var roles = await _service.GetRolesByUserAsync(userId);

        return Ok(roles);
    }

    [HttpGet("role/{roleId:guid}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(Guid roleId)
    {
        var users = await _service.GetUsersByRoleAsync(roleId);

        return Ok(users);
    }
}