using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Domain.Entities;

public class Equipement
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public Guid CategorieId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Marque { get; set; } = string.Empty;
    public string Modele { get; set; } = string.Empty;
    public string NumeroSerie { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public Guid ResponsableId { get; set; }
    public DateTime DateInstallation { get; set; }
    public DateTime? DateMiseEnService { get; set; }
    public EquipementEtat Etat { get; set; } = EquipementEtat.Disponible;
    public DateTime? DateFinGarantie { get; set; }
    public decimal? ValeurAcquisition { get; set; }
    public string? Fournisseur { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Categorie Categorie { get; set; } = null!;
    public ServiceEntity Service { get; set; } = null!;
    public User Responsable { get; set; } = null!;
    public ICollection<EquipementDocument> Documents { get; set; } = new List<EquipementDocument>();
    public ICollection<EquipementPhoto> Photos { get; set; } = new List<EquipementPhoto>();
    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    public ICollection<HistoriqueEntry> Historique { get; set; } = new List<HistoriqueEntry>();
    public ICollection<TechnicienEquipement> Techniciens { get; set; } = new List<TechnicienEquipement>();
}
