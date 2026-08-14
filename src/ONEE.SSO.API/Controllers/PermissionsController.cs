using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
    {
        return Ok(await _permissionService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> GetById(Guid id)
    {
        var permission = await _permissionService.GetByIdAsync(id);

        if (permission == null)
            return NotFound();

        return Ok(permission);
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetByClient(Guid clientId)
    {
        return Ok(await _permissionService.GetByClientAsync(clientId));
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> Create(CreatePermissionDto dto)
    {
        var permission = await _permissionService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = permission.Id },
            permission);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> Update(Guid id, UpdatePermissionDto dto)
    {
        return Ok(await _permissionService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _permissionService.DeleteAsync(id);

        return NoContent();
    }
}