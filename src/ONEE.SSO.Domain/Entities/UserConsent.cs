namespace ONEE.SSO.Domain.Entities;

/// <summary>
/// Représente le consentement d'un utilisateur pour accéder à une application cliente.
/// Permet de mémoriser les autorisations pour éviter de redemander le consentement à chaque connexion.
/// </summary>
public class UserConsent
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// ID de l'utilisateur qui a donné son consentement
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// ID de l'application cliente (client_id OIDC)
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Scopes autorisés par l'utilisateur (ex: "openid profile email")
    /// </summary>
    public string Scopes { get; set; } = string.Empty;
    
    /// <summary>
    /// Date à laquelle le consentement a été accordé
    /// </summary>
    public DateTime GrantedAt { get; set; }
    
    /// <summary>
    /// Date d'expiration du consentement (optionnel, null = permanent)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Adresse IP d'où le consentement a été donné
    /// </summary>
    public string? IpAddress { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
