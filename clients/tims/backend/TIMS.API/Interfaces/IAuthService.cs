using TIMS.API.DTOs.Auth;

namespace TIMS.API.Interfaces;

/// <summary>
/// Interface d'authentification isolée.
///
/// Implémentations prévues :
///   ✅  StubAuthService     — temporaire, JWT local (actuellement en production)
///   🔜  SsoAuthService      — futur, délègue au microservice SSO via OIDC
///
/// Aucun Controller métier, Service métier, Repository ou Entity métier
/// ne dépend d'une implémentation concrète.
///
/// Pour intégrer le SSO : remplacer uniquement le binding DI dans Program.cs.
///   AVANT : services.AddScoped&lt;IAuthService, StubAuthService&gt;()
///   APRÈS : services.AddScoped&lt;IAuthService, SsoAuthService&gt;()
/// </summary>
public interface IAuthService
{
    /// <summary>Authentifie l'utilisateur et retourne un JWT + profil.</summary>
    Task<LoginResponseDto> LoginAsync(LoginDto dto);

    /// <summary>Déconnecte l'utilisateur (invalide la session SSO si applicable).</summary>
    Task LogoutAsync(int timsUserId);
}
