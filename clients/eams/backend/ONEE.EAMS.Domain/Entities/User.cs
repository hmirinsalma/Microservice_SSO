using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Domain.Entities;

/// <summary>
/// Représente un utilisateur EAMS.
/// Les informations d'authentification (mot de passe, hash, tokens)
/// sont supprimées — elles seront gérées exclusivement par le futur microservice SSO.
/// Seul le SsoId (identifiant externe fourni par le SSO) est conservé pour la liaison.
/// </summary>
public class User
{
    public Guid Id { get; set; }

    /// <summary>
    /// Identifiant externe fourni par le microservice SSO (sub claim du JWT SSO).
    /// Nullable pendant la phase de transition — requis après intégration SSO.
    /// </summary>
    public string? SsoId { get; set; }

    // ── Informations métier ─────────────────────────────────────────────────
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Poste { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Rôle applicatif de l'utilisateur dans EAMS.
    ///
    /// IMPORTANT — Architecture SSO :
    /// Ce champ a DEUX usages distincts et documentés :
    ///
    /// 1. CIBLAGE MÉTIER (usage interne EAMS) :
    ///    Utilisé pour les requêtes métier comme "qui notifier en cas de panne ?"
    ///    ou "quels techniciens affecter ?". C'est une copie locale synchronisée
    ///    depuis le SSO lors de la liaison du compte.
    ///
    /// 2. AUTORISATION → JAMAIS utilisé pour autoriser une action.
    ///    L'autorisation se base UNIQUEMENT sur le claim 'role' du JWT fourni par le SSO.
    ///
    /// Lors de l'intégration SSO, une synchronisation de ce champ devra être
    /// implémentée (webhook ou appel à l'intégration SSO lors du login).
    /// </summary>
    public UserRole RoleMetier { get; set; }

    public Guid? ServiceId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ── Navigation ──────────────────────────────────────────────────────────
    public ServiceEntity? Service { get; set; }
    public ICollection<Equipement> EquipementsResponsable { get; set; } = new List<Equipement>();
    public ICollection<TechnicienEquipement> Affectations { get; set; } = new List<TechnicienEquipement>();
    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
