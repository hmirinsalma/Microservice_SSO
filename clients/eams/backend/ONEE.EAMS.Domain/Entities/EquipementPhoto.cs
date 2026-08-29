namespace ONEE.EAMS.Domain.Entities;

public class EquipementPhoto
{
    public Guid Id { get; set; }
    public Guid EquipementId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Guid UploadedById { get; set; }

    public Equipement Equipement { get; set; } = null!;
    public User UploadedBy { get; set; } = null!;
}
