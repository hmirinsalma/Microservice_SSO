namespace ONEE.EAMS.Domain.Entities;

public class TechnicienEquipement
{
    public Guid TechnicienId { get; set; }
    public Guid EquipementId { get; set; }
    public DateTime AffectedAt { get; set; } = DateTime.UtcNow;
    public Guid AffectedById { get; set; }

    public User Technicien { get; set; } = null!;
    public Equipement Equipement { get; set; } = null!;
}
