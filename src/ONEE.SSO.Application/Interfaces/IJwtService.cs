using System.Security.Claims;

namespace ONEE.SSO.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);
    
    string GenerateIdToken(
        Guid userId,
        string email,
        string? fullName,
        string clientId);
        
    ClaimsPrincipal? ValidateToken(string token);
    
    string? GetJtiFromToken(string token);
}