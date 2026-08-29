namespace GestionPersonnel.API.DTOs.Employe;

public class UpdateEmployeDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public DateTime DateEmbauche { get; set; }
    public string Poste { get; set; } = string.Empty;
    public string Statut { get; set; } = "Actif";
    public int DirectionId { get; set; }
    public int ServiceId { get; set; }
}
