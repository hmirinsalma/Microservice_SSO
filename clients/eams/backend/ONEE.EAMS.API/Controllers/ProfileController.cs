using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.User;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

/// <summary>
/// Profil utilisateur — informations métier uniquement.
/// La gestion du mot de passe est déléguée au microservice SSO.
/// </summary>
[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _service;

    public ProfileController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _service.GetProfileAsync(User);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    /// <summary>
    /// Met à jour le téléphone et la photo uniquement.
    /// Le mot de passe est géré par le microservice SSO.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var result = await _service.UpdateProfileAsync(request, User);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    // NOTE SSO : L'endpoint PATCH /profile/password a été supprimé.
    // Le changement de mot de passe sera accessible via l'interface du microservice SSO.
}
