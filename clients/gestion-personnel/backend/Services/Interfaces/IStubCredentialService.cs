namespace GestionPersonnel.API.Services.Interfaces;

/// <summary>
/// Interface de gestion des credentials temporaires (stub local).
///
/// TEMPORAIRE — Cette interface et son implémentation seront supprimées
/// lors de l'intégration du microservice SSO.
///
/// Le remplacement par SsoAuthService ne nécessitera que :
///   1. Supprimer l'enregistrement DI de IStubCredentialService
///   2. Supprimer la table StubCredentials (migration EF)
/// Aucune modification dans UserManagementService ou les Controllers.
/// </summary>
public interface IStubCredentialService
{
    /// <summary>Crée un credential (hash BCrypt) pour un User local.</summary>
    Task CreateAsync(int userId, string plainPassword);

    /// <summary>Supprime le credential d'un User (appelé à la suppression du compte).</summary>
    Task DeleteAsync(int userId);
}
