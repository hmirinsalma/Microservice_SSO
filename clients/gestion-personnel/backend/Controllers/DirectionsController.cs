using FluentValidation;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Direction;
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
public class DirectionsController : ControllerBase
{
    private readonly IDirectionService              _service;
    private readonly IValidator<CreateDirectionDto> _createValidator;
    private readonly IValidator<UpdateDirectionDto> _updateValidator;
    private readonly AppDbContext                   _db;

    public DirectionsController(
        IDirectionService               service,
        IValidator<CreateDirectionDto>  createValidator,
        IValidator<UpdateDirectionDto>  updateValidator,
        AppDbContext                    db)
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

        var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var emp     = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
        if (emp == null) return Ok(await _service.GetAllAsync());

        var direction = await _service.GetByIdAsync(emp.DirectionId);
        return Ok(new[] { direction });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = ClaimsHelper.GetRole(User);
        if (role != "AdministrateurRH")
        {
            var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
            var emp    = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp != null && emp.DirectionId != id)
                throw new AppException("Accès refusé : vous ne pouvez consulter que votre direction.", 403);
        }
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Create([FromBody] CreateDirectionDto dto)
    {
        var v = await _createValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new { message = "Erreur de validation.", errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        return CreatedAtAction(nameof(GetById), new { id = (await _service.CreateAsync(dto)).Id }, await _service.CreateAsync(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDirectionDto dto)
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
