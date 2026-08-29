namespace GestionPersonnel.API.Models;

/// <summary>
/// Stocke les credentials locaux temporaires du StubAuthService.
/// Cette table sera supprimée lors de l'intégration du SSO.
/// Le modèle User principal reste propre — aucun champ d'authentification.
/// </summary>
public class StubCredential
{
    public int Id { get; set; }

    // FK vers User
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>Hash BCrypt — stub uniquement, supprimé avec SSO</summary>
    public string PasswordHash { get; set; } = string.Empty;
}
