using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class AuditLog : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}