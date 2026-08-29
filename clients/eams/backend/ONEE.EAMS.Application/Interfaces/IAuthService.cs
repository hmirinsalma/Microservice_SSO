using ONEE.EAMS.Application.DTOs.Auth;

namespace ONEE.EAMS.Application.Interfaces;

/// <summary>
/// Interface d'authentification SSO-ready.
///
/// Architecture de migration :
///   Phase actuelle (dev/démo) → StubAuthService : simule le SSO, lit les users en base
///   Phase production          → SsoAuthService  : délègue au microservice SSO externe
///
/// Aucun Controller, Service métier ou Repository ne dépend de l'implémentation concrète.
/// Le remplacement se fait uniquement dans la couche Infrastructure (DI registration).
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authentifie l'utilisateur.
    /// Phase actuelle : vérifie l'email en base et génère un JWT local.
    /// Phase SSO      : délègue au microservice SSO, retourne le token SSO.
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Déconnecte l'utilisateur.
    /// Phase SSO : révoque la session au niveau du SSO.
    /// </summary>
    Task LogoutAsync(Guid userId);
}
