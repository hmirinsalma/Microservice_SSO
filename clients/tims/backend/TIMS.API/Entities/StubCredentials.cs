namespace TIMS.API.Entities;

/// <summary>
/// ⚠️  TABLE TEMPORAIRE — SSO STUB UNIQUEMENT
///
/// Cette table est utilisée uniquement par StubAuthService pour permettre
/// de tester l'application avant l'intégration du vrai microservice SSO.
///
/// INSTRUCTIONS DE SUPPRESSION lors de l'intégration SSO :
///   1. Supprimer cette entité
///   2. Supprimer la migration correspondante
///   3. Supprimer StubAuthService
///   4. Supprimer la table StubCredentials en base
///   5. Supprimer la page Login locale (remplacer par redirect OIDC)
///
/// La couche métier (Services, Repositories, Controllers métier)
/// ne connaît pas et ne doit jamais connaître cette entité.
/// </summary>
public class StubCredentials
{
    public int    Id           { get; set; }
    public int    UserId       { get; set; }
    public string Email        { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    // Not a navigation property on User — isolation volontaire
    public User User { get; set; } = null!;
}
