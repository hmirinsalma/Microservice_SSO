using FluentValidation;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Employe;
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
public class EmployesController : ControllerBase
{
    private readonly IEmployeService             _service;
    private readonly IValidator<CreateEmployeDto> _createValidator;
    private readonly IValidator<UpdateEmployeDto> _updateValidator;
    private readonly AppDbContext                 _db;

    public EmployesController(
        IEmployeService              service,
        IValidator<CreateEmployeDto> createValidator,
        IValidator<UpdateEmployeDto> updateValidator,
        AppDbContext                  db)
    {
        _service         = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _db              = db;
    }

    // ── GET liste — filtré selon le rôle du claim JWT ────────
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] EmployeQueryDto query)
    {
        var role   = ClaimsHelper.GetRole(User);
        var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);

        switch (role)
        {
            case "AdministrateurRH":
                return Ok(await _service.GetPagedAsync(query));

            case "Directeur":
            {
                var emp = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
                if (emp == null) return Ok(new { data = Array.Empty<object>(), totalCount = 0 });
                query.DirectionId = emp.DirectionId;
                return Ok(await _service.GetPagedAsync(query));
            }

            case "ChefDeService":
            {
                var emp = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
                if (emp == null) return Ok(new { data = Array.Empty<object>(), totalCount = 0 });
                query.ServiceId = emp.ServiceId;
                return Ok(await _service.GetPagedAsync(query));
            }

            case "Employe":
            {
                var emp = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
                if (emp == null) return Ok(new { data = Array.Empty<object>(), totalCount = 0 });
                var detail = await _service.GetByIdAsync(emp.Id);
                return Ok(new { data = new[] { detail }, totalCount = 1, page = 1, totalPages = 1 });
            }

            default: return Forbid();
        }
    }

    // ── GET/:id ──────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role   = ClaimsHelper.GetRole(User);
        var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);

        if (role == "AdministrateurRH")
            return Ok(await _service.GetByIdAsync(id));

        var emp = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);

        if (role == "Directeur" && emp != null)
        {
            var target = await _db.Employes.FindAsync(id);
            if (target?.DirectionId != emp.DirectionId)
                throw new AppException("Accès refusé : cet employé n'appartient pas à votre direction.", 403);
        }
        else if (role == "ChefDeService" && emp != null)
        {
            var target = await _db.Employes.FindAsync(id);
            if (target?.ServiceId != emp.ServiceId)
                throw new AppException("Accès refusé : cet employé n'appartient pas à votre service.", 403);
        }
        else if (role == "Employe" && emp?.Id != id)
        {
            throw new AppException("Vous ne pouvez consulter que votre propre fiche.", 403);
        }

        return Ok(await _service.GetByIdAsync(id));
    }

    // ── POST — Admin RH uniquement ───────────────────────────
    [HttpPost]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeDto dto)
    {
        var v = await _createValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new { message = "Erreur de validation.", errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ── PUT — Admin RH uniquement ────────────────────────────
    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeDto dto)
    {
        var v = await _updateValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new { message = "Erreur de validation.", errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });

        return Ok(await _service.UpdateAsync(id, dto));
    }

    // ── PATCH profil ─────────────────────────────────────────
    [HttpPatch("{id:int}/profil")]
    public async Task<IActionResult> UpdateProfil(int id, [FromBody] UpdateProfilDto dto)
    {
        var role   = ClaimsHelper.GetRole(User);
        var userId = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var emp    = await _db.Employes.FindAsync(id)
            ?? throw new NotFoundException("Employé", id);

        if (role != "AdministrateurRH" && emp.UserId != userId)
            throw new AppException("Vous ne pouvez modifier que votre propre profil.", 403);

        if (dto.Telephone != null) emp.Telephone = dto.Telephone;
        if (dto.Adresse   != null) emp.Adresse   = dto.Adresse;
        if (dto.PhotoUrl  != null) emp.PhotoUrl  = dto.PhotoUrl;
        emp.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(await _service.GetByIdAsync(id));
    }

    // ── DELETE — Admin RH uniquement ─────────────────────────
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdministrateurRH")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
