using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Auth;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly SsoProvisioningService _provisioningService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly HttpClient _httpClient;

    public AuthController(
        IAuthService authService, 
        SsoProvisioningService provisioningService,
        IConfiguration config,
        ILogger<AuthController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _provisioningService = provisioningService;
        _config = config;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>Login — retourne un JWT (temporaire, sera remplacé par SSO)</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>
    /// 🎯 NOUVEAU: Endpoint de callback SSO avec AUTO-PROVISIONING
    /// Appelé après que l'utilisateur s'est authentifié via le SSO.
    /// Si l'utilisateur n'existe pas dans la base EAMS, il est créé automatiquement!
    /// </summary>
    [HttpPost("sso-callback")]
    [Authorize] // L'utilisateur doit être authentifié par le SSO (bearer token)
    public async Task<IActionResult> SsoCallback()
    {
        try
        {
            _logger.LogInformation("📥 [EAMS] Callback SSO reçu");

            var ssoUser = User;

            if (!ssoUser.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("⚠️ [EAMS] Callback SSO: utilisateur non authentifié");
                return Unauthorized(ApiResponse<LoginResponse>.Fail("Non authentifié par le SSO", 401));
            }

            // 🎯 AUTO-PROVISIONING: Récupère ou crée l'utilisateur
            var user = await _provisioningService.GetOrCreateUserFromSsoAsync(ssoUser);

            // Optionnel: Synchroniser les données du SSO
            await _provisioningService.UpdateUserFromSsoAsync(user, ssoUser);

            // Générer un JWT local pour l'application EAMS
            var token = GenerateLocalToken(user);

            _logger.LogInformation($"✅ [EAMS] Callback SSO traité avec succès pour {user.Email}");

            // 🔔 Envoyer une notification au SSO
            await SendSsoNotificationAsync(user, HttpContext);

            var response = new LoginResponse(
                Token: token,
                Role: user.Role ?? "Technicien",
                Nom: user.Nom,
                Prenom: user.Prenom,
                Email: user.Email,
                UserId: user.Id,
                ServiceId: user.ServiceId,
                ExpiresAt: DateTime.UtcNow.AddHours(8)
            );

            return Ok(ApiResponse<LoginResponse>.Ok(response, 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [EAMS] Erreur lors du callback SSO");
            return StatusCode(500, ApiResponse<LoginResponse>.Fail($"Erreur interne lors du callback SSO: {ex.Message}", 500));
        }
    }

    /// <summary>Logout — invalide la session courante</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(User.GetUserId());
        return Ok(ApiResponse<object>.Ok(new { message = "Déconnexion réussie." }));
    }

    /// <summary>
    /// Génère un JWT local pour l'application EAMS (après authentification SSO).
    /// </summary>
    private string GenerateLocalToken(Domain.Entities.User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}"),
            new Claim(ClaimTypes.Role, user.Role ?? "Technicien"),
            new Claim("sso_id", user.SsoId ?? ""),
            new Claim("eams_user_id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (user.ServiceId.HasValue)
        {
            claims.Add(new Claim("service_id", user.ServiceId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Envoie une notification au SSO lors de la connexion réussie
    /// </summary>
    private async Task SendSsoNotificationAsync(Domain.Entities.User user, HttpContext context)
    {
        try
        {
            var ssoApiUrl = _config["SsoSettings:ApiUrl"] ?? "http://localhost:5205/api";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers.UserAgent.ToString();

            var payload = new
            {
                userId = user.SsoId,
                title = "Connexion réussie à EAMS",
                message = $"Vous vous êtes connecté avec succès à l'application EAMS (Gestion des Équipements).",
                type = "success",
                clientApplicationName = "EAMS",
                ipAddress = ipAddress,
                userAgent = userAgent
            };

            await _httpClient.PostAsJsonAsync($"{ssoApiUrl}/Notifications/create", payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Impossible d'envoyer la notification SSO");
            // Ne pas bloquer le login si la notification échoue
        }
    }
}
