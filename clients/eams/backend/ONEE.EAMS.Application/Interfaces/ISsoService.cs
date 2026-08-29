namespace ONEE.EAMS.Application.Interfaces;

/// <summary>
/// Contrat d'intégration avec le futur microservice SSO.
///
/// À implémenter lorsque le microservice SSO sera disponible.
/// L'implémentation concrète (SsoService) devra communiquer avec les endpoints
/// du SSO via HTTP pour valider les tokens, récupérer les profils et obtenir les rôles.
///
/// Points d'intégration prévus :
///   POST {SSO_BASE_URL}/auth/token          → LoginAsync
///   POST {SSO_BASE_URL}/auth/logout         → LogoutAsync
///   GET  {SSO_BASE_URL}/users/{ssoId}       → GetUserProfileAsync
///   GET  {SSO_BASE_URL}/users/{ssoId}/roles → GetUserRolesAsync
/// </summary>
public interface ISsoService
{
    /// <summary>
    /// Valide un token JWT émis par le SSO et retourne les claims de l'utilisateur.
    /// En attendant le SSO, cette méthode n'est pas appelée — la validation JWT
    /// locale (middleware ASP.NET Core) est maintenue temporairement.
    /// </summary>
    Task<SsoUserInfo?> ValidateTokenAsync(string token);

    /// <summary>
    /// Retourne les informations de profil d'un utilisateur depuis le SSO.
    /// </summary>
    Task<SsoUserInfo?> GetUserProfileAsync(string ssoId);
}

/// <summary>
/// Représente les informations utilisateur fournies par le microservice SSO.
/// </summary>
public record SsoUserInfo(
    string SsoId,
    string Email,
    string Nom,
    string Prenom,
    string Role,
    string? ServiceId,
    bool IsActive
);
