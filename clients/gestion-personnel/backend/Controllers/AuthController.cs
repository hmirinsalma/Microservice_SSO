using FluentValidation;
using GestionPersonnel.API.DTOs.Auth;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPersonnel.API.Controllers;

/// <summary>
/// Controller d'authentification SSO-Ready.
/// Dépend UNIQUEMENT de IAuthService (interface).
/// Actuellement servi par StubAuthService.
/// Lors de l'intégration SSO : remplacer StubAuthService par SsoAuthService dans Program.cs.
/// Ce Controller ne changera pas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService                _authService;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    public AuthController(IAuthService authService, IValidator<LoginRequestDto> loginValidator)
    {
        _authService    = authService;
        _loginValidator = loginValidator;
    }

    /// <summary>
    /// Stub temporaire — sera remplacé par une redirection SSO (OAuth2/OIDC).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var v = await _loginValidator.ValidateAsync(dto);
        if (!v.IsValid)
            return BadRequest(new { message = "Erreur de validation.", errors = v.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        return Ok(await _authService.LoginAsync(dto));
    }

    /// <summary>
    /// Déconnexion côté client (suppression du token JWT local).
    /// Avec SSO : appellera le endpoint de révocation du SSO.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
        => Ok(new { message = "Déconnexion réussie." });
}
