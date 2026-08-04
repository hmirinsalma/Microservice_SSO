using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
}