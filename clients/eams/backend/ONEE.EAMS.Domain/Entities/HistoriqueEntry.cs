namespace ONEE.EAMS.Domain.Entities;

public class HistoriqueEntry
{
    public Guid Id { get; set; }
    public Guid EntiteId { get; set; }
    public string EntiteType { get; set; } = string.Empty;
    public string TypeEvenement { get; set; } = string.Empty;
    public string? ValeurAvant { get; set; }
    public string? ValeurApres { get; set; }
    public Guid AuteurId { get; set; }
    public DateTime HorodatageUtc { get; set; } = DateTime.UtcNow;

    public User Auteur { get; set; } = null!;
    public Equipement? Equipement { get; set; }
}
