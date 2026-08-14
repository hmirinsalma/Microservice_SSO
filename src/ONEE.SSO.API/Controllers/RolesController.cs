using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetByClient(Guid clientId)
    {
        var roles = await _roleService.GetByClientAsync(clientId);

        return Ok(roles);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create(CreateRoleDto dto)
    {
        var role = await _roleService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = role.Id },
            role);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoleDto>> Update(
        Guid id,
        UpdateRoleDto dto)
    {
        var role = await _roleService.UpdateAsync(id, dto);

        return Ok(role);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteAsync(id);

        return NoContent();
    }
}