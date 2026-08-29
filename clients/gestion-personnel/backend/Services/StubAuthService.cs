using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Auth;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GestionPersonnel.API.Services;

/// <summary>
/// STUB TEMPORAIRE — Remplace le microservice SSO pendant le développement.
///
/// Architecture SSO-Ready :
///   - User.PasswordHash n'existe plus dans le modèle principal
///   - Les credentials sont isolés dans StubCredentials (table séparée)
///   - La table StubCredentials sera supprimée lors de l'intégration SSO
///
/// Migration vers SsoAuthService :
///   Modifier UNIQUEMENT dans Program.cs :
///     builder.Services.AddScoped&lt;IAuthService, StubAuthService&gt;();
///     → builder.Services.AddScoped&lt;IAuthService, SsoAuthService&gt;();
///   + Supprimer la table StubCredentials
///   Aucune autre modification requise.
/// </summary>
public class StubAuthService : IAuthService
{
    private readonly AppDbContext    _db;
    private readonly IConfiguration _config;

    public StubAuthService(AppDbContext db, IConfiguration config)
    {
        _db     = db;
        _config = config;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == dto.Email.ToLower() && u.IsActive)
            ?? throw new UnauthorizedException("Email ou mot de passe incorrect.");

        // Récupérer le credential stub associé
        var cred = await _db.StubCredentials
            .FirstOrDefaultAsync(c => c.UserId == user.Id)
            ?? throw new UnauthorizedException("Email ou mot de passe incorrect.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, cred.PasswordHash))
            throw new UnauthorizedException("Email ou mot de passe incorrect.");

        var token   = GenerateStubToken(user.Id, user.Email, user.Username, user.Role.Nom);
        var expires = DateTime.UtcNow.AddHours(
            double.Parse(_config["Jwt:ExpirationHours"] ?? "8"));

        return new LoginResponseDto
        {
            Token     = token,
            Username  = user.Username,
            Email     = user.Email,
            Role      = user.Role.Nom,
            ExpiresAt = expires,
        };
    }

    private string GenerateStubToken(int userId, string email, string username, string role)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var exp   = DateTime.UtcNow.AddHours(
            double.Parse(_config["Jwt:ExpirationHours"] ?? "8"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Name,               username),
            new Claim(ClaimTypes.Role,               role),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims, expires: exp, signingCredentials: creds));
    }
}
