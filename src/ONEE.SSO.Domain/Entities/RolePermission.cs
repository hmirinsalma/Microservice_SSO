using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}