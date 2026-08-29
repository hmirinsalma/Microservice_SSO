namespace GestionPersonnel.API.Models;

public class Direction
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Employe> Employes { get; set; } = new List<Employe>();
}
