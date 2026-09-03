namespace ONEE.SSO.Application.DTOs;

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    // Optionnel : pour mise à jour des rôles
    public List<Guid>? RoleIds { get; set; }

    // Optionnel : pour changement de mot de passe
    public string? Password { get; set; }
}