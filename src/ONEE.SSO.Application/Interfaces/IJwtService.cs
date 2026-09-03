using System.Security.Claims;

namespace ONEE.SSO.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        string? firstName,
        string? lastName,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);
    
    string GenerateIdToken(
        Guid userId,
        string email,
        string? firstName,
        string? lastName,
        string clientId,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? permissions = null);
        
    ClaimsPrincipal? ValidateToken(string token);
    
    string? GetJtiFromToken(string token);
}