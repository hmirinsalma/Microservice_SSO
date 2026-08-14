namespace ONEE.SSO.Application.Features.Auth.Commands;

public class RefreshTokenCommand
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}