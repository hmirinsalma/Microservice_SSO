namespace TIMS.API.Entities;

public class Attachment
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int InterventionId { get; set; }
    public Intervention Intervention { get; set; } = null!;
    public int UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
}
