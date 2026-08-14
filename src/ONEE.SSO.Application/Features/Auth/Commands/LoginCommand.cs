namespace ONEE.SSO.Application.Features.Auth.Commands;

public class LoginCommand
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}