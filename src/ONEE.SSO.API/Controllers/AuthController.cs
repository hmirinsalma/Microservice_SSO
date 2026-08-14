using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Features.Auth.Handlers;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginCommandHandler _loginCommandHandler;
    private readonly LogoutCommandHandler _logoutCommandHandler;
    private readonly ValidateTokenCommandHandler _validateTokenCommandHandler;
    private readonly RefreshTokenCommandHandler _refreshTokenCommandHandler;
    private readonly ForgotPasswordCommandHandler _forgotPasswordCommandHandler;
    private readonly ResetPasswordCommandHandler _resetPasswordCommandHandler;
    private readonly ChangePasswordCommandHandler _changePasswordCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public AuthController(
        LoginCommandHandler loginCommandHandler, 
        LogoutCommandHandler logoutCommandHandler,
        ValidateTokenCommandHandler validateTokenCommandHandler,
        RefreshTokenCommandHandler refreshTokenCommandHandler,
        ForgotPasswordCommandHandler forgotPasswordCommandHandler,
        ResetPasswordCommandHandler resetPasswordCommandHandler,
        ChangePasswordCommandHandler changePasswordCommandHandler,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository)
    {
        _loginCommandHandler = loginCommandHandler;
        _logoutCommandHandler = logoutCommandHandler;
        _validateTokenCommandHandler = validateTokenCommandHandler;
        _refreshTokenCommandHandler = refreshTokenCommandHandler;
        _forgotPasswordCommandHandler = forgotPasswordCommandHandler;
        _resetPasswordCommandHandler = resetPasswordCommandHandler;
        _changePasswordCommandHandler = changePasswordCommandHandler;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
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

    /// <summary>
    /// Endpoint OIDC userinfo - Retourne les informations de l'utilisateur authentifié
    /// </summary>
    [HttpGet("userinfo")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token invalide" });
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Utilisateur introuvable ou inactif" });
        }

        // Récupérer les rôles
        var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
        var roles = userRoles.Select(ur => ur.Role.Name).Distinct().ToList();

        // Récupérer les permissions
        var permissions = new List<string>();
        foreach (var userRole in userRoles)
        {
            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(userRole.RoleId);
            permissions.AddRange(rolePermissions.Select(rp => rp.Permission.Code));
        }
        permissions = permissions.Distinct().ToList();

        var response = new UserinfoResponseDto
        {
            Sub = userId.ToString(),
            Email = user.Email,
            EmailVerified = true, // TODO: Implémenter la vérification email
            Name = $"{user.FirstName} {user.LastName}",
            GivenName = user.FirstName,
            FamilyName = user.LastName,
            Roles = roles,
            Permissions = permissions
        };

        return Ok(response);
    }

    /// <summary>
    /// Demande de réinitialisation de mot de passe
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new ForgotPasswordCommand
        {
            Email = request.Email,
            IpAddress = ipAddress
        };

        var result = await _forgotPasswordCommandHandler.HandleAsync(command);
        return Ok(result);
    }

    /// <summary>
    /// Réinitialisation du mot de passe avec un token valide
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new ResetPasswordCommand
        {
            Token = request.Token,
            NewPassword = request.NewPassword,
            IpAddress = ipAddress
        };

        var result = await _resetPasswordCommandHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Changement de mot de passe pour un utilisateur authentifié
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token invalide" });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var accessToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        var command = new ChangePasswordCommand
        {
            UserId = userId,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            IpAddress = ipAddress,
            AccessToken = accessToken
        };

        var result = await _changePasswordCommandHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}