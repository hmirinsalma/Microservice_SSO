namespace ONEE.SSO.Application.Features.Auth.Commands;

public class LogoutCommand
{
    public string? RefreshToken { get; set; }
    public bool LogoutAllDevices { get; set; } = false;
    public string? IpAddress { get; set; }
}