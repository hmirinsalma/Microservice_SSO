using GestionPersonnel.API.DTOs.Auth;

namespace GestionPersonnel.API.Services.Interfaces;

/// <summary>
/// Contrat d'authentification.
/// Implémenté par StubAuthService (temporaire) jusqu'à l'intégration du microservice SSO.
/// Le remplacement par SsoAuthService nécessitera uniquement de changer
/// l'enregistrement DI dans Program.cs.
/// Aucun Controller, Service ou Repository ne dépend de l'implémentation concrète.
/// </summary>
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
}
