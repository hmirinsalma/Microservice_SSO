namespace TIMS.API.Entities;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Equipe> Equipes { get; set; } = new List<Equipe>();
    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}
