using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.DTOs.Auth;
using TIMS.API.Extensions;
using TIMS.API.Interfaces;
using TIMS.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;

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
    private readonly SsoProvisioningService _provisioningService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly HttpClient _httpClient;

    public AuthController(
        IAuthService auth, 
        SsoProvisioningService provisioningService,
        IConfiguration config,
        ILogger<AuthController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _auth = auth;
        _provisioningService = provisioningService;
        _config = config;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

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

    /// <summary>
    /// 🎯 NOUVEAU: Endpoint de callback SSO avec AUTO-PROVISIONING
    /// </summary>
    [HttpPost("sso-callback")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> SsoCallback()
    {
        try
        {
            _logger.LogInformation("📥 [TIMS] Callback SSO reçu");

            var ssoUser = User;

            if (!ssoUser.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Fail("Non authentifié par le SSO"));
            }

            // 🎯 AUTO-PROVISIONING
            var user = await _provisioningService.GetOrCreateUserFromSsoAsync(ssoUser);
            await _provisioningService.UpdateUserFromSsoAsync(user, ssoUser);

            // Générer JWT local pour TIMS
            var token = GenerateLocalToken(user);

            _logger.LogInformation($"✅ [TIMS] Callback SSO traité pour {user.Email}");

            // 🔔 Envoyer une notification au SSO
            await SendSsoNotificationAsync(user, HttpContext);

            var response = new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    SsoId = user.SsoId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ServiceId = user.ServiceId,
                    EquipeId = user.EquipeId,
                    Roles = new List<string> { user.RoleMetier ?? "Technicien" }
                }
            };

            return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Authentification SSO réussie"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "❌ [TIMS] Erreur callback SSO - Accès refusé");
            return Unauthorized(ApiResponse<LoginResponseDto>.Fail($"Accès refusé: {ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [TIMS] Erreur callback SSO");
            return StatusCode(500, ApiResponse<LoginResponseDto>.Fail($"Erreur SSO: {ex.Message}"));
        }
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

    /// <summary>
    /// Génère un JWT local pour l'application TIMS.
    /// </summary>
    private string GenerateLocalToken(Entities.User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("sso_id", user.SsoId ?? ""),
            new Claim("tims_user_id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (!string.IsNullOrEmpty(user.RoleMetier))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.RoleMetier));
        }

        if (user.ServiceId.HasValue)
        {
            claims.Add(new Claim("service_id", user.ServiceId.Value.ToString()));
        }

        if (user.EquipeId.HasValue)
        {
            claims.Add(new Claim("team_id", user.EquipeId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Envoie une notification au SSO lors de la connexion réussie
    /// </summary>
    private async Task SendSsoNotificationAsync(Entities.User user, HttpContext context)
    {
        try
        {
            var ssoApiUrl = _config["SsoSettings:ApiUrl"] ?? "http://localhost:5205/api";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers.UserAgent.ToString();

            var payload = new
            {
                userId = user.SsoId,
                title = "Connexion réussie à TIMS",
                message = $"Vous vous êtes connecté avec succès à l'application TIMS (Gestion des Interventions).",
                type = "success",
                clientApplicationName = "TIMS",
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
