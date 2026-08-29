namespace GestionPersonnel.API.DTOs.Conge;

public class CreateCongeDto
{
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; } = string.Empty;
}
