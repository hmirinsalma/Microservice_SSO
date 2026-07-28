using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}