using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class ClientApplication : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    // Nouvelle URL après déconnexion
    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    // Exemple :
    // "openid profile email roles offline_access eams"
    public string AllowedScopes { get; set; } = string.Empty;

    // Exemple :
    // "authorization_code"
    public string AllowedGrantTypes { get; set; } = string.Empty;

    public bool RequirePkce { get; set; }

    public bool RequireConsent { get; set; }

    // en secondes
    public int AccessTokenLifetime { get; set; }

    // en secondes
    public int RefreshTokenLifetime { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Role> Roles { get; set; } = new List<Role>();

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}