using FluentValidation;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Service;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Infrastructure;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceService              _service;
    private readonly IValidator<CreateServiceDto> _createValidator;
    private readonly IValidator<UpdateServiceDto> _updateValidator;
    private readonly AppDbContext                 _db;

    public ServicesController(
        IServiceService               service,
        IValidator<CreateServiceDto>  createValidator,
        IValidator<UpdateServiceDto>  updateValidator,
        AppDbContext                   db)
    {
        _service         = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _db              = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = ClaimsHelper.GetRole(User);

        if (role == "AdministrateurRH")
            return Ok(await _service.GetAllAsync());

        var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var emp    = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
        if (emp == null) return Ok(await _service.GetAllAsync());

        if (role == "Directeur")
            return Ok(await _service.GetByDirectionAsync(emp.DirectionId));

        var svc = await _service.GetByIdAsync(emp.ServiceId);
        return Ok(new[] { svc });
    }

    [HttpGet("direction/{directionId:int}")]
    public async Task<IActionResult> GetByDirection(int directionId)
    {
        var role = ClaimsHelper.GetRole(User);
        if (role != "AdministrateurRH")
        {
            var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
            var emp    = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp != null && emp.DirectionId != directionId)
                throw new AppException("Accès refusé.", 403);
        }
        return Ok(await _service.GetByDirectionAsync(directionId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = ClaimsHelper.GetRole(User);
        if (role != "AdministrateurRH")
        {
            var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
            var emp    = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp != null && (role == "ChefDeService" || role == "Employe") && emp.ServiceId != id)
                throw new AppException("Accès refusé.", 403);
        }
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
    {
        var v = await _createValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new { message = "Erreur de validation.", errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceDto dto)
    {
        var v = await _updateValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new { message = "Erreur de validation.", errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        return Ok(await _service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
