using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.Auth;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

/// <summary>
/// ⚠️  STUB TEMPORAIRE — À SUPPRIMER lors de l'intégration SSO
///
/// Ce service simule le futur microservice SSO en générant un JWT local.
/// Il est l'UNIQUE composant qui :
///   - accède à StubCredentials
///   - utilise BCrypt
///   - génère des JWT
///
/// Aucun autre service, repository ou controller métier ne dépend de lui.
///
/// PLAN DE MIGRATION SSO :
///   1. Créer SsoAuthService implémentant IAuthService
///   2. Dans Program.cs : remplacer StubAuthService → SsoAuthService
///   3. Supprimer ce fichier
///   4. Supprimer la table StubCredentials
///   5. Configurer JWT/OIDC avec la clé publique du SSO
/// </summary>
public class StubAuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration       _cfg;
    private readonly ILogger<StubAuthService> _log;

    public StubAuthService(ApplicationDbContext db, IConfiguration cfg, ILogger<StubAuthService> log)
    { _db = db; _cfg = cfg; _log = log; }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        // Cherche dans StubCredentials (table temporaire)
        var cred = await _db.StubCredentials
            .Include(c => c.User)
                .ThenInclude(u => u.Service)
            .Include(c => c.User)
                .ThenInclude(u => u.Equipe)
            .Include(c => c.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(c => c.Email == dto.Email.ToLower());

        if (cred == null || !BCrypt.Net.BCrypt.Verify(dto.Password, cred.PasswordHash))
        {
            _log.LogWarning("[STUB] Tentative de connexion échouée : {Email}", dto.Email);
            throw new AppException("Identifiants invalides", 401, "INVALID_CREDENTIALS");
        }

        if (!cred.User.IsActive)
            throw new AppException("Compte désactivé", 401, "ACCOUNT_DISABLED");

        var user  = cred.User;
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Génère un JWT avec les claims qu'enverra le vrai SSO
        var token   = GenerateStubJwt(user, roles);
        var expires = DateTime.UtcNow.AddMinutes(60);

        _log.LogInformation("[STUB] Connexion réussie : {Email} [{Roles}]", dto.Email, string.Join(",", roles));

        return new LoginResponseDto
        {
            Token     = token,
            ExpiresAt = expires,
            User = new UserInfoDto
            {
                Id              = user.Id,
                SsoId           = user.SsoId ?? $"stub-{user.Id}",
                FirstName       = user.FirstName,
                LastName        = user.LastName,
                Email           = user.Email,
                ProfilePhotoPath= user.ProfilePhotoPath,
                Roles           = roles,
                ServiceName     = user.Service?.Name,
                ServiceId       = user.ServiceId,
                EquipeName      = user.Equipe?.Name,
                EquipeId        = user.EquipeId,
            }
        };
    }

    public Task LogoutAsync(int timsUserId)
    {
        // JWT stateless — le client supprime le token.
        // Future implémentation SSO : appeler l'endpoint /logout du SSO.
        _log.LogInformation("[STUB] Déconnexion userId={UserId}", timsUserId);
        return Task.CompletedTask;
    }

    // ── JWT Stub ─────────────────────────────────────────────────────────────

    private string GenerateStubJwt(Entities.User user, List<string> roles)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            // Claims standards OIDC (compatibles avec le futur SSO)
            new(JwtRegisteredClaimNames.Sub,   user.SsoId ?? $"stub-{user.Id}"),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName,  user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),

            // Claims TIMS spécifiques
            new("tims_user_id", user.Id.ToString()),
            new("serviceId",    (user.ServiceId ?? 0).ToString()),
            new("teamId",       (user.EquipeId  ?? 0).ToString()),

            // Claim standard ASP.NET pour [Authorize(Roles=...)]
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        // Rôles (compatibles [Authorize(Roles=...)])
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:   _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims:   claims,
            expires:  DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
