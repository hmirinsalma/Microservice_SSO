namespace TIMS.API.Interfaces;

/// <summary>
/// ⚠️ INTERFACE TEMPORAIRE — STUB UNIQUEMENT
///
/// Isole toute logique de mot de passe hors des services métier.
/// Le UserService métier ne connaît plus BCrypt directement.
///
/// SSO Migration : Supprimer cette interface et son implémentation.
/// Le changement de mot de passe sera géré par le microservice SSO.
/// </summary>
public interface IStubPasswordService
{
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> VerifyPasswordAsync(int userId, string password);
    /// <summary>Crée les credentials stub pour un nouvel utilisateur.</summary>
    Task CreateCredentialAsync(int userId, string email, string password);
}
