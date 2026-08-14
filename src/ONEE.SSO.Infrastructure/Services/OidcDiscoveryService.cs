using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.Infrastructure.Services;

public class OidcDiscoveryService : IOidcDiscoveryService
{
    private readonly IConfiguration _configuration;

    public OidcDiscoveryService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public OidcConfigurationDto GetOidcConfiguration(string baseUrl)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? "ONEE.SSO";

        return new OidcConfigurationDto
        {
            Issuer = issuer,
            AuthorizationEndpoint = $"{baseUrl}/connect/authorize",
            TokenEndpoint = $"{baseUrl}/connect/token",
            UserinfoEndpoint = $"{baseUrl}/api/auth/userinfo",
            JwksUri = $"{baseUrl}/.well-known/jwks.json",
            EndSessionEndpoint = $"{baseUrl}/connect/logout",
            ResponseTypesSupported = new[] { "code", "token", "id_token", "code id_token" },
            ScopesSupported = new[] 
            { 
                "openid", 
                "profile", 
                "email", 
                "roles", 
                "offline_access",
                "gestion-personnel",
                "tims",
                "eams"
            },
            GrantTypesSupported = new[] 
            { 
                "authorization_code", 
                "refresh_token", 
                "client_credentials" 
            },
            SubjectTypesSupported = new[] { "public" },
            IdTokenSigningAlgValuesSupported = new[] { "HS256" },
            TokenEndpointAuthMethodsSupported = new[] 
            { 
                "client_secret_basic", 
                "client_secret_post" 
            },
            ClaimsSupported = new[]
            {
                "sub",
                "email",
                "email_verified",
                "name",
                "given_name",
                "family_name",
                "role",
                "permission",
                "iat",
                "exp",
                "jti"
            },
            CodeChallengeMethodsSupported = true
        };
    }

    public JwksDto GetJwks()
    {
        // Pour l'instant, on utilise une clé symétrique (HMAC-SHA256)
        // Dans une vraie implémentation, on utiliserait RSA avec clés publique/privée
        
        var jwtSection = _configuration.GetSection("Jwt");
        var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        
        // Pour HMAC, on ne peut pas exposer la clé secrète publiquement
        // On retourne un JWKS vide pour l'instant
        // TODO: Implémenter RSA pour une vraie clé publique
        
        return new JwksDto
        {
            Keys = new List<JwkDto>()
        };
    }
}