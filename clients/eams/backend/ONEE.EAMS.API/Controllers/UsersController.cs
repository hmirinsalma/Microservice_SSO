using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.User;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

/// <summary>
/// Gestion des utilisateurs métier EAMS — Admin uniquement.
///
/// Périmètre :
///   - CRUD des profils utilisateurs métier (nom, rôle, service, téléphone)
///   - Activation/Désactivation
///
/// Hors périmètre (délégué au microservice SSO) :
///   - Création de comptes d'authentification
///   - Gestion des mots de passe
///   - Gestion des rôles d'authentification
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin_Patrimoine")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    /// <summary>
    /// Crée un profil utilisateur métier EAMS.
    /// Aucun compte d'authentification n'est créé ici.
    /// Le compte SSO doit être créé côté microservice SSO avant ou après.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<UserDto>.Ok(result, 201));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        await _service.ToggleActiveAsync(id);
        return Ok(ApiResponse<object>.Ok(new { message = "Statut utilisateur modifié." }));
    }
}
