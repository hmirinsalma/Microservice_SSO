namespace GestionPersonnel.API.Models;

public enum StatutEmploye { Actif, Inactif, Suspendu }

public class Employe
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime DateEmbauche { get; set; }
    public string Poste { get; set; } = string.Empty;
    public StatutEmploye Statut { get; set; } = StatutEmploye.Actif;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Lien avec le compte utilisateur (optionnel)
    public int? UserId { get; set; }
    public User? User { get; set; }

    // Responsable hiérarchique (auto-référence)
    public int? ResponsableId { get; set; }
    public Employe? Responsable { get; set; }
    public ICollection<Employe> Subordonnes { get; set; } = new List<Employe>();

    // FK
    public int DirectionId { get; set; }
    public Direction Direction { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    // Navigation
    public ICollection<Conge> Conges { get; set; } = new List<Conge>();
}
