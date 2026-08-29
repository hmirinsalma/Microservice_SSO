namespace GestionPersonnel.API.DTOs.Employe;

public class EmployeDto
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime DateEmbauche { get; set; }
    public string Poste { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public int DirectionId { get; set; }
    public string DirectionNom { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public string ServiceNom { get; set; } = string.Empty;
    public int? ResponsableId { get; set; }
    public string? ResponsableNom { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
