using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientApplicationsController : ControllerBase
{
    private readonly IClientApplicationService _clientService;

    public ClientApplicationsController(IClientApplicationService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientApplicationDto>>> GetAll()
    {
        var clients = await _clientService.GetAllAsync();
        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientApplicationDto>> GetById(Guid id)
    {
        var client = await _clientService.GetByIdAsync(id);

        if (client == null)
            return NotFound();

        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientApplicationDto>> Create(CreateClientApplicationDto dto)
    {
        var client = await _clientService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = client.Id },
            client);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientApplicationDto>> Update(
        Guid id,
        UpdateClientApplicationDto dto)
    {
        var client = await _clientService.UpdateAsync(id, dto);

        return Ok(client);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _clientService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ClientApplicationDto>>> Search(string keyword)
    {
        var clients = await _clientService.SearchAsync(keyword);

        return Ok(clients);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<ClientApplicationDto>>> GetPaged(
        int page = 1,
        int pageSize = 10)
    {
        var clients = await _clientService.GetPagedAsync(page, pageSize);

        return Ok(clients);
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _clientService.ActivateAsync(id);

        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _clientService.DeactivateAsync(id);

        return NoContent();
    }
}