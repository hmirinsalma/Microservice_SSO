namespace ONEE.SSO.Application.DTOs;

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public Guid ClientId { get; set; }
}