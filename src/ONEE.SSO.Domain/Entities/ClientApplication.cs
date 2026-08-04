using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class ClientApplication : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Role> Roles { get; set; } = new List<Role>();

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}