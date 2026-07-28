using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class UserSession : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public string SessionId { get; set; } = Guid.NewGuid().ToString();

    public string? Device { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public string? IpAddress { get; set; }

    public DateTime LoginAt { get; set; }

    public DateTime? LogoutAt { get; set; }

    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
}