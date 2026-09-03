namespace ONEE.SSO.Application.DTOs;

public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public List<Guid> RoleIds { get; set; } = new();

    public bool IsActive { get; set; } = true;
}