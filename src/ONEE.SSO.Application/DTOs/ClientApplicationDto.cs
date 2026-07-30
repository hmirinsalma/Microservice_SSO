namespace ONEE.SSO.Application.DTOs;

public class ClientApplicationDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}