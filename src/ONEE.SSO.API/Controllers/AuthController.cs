using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Features.Auth.Handlers;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginCommandHandler _loginCommandHandler;
    private readonly LogoutCommandHandler _logoutCommandHandler;
    private readonly ValidateTokenCommandHandler _validateTokenCommandHandler;
    private readonly RefreshTokenCommandHandler _refreshTokenCommandHandler;

    public AuthController(
        LoginCommandHandler loginCommandHandler, 
        LogoutCommandHandler logoutCommandHandler,
        ValidateTokenCommandHandler validateTokenCommandHandler,
        RefreshTokenCommandHandler refreshTokenCommandHandler)
    {
        _loginCommandHandler = loginCommandHandler;
        _logoutCommandHandler = logoutCommandHandler;
        _validateTokenCommandHandler = validateTokenCommandHandler;
        _refreshTokenCommandHandler = refreshTokenCommandHandler;
    }

    /// <summary>
    /// Authentifie un utilisateur avec email et mot de passe
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        var result = await _loginCommandHandler.HandleAsync(command);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Email ou mot de passe incorrect."
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Déconnecte l'utilisateur en révoquant son refresh token et invalidant sa session
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        var command = new LogoutCommand
        {
            RefreshToken = request.RefreshToken,
            LogoutAllDevices = request.LogoutAllDevices,
            IpAddress = ipAddress
        };

        var result = await _logoutCommandHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Valide un token JWT et retourne les informations de l'utilisateur
    /// </summary>
    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequestDto request)
    {
        var command = new ValidateTokenCommand
        {
            Token = request.Token
        };

        var result = await _validateTokenCommandHandler.HandleAsync(command);

        if (!result.IsValid)
        {
            return Unauthorized(new
            {
                message = result.Reason
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Renouvelle un access token en utilisant un refresh token valide
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken,
            IpAddress = ipAddress
        };

        var result = await _refreshTokenCommandHandler.HandleAsync(command);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Refresh token invalide ou expiré."
            });
        }

        return Ok(result);
    }
}