using System.Security.Claims;

namespace ONEE.SSO.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);
        
    ClaimsPrincipal? ValidateToken(string token);
    
    string? GetJtiFromToken(string token);
}