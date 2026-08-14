namespace ONEE.SSO.Application.Features.Users.Commands;

public class UnlockUserCommand
{
    public Guid UserId { get; set; }
    public Guid AdminUserId { get; set; }
    public string? IpAddress { get; set; }
}