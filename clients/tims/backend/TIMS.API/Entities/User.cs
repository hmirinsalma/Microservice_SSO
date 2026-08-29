namespace TIMS.API.Entities;

/// <summary>
/// Table métier uniquement.
/// Aucune donnée d'authentification ici.
/// L'authentification est déléguée au futur microservice SSO.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>
    /// Identifiant provenant du futur SSO (sub claim).
    /// Null jusqu'à l'intégration du SSO.
    /// </summary>
    public string? SsoId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string? Phone    { get; set; }
    public string? Poste    { get; set; }
    public string? ProfilePhotoPath { get; set; }
    public bool IsActive    { get; set; } = true;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ── Métier ────────────────────────────────────────────────────────────────
    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    public int? EquipeId { get; set; }
    public Equipe? Equipe { get; set; }

    /// <summary>
    /// Rôle métier : uniquement pour affichage, statistiques, notifications, affectations.
    /// Ne jamais utiliser pour les autorisations (→ claim JWT uniquement).
    /// </summary>
    public string? RoleMetier { get; set; }

    // Navigation collections
    public ICollection<UserRole>       UserRoles                    { get; set; } = new List<UserRole>();
    public ICollection<Notification>   Notifications                { get; set; } = new List<Notification>();
    public ICollection<Intervention>   InterventionsAsResponsable   { get; set; } = new List<Intervention>();
    public ICollection<Intervention>   InterventionsAsChefService   { get; set; } = new List<Intervention>();
    public ICollection<Intervention>   InterventionsAsTechnicien    { get; set; } = new List<Intervention>();
}
