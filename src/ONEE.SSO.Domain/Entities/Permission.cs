using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid ClientId { get; set; }

    public ClientApplication Client { get; set; } = null!;

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}