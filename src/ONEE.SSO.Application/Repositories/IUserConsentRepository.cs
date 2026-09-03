using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IUserConsentRepository : IRepository<UserConsent>
{
    /// <summary>
    /// Récupère le consentement d'un utilisateur pour une application cliente donnée
    /// </summary>
    Task<UserConsent?> GetByUserAndClientAsync(Guid userId, string clientId);
    
    /// <summary>
    /// Récupère tous les consentements d'un utilisateur
    /// </summary>
    Task<IEnumerable<UserConsent>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Vérifie si un consentement valide existe pour un utilisateur et une application
    /// </summary>
    Task<bool> HasValidConsentAsync(Guid userId, string clientId);
    
    /// <summary>
    /// Révoque tous les consentements d'un utilisateur pour une application donnée
    /// </summary>
    Task RevokeConsentAsync(Guid userId, string clientId);
}
