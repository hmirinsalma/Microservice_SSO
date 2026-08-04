namespace ONEE.SSO.Application.DTOs;

public class CreateClientApplicationDto
{
    public string Name { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}