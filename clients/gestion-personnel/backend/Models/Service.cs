namespace GestionPersonnel.API.Models;

public class Service
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }

    // FK
    public int DirectionId { get; set; }
    public Direction Direction { get; set; } = null!;

    // Navigation
    public ICollection<Employe> Employes { get; set; } = new List<Employe>();
}
