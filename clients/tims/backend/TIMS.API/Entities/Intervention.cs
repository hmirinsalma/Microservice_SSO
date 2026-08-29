namespace TIMS.API.Entities;

public class Intervention
{
    public int Id { get; set; }
    public string NumeroIntervention { get; set; } = string.Empty;
    public string Objet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TypeIntervention { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;
    public string Equipement { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DatePrevue { get; set; }
    public DateTime? DateCloture { get; set; }
    public InterventionPriority Priority { get; set; } = InterventionPriority.Normale;
    public InterventionStatus Status { get; set; } = InterventionStatus.Nouvelle;
    public string? CompteRendu { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? UpdatedAt { get; set; }

    // Relations
    public int? ResponsableId { get; set; }
    public User? Responsable { get; set; }

    public int? ChefServiceId { get; set; }
    public User? ChefService { get; set; }

    public int? TechnicienId { get; set; }
    public User? Technicien { get; set; }

    public int? EquipeId { get; set; }
    public Equipe? Equipe { get; set; }

    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public ICollection<InterventionHistory> History { get; set; } = new List<InterventionHistory>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
