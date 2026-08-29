using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var issuer = jwtSection["Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured.");

        var audience = jwtSection["Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured.");

        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        var expirationMinutes = int.TryParse(
            jwtSection["AccessTokenExpirationMinutes"],
            out var minutes)
            ? minutes
            : 15; // 15 minutes par défaut selon le spec

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var jti = Guid.NewGuid().ToString(); // JWT ID unique

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };

        foreach (var role in roles.Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions.Distinct())
        {
            claims.Add(new Claim("permission", permission));
        }

        // Créer le header avec kid (Key ID) pour la validation JWT
        var header = new JwtHeader(credentials);
        header.Add("kid", "onee-sso-key-2024");

        // Créer le payload
        var now = DateTime.UtcNow;
        var payload = new JwtPayload(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expirationMinutes));

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var issuer = jwtSection["Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");

            var audience = jwtSection["Audience"]
                ?? throw new InvalidOperationException("JWT Audience is not configured.");

            var secretKey = jwtSection["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateIdToken(
        Guid userId,
        string email,
        string? fullName,
        string clientId)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var issuer = jwtSection["Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured.");

        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        var expirationMinutes = int.TryParse(
            jwtSection["AccessTokenExpirationMinutes"],
            out var minutes)
            ? minutes
            : 15;

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var jti = Guid.NewGuid().ToString();

        // Claims OIDC standard pour id_token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Aud, clientId), // Audience = client_id
            new("email_verified", "true")
        };

        // Ajouter le nom si disponible
        if (!string.IsNullOrEmpty(fullName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, fullName));
        }

        // Créer le header avec kid (Key ID) pour la validation JWT
        var header = new JwtHeader(credentials);
        header.Add("kid", "onee-sso-key-2024");

        // Créer le payload
        var payload = new JwtPayload(
            issuer: issuer,
            audience: clientId, // Pour id_token, audience = client_id
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expirationMinutes));

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public string? GetJtiFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
        catch
        {
            return null;
        }
    }
}
