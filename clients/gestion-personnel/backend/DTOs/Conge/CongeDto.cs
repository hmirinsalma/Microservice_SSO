namespace GestionPersonnel.API.DTOs.Conge;

public class CongeDto
{
    public int Id { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public int NombreJours { get; set; }
    public string Motif { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public string? CommentaireChef { get; set; }
    public string? CommentaireDirecteur { get; set; }
    public DateTime? DateTraitementChef { get; set; }
    public DateTime? DateTraitementDirecteur { get; set; }
    public DateTime CreatedAt { get; set; }
    // Employé
    public int EmployeId { get; set; }
    public string EmployeNom { get; set; } = string.Empty;
    public string EmployePrenom { get; set; } = string.Empty;
    public string EmployeMatricule { get; set; } = string.Empty;
    public string DirectionNom { get; set; } = string.Empty;
    public string ServiceNom { get; set; } = string.Empty;
}
