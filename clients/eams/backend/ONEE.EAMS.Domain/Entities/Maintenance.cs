using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Domain.Entities;

public class Maintenance
{
    public Guid Id { get; set; }
    public Guid EquipementId { get; set; }
    public Guid TechnicienId { get; set; }
    public MaintenanceType Type { get; set; }
    public MaintenanceStatut Statut { get; set; } = MaintenanceStatut.Planifiee;
    public DateTime DatePlanifiee { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateCloture { get; set; }
    public int? DureeMinutes { get; set; }
    public EquipementEtat? EtatAvant { get; set; }
    public EquipementEtat? EtatApres { get; set; }
    public string? Observations { get; set; }
    public string? PiecesRemplacees { get; set; }
    public decimal? CoutEstime { get; set; }
    public decimal? CoutReel { get; set; }
    public DateTime? ProchaineMaintenance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Equipement Equipement { get; set; } = null!;
    public User Technicien { get; set; } = null!;
}
