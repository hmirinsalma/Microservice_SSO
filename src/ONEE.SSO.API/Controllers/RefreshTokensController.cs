using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshTokensController : ControllerBase
{
    private readonly IRefreshTokenService _service;

    public RefreshTokensController(IRefreshTokenService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RefreshTokenDto>>> GetAll()
    {
        var tokens = await _service.GetAllAsync();
        return Ok(tokens);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RefreshTokenDto>> GetById(Guid id)
    {
        var token = await _service.GetByIdAsync(id);

        if (token == null)
            return NotFound();

        return Ok(token);
    }

    [HttpPut("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        await _service.RevokeAsync(id);
        return NoContent();
    }
}