namespace ONEE.EAMS.Domain.Entities;

public class EquipementDocument
{
    public Guid Id { get; set; }
    public Guid EquipementId { get; set; }
    public string NomFichier { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long TailleOctets { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Guid UploadedById { get; set; }

    public Equipement Equipement { get; set; } = null!;
    public User UploadedBy { get; set; } = null!;
}
