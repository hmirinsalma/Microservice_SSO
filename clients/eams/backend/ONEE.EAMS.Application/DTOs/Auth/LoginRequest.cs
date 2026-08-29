namespace ONEE.EAMS.Application.DTOs.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, string Role, string Nom, string Prenom, string Email, Guid UserId, Guid? ServiceId, DateTime ExpiresAt);
