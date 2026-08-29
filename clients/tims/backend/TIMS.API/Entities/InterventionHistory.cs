namespace TIMS.API.Entities;

public class InterventionHistory
{
    public int Id { get; set; }
    public int InterventionId { get; set; }
    public Intervention Intervention { get; set; } = null!;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public HistoryActionType ActionType { get; set; }
    public string? FieldChanged { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
