namespace ONEE.SSO.Application.Features.Auth.Commands;

public class ResetPasswordCommand
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}