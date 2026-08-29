namespace TIMS.API.Entities;

public class Equipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public ICollection<User> Members { get; set; } = new List<User>();
    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}
