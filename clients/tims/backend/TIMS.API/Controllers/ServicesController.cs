using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.DTOs.User;
using TIMS.API.Entities;
using TIMS.API.Interfaces;

namespace TIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceEquipeService _svc;
    public ServicesController(IServiceEquipeService svc) { _svc = svc; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ServiceDto>>>> GetAll()
        => Ok(ApiResponse<List<ServiceDto>>.Ok(await _svc.GetAllServicesAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> GetById(int id)
        => Ok(ApiResponse<ServiceDto>.Ok(await _svc.GetServiceByIdAsync(id)));

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> Create([FromBody] CreateServiceDto dto)
    {
        var result = await _svc.CreateServiceAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ServiceDto>.Ok(result, "Service créé"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> Update(int id, [FromBody] CreateServiceDto dto)
        => Ok(ApiResponse<ServiceDto>.Ok(await _svc.UpdateServiceAsync(id, dto), "Service mis à jour"));
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipesController : ControllerBase
{
    private readonly IServiceEquipeService _svc;
    public EquipesController(IServiceEquipeService svc) { _svc = svc; }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<EquipeDto>>>> GetAll()
        => Ok(ApiResponse<List<EquipeDto>>.Ok(await _svc.GetAllEquipesAsync()));

    [HttpGet("service/{serviceId:int}")]
    public async Task<ActionResult<ApiResponse<List<EquipeDto>>>> GetByService(int serviceId)
        => Ok(ApiResponse<List<EquipeDto>>.Ok(await _svc.GetEquipesByServiceAsync(serviceId)));

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<EquipeDto>>> Create([FromBody] CreateEquipeDto dto)
    {
        var result = await _svc.CreateEquipeAsync(dto);
        return Ok(ApiResponse<EquipeDto>.Ok(result, "Équipe créée"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<EquipeDto>>> Update(int id, [FromBody] CreateEquipeDto dto)
        => Ok(ApiResponse<EquipeDto>.Ok(await _svc.UpdateEquipeAsync(id, dto), "Équipe mise à jour"));
}
