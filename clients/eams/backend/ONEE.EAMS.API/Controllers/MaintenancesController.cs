using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Maintenance;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/maintenances")]
[Authorize]
public class MaintenancesController : ControllerBase
{
    private readonly IMaintenanceService _service;

    public MaintenancesController(IMaintenanceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] MaintenanceFilterRequest filter)
    {
        var result = await _service.GetAllAsync(filter, User);
        return Ok(ApiResponse<PagedResult<MaintenanceListDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id, User);
        return Ok(ApiResponse<MaintenanceDetailDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin_Patrimoine,Chef_de_Service")]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequest request)
    {
        var result = await _service.CreateAsync(request, User);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<MaintenanceDetailDto>.Ok(result, 201));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin_Patrimoine,Chef_de_Service,Technicien")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaintenanceRequest request)
    {
        var result = await _service.UpdateAsync(id, request, User);
        return Ok(ApiResponse<MaintenanceDetailDto>.Ok(result));
    }

    [HttpPatch("{id:guid}/cloturer")]
    [Authorize(Roles = "Admin_Patrimoine,Technicien")]
    public async Task<IActionResult> Cloturer(Guid id, [FromBody] CloturerMaintenanceRequest request)
    {
        var result = await _service.CloturerAsync(id, request, User);
        return Ok(ApiResponse<MaintenanceDetailDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, User);
        return Ok(ApiResponse<object>.Ok(new { message = "Maintenance supprimée." }));
    }
}
