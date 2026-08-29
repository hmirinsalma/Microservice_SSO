using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.DTOs.Auth;
using TIMS.API.Extensions;
using TIMS.API.Interfaces;

namespace TIMS.API.Controllers;

/// <summary>
/// Controller d'authentification.
/// Dépend uniquement de IAuthService (jamais de StubAuthService directement).
///
/// TODO SSO : Lors de l'intégration, ce controller sera remplacé par
/// une redirection OIDC vers le microservice SSO.
/// Le login local sera supprimé.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) { _auth = auth; }

    /// <summary>
    /// ⚠️ TEMPORAIRE — Login local via StubAuthService.
    /// Sera remplacé par une redirection OIDC vers le SSO.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Connexion réussie"));
    }

    /// <summary>Déconnexion. Le client supprime le JWT.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        await _auth.LogoutAsync(ClaimsHelper.GetTimsUserId(User));
        return Ok(ApiResponse<object>.Ok(null!, "Déconnexion réussie"));
    }

    /// <summary>Informations de l'utilisateur connecté depuis le JWT.</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(ApiResponse<object>.Ok(new
    {
        timsUserId = ClaimsHelper.GetTimsUserId(User),
        ssoId      = ClaimsHelper.GetSsoId(User),
        role       = ClaimsHelper.GetRole(User),
        serviceId  = ClaimsHelper.GetServiceId(User),
        teamId     = ClaimsHelper.GetTeamId(User),
        email      = ClaimsHelper.GetEmail(User),
    }));
}
