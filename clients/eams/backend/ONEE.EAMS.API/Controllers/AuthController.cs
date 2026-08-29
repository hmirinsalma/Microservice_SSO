using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Auth;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Login — retourne un JWT</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>Logout — invalide la session courante</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(User.GetUserId());
        return Ok(ApiResponse<object>.Ok(new { message = "Déconnexion réussie." }));
    }
}
