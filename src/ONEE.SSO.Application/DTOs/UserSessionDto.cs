namespace ONEE.SSO.Application.DTOs;

public class UserSessionDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime LoginAt { get; set; }

    public DateTime? LogoutAt { get; set; }

    public bool IsActive { get; set; }
}