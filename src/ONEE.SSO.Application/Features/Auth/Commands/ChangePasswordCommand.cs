namespace ONEE.SSO.Application.Features.Auth.Commands;

public class ChangePasswordCommand
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? AccessToken { get; set; }
}