namespace ONEE.SSO.Application.DTOs;

public class UpdateClientApplicationDto
{
    public string Name { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    public string AllowedScopes { get; set; } = string.Empty;

    public string AllowedGrantTypes { get; set; } = string.Empty;

    public bool RequirePkce { get; set; }

    public bool RequireConsent { get; set; }

    public int AccessTokenLifetime { get; set; }

    public int RefreshTokenLifetime { get; set; }
}