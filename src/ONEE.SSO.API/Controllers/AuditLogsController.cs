using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _service;

    public AuditLogsController(IAuditLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
    {
        var logs = await _service.GetAllAsync();
        return Ok(logs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuditLogDto>> GetById(Guid id)
    {
        var log = await _service.GetByIdAsync(id);

        if (log == null)
            return NotFound();

        return Ok(log);
    }
}