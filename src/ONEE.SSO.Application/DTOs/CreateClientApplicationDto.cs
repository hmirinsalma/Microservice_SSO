namespace ONEE.SSO.Application.DTOs;

public class CreateClientApplicationDto
{
    public string Name { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    public string AllowedScopes { get; set; } = string.Empty;

    public string AllowedGrantTypes { get; set; } = string.Empty;

    public bool RequirePkce { get; set; }

    public bool RequireConsent { get; set; }

    public int AccessTokenLifetime { get; set; }

    public int RefreshTokenLifetime { get; set; }
}