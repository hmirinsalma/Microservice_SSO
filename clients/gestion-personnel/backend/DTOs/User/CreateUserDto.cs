namespace GestionPersonnel.API.DTOs.User;

/// <summary>
/// DTO de création d'un compte utilisateur.
/// Password : temporaire stub — sera supprimé lors de l'intégration SSO.
/// SsoId sera alimenté directement par le SSO après intégration.
/// </summary>
public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;

    /// <summary>TEMPORAIRE — stub uniquement. Supprimé avec SSO.</summary>
    public string? Password { get; set; }

    public int RoleId { get; set; }

    /// <summary>Lier à une fiche employé existante (optionnel)</summary>
    public int? EmployeId { get; set; }
}
