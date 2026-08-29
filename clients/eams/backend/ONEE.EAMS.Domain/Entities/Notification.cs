namespace ONEE.EAMS.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string TypeEvenement { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid RessourceId { get; set; }
    public string RessourceType { get; set; } = string.Empty;
    public Guid DestinataireId { get; set; }
    public bool EstLue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Destinataire { get; set; } = null!;
}
