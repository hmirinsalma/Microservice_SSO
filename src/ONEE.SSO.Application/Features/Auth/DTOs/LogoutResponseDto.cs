namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class LogoutResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int SessionsRevoked { get; set; }
}