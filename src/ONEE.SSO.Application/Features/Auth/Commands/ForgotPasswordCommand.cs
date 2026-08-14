namespace ONEE.SSO.Application.Features.Auth.Commands;

public class ForgotPasswordCommand
{
    public string Email { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}