namespace ONEE.SSO.Application.DTOs;

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public Guid ClientId { get; set; }
}