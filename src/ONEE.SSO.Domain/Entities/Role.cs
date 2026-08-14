using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsSystemRole { get; set; }

    public Guid ClientId { get; set; }

    public ClientApplication Client { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}