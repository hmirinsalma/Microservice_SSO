namespace ONEE.SSO.Application.DTOs;

public class UpdateClientApplicationDto
{
    public string Name { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}