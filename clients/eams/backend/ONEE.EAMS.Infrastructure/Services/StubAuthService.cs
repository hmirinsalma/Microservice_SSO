using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Auth;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Infrastructure.Data;

namespace ONEE.EAMS.Infrastructure.Services;

/// <summary>
/// Implémentation STUB de IAuthService — utilisée pendant la phase de développement
/// et de démonstration, EN ATTENTE du microservice SSO.
///
/// ─────────────────────────────────────────────────────────────────────────────
/// IMPORTANT — Architecture SSO
/// ─────────────────────────────────────────────────────────────────────────────
/// Cette classe est temporaire et sera supprimée lors de l'intégration SSO.
/// Elle est totalement isolée : Controllers, Services et Repositories ne la
/// connaissent pas — ils dépendent uniquement de IAuthService.
///
/// Structure des claims — IDENTIQUE à ce que le futur SSO devra produire :
///
///   Claim standard  | "sub"             → SsoId de l'utilisateur (ex: SSO subject)
///   Claim custom    | "eams_user_id"    → Id EAMS interne (Guid) pour les requêtes métier
///   Claim standard  | email             → Email
///   Claim standard  | role              → Rôle métier (Admin_Patrimoine, Technicien...)
///   Claim custom    | "serviceId"       → Guid du service EAMS
///   Claim custom    | "nom"             → Nom (équivaut à family_name OIDC)
///   Claim custom    | "prenom"          → Prénom (équivaut à given_name OIDC)
///
/// Le SSO devra produire exactement ces claims pour que EAMS fonctionne sans
/// aucune modification dans la couche Application.
///
/// Migration :
///   InfrastructureExtensions.cs → remplacer StubAuthService par SsoAuthService
///   Aucune autre modification nécessaire.
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class StubAuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public StubAuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config  = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Phase STUB : identification par email uniquement (pas de mot de passe)
        var user = await _context.Users
            .Include(u => u.Service)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user is null)
            throw new UnauthorizedException("Identifiant incorrect ou compte inactif.");

        var jwtKey      = _config["Jwt:Key"]        ?? throw new InvalidOperationException("JWT key not configured.");
        var issuer      = _config["Jwt:Issuer"]     ?? "ONEE-EAMS";
        var audience    = _config["Jwt:Audience"]   ?? "ONEE-EAMS-Client";
        var expiryHours = int.Parse(_config["Jwt:ExpiryHours"] ?? "8");

        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(expiryHours);

        // ── Claims — structure cible à reproduire côté SSO ──────────────────────
        // "sub"          → identifiant SSO (SsoId). En phase stub = User.Id (temporaire)
        // "eams_user_id" → User.Id EAMS interne — utilisé par ClaimsHelper.GetUserId()
        //                  Le SSO devra inclure ce claim dans le JWT qu'il émettra.
        var ssoSubject = user.SsoId ?? user.Id.ToString(); // fallback stub

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   ssoSubject),
            new("eams_user_id",                user.Id.ToString()),
            new(ClaimTypes.Email,              user.Email),
            new(ClaimTypes.Role,               user.Role ?? "Technicien"),
            new("serviceId",                   user.ServiceId?.ToString() ?? ""),
            new("nom",                         user.Nom),
            new("prenom",                      user.Prenom),
            new("ssoId",                       user.SsoId ?? ""),
        };

        var token       = new JwtSecurityToken(issuer, audience, claims, expires: expiry, signingCredentials: creds);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(
            tokenString, user.Role ?? "Technicien",
            user.Nom, user.Prenom, user.Email,
            user.Id, user.ServiceId, expiry);
    }

    public Task LogoutAsync(Guid userId)
    {
        // Phase STUB : JWT stateless — le client supprime le token côté navigateur.
        // Phase SSO  : POST {SSO_BASE_URL}/auth/logout pour révoquer la session.
        return Task.CompletedTask;
    }
}
