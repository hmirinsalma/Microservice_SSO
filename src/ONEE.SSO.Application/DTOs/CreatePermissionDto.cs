namespace ONEE.SSO.Application.DTOs;

public class CreatePermissionDto
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }
}