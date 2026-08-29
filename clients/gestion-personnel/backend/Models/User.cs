namespace GestionPersonnel.API.Models;

/// <summary>
/// Représente un compte utilisateur de l'application.
/// L'authentification est déléguée au microservice SSO.
/// SsoId = identifiant unique fourni par le SSO dans le claim 'sub'.
/// PasswordHash est supprimé — le SSO gère les mots de passe.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Identifiant unique côté SSO (claim 'sub' du JWT)</summary>
    public string? SsoId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK Rôle métier (distinct du rôle d'autorisation SSO)
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // Lien vers la fiche employé (1-1 optionnel)
    public Employe? Employe { get; set; }
}
