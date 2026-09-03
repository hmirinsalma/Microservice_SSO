using FluentValidation;
using GestionPersonnel.API.DTOs.Auth;
using GestionPersonnel.API.Services.Interfaces;
using GestionPersonnel.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;

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
    private readonly SsoProvisioningService      _provisioningService;
    private readonly IConfiguration              _config;
    private readonly ILogger<AuthController>     _logger;
    private readonly HttpClient                  _httpClient;

    public AuthController(
        IAuthService authService, 
        IValidator<LoginRequestDto> loginValidator,
        SsoProvisioningService provisioningService,
        IConfiguration config,
        ILogger<AuthController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _loginValidator = loginValidator;
        _provisioningService = provisioningService;
        _config = config;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
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
    /// 🎯 NOUVEAU: Endpoint de callback SSO avec AUTO-PROVISIONING
    /// Appelé après que l'utilisateur s'est authentifié via le SSO.
    /// Si l'utilisateur n'existe pas dans la base RH, il est créé automatiquement!
    /// </summary>
    [HttpPost("sso-callback")]
    [Authorize] // L'utilisateur doit être authentifié par le SSO (bearer token)
    public async Task<IActionResult> SsoCallback()
    {
        try
        {
            _logger.LogInformation("📥 Callback SSO reçu");

            // L'utilisateur authentifié par le SSO (via le JWT Bearer)
            var ssoUser = User;

            if (!ssoUser.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("⚠️ Callback SSO: utilisateur non authentifié");
                return Unauthorized(new { message = "Non authentifié par le SSO" });
            }

            // 🎯 AUTO-PROVISIONING: Récupère ou crée l'utilisateur
            var user = await _provisioningService.GetOrCreateUserFromSsoAsync(ssoUser);

            // Optionnel: Synchroniser les données du SSO
            await _provisioningService.UpdateUserFromSsoAsync(user, ssoUser);

            // Générer un JWT local pour l'application RH
            var token = GenerateLocalToken(user);

            _logger.LogInformation($"✅ Callback SSO traité avec succès pour {user.Email}");

            // 🔔 Envoyer une notification au SSO
            await SendSsoNotificationAsync(user, HttpContext);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.Nom,
                ExpiresAt = DateTime.UtcNow.AddHours(8)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors du callback SSO");
            return StatusCode(500, new { message = "Erreur interne lors du callback SSO", details = ex.Message });
        }
    }

    /// <summary>
    /// Déconnexion côté client (suppression du token JWT local).
    /// Avec SSO : appellera le endpoint de révocation du SSO.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
        => Ok(new { message = "Déconnexion réussie." });

    /// <summary>
    /// Génère un JWT local pour l'application RH (après authentification SSO).
    /// </summary>
    private string GenerateLocalToken(Models.User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpirationHours"] ?? "8"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Nom),
            new Claim("sso_id", user.SsoId ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

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
    private async Task SendSsoNotificationAsync(Models.User user, HttpContext context)
    {
        try
        {
            var ssoApiUrl = _config["SsoSettings:ApiUrl"] ?? "http://localhost:5205/api";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers.UserAgent.ToString();

            var payload = new
            {
                userId = user.SsoId,
                title = "Connexion réussie à Gestion RH",
                message = $"Vous vous êtes connecté avec succès à l'application Gestion du Personnel.",
                type = "success",
                clientApplicationName = "Gestion RH",
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

