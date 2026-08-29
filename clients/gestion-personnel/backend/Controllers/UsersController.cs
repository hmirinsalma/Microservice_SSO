using FluentValidation;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.User;
using GestionPersonnel.API.Infrastructure;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Controllers;

/// <summary>
/// Gestion des comptes utilisateurs.
/// SSO-Ready :
///   - Dépend de IUserManagementService (interface métier, sans auth)
///   - Dépend de IStubCredentialService (stub temporaire, supprimé avec SSO)
///   - Lors du SSO : supprimer IStubCredentialService des injections et des appels
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "AdministrateurRH")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService    _service;
    private readonly IStubCredentialService    _stubCred;    // TEMPORAIRE — supprimé avec SSO
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;
    private readonly AppDbContext              _db;

    public UsersController(
        IUserManagementService    service,
        IStubCredentialService    stubCred,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator,
        AppDbContext               db)
    {
        _service         = service;
        _stubCred        = stubCred;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _db              = db;
    }

    // ── Employés sans compte ─────────────────────────────────
    [HttpGet("employes-sans-compte")]
    public async Task<IActionResult> GetEmployesSansCompte()
    {
        var list = await _db.Employes
            .Include(e => e.Direction)
            .Where(e => e.UserId == null)
            .OrderBy(e => e.Nom)
            .Select(e => new
            {
                e.Id, e.Nom, e.Prenom, e.Matricule, e.Email, e.Poste,
                DirectionNom = e.Direction.Nom
            })
            .ToListAsync();
        return Ok(list);
    }

    // ── Liste / Rôles / Détail ───────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
        => Ok(await _service.GetAllRolesAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    // ── Créer ────────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var v = await _createValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new
            {
                message = "Erreur de validation.",
                errors  = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });

        // 1. Créer le compte (service métier pur — aucun password)
        var created = await _service.CreateAsync(dto);

        // 2. Créer le credential stub si un mot de passe est fourni
        // TEMPORAIRE — Supprimé lors de l'intégration SSO
        if (!string.IsNullOrWhiteSpace(dto.Password))
            await _stubCred.CreateAsync(created.Id, dto.Password);

        // 3. Lier la fiche employé si sélectionnée
        if (dto.EmployeId.HasValue)
        {
            var emp = await _db.Employes.FindAsync(dto.EmployeId.Value);
            if (emp != null)
            {
                emp.UserId = created.Id;
                await _db.SaveChangesAsync();
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ── Modifier ─────────────────────────────────────────────
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var v = await _updateValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new
            {
                message = "Erreur de validation.",
                errors  = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        return Ok(await _service.UpdateAsync(id, dto));
    }

    // ── Activer / Désactiver ─────────────────────────────────
    [HttpPatch("{id:int}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        await _service.ToggleActiveAsync(id);
        return Ok(new { message = "Statut modifié." });
    }

    // ── Supprimer ────────────────────────────────────────────
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Supprimer le credential stub avant le compte
        // TEMPORAIRE — Supprimé lors de l'intégration SSO
        await _stubCred.DeleteAsync(id);
        await _service.DeleteAsync(id);
        return NoContent();
    }

    // ──────────────────────────────────────────────────────────
    // NOTE : PATCH /password supprimé — géré par le SSO.
    // ──────────────────────────────────────────────────────────
}
