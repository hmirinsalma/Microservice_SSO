namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class LogoutRequestDto
{
    public string? RefreshToken { get; set; }
    public bool LogoutAllDevices { get; set; } = false;
}