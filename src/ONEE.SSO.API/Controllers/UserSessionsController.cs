using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserSessionsController : ControllerBase
{
    private readonly IUserSessionService _service;

    public UserSessionsController(IUserSessionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSessionDto>>> GetAll()
    {
        var sessions = await _service.GetAllAsync();
        return Ok(sessions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserSessionDto>> GetById(Guid id)
    {
        var session = await _service.GetByIdAsync(id);

        if (session == null)
            return NotFound();

        return Ok(session);
    }

    [HttpPut("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        await _service.RevokeAsync(id);
        return NoContent();
    }
}