namespace GestionPersonnel.API.DTOs.Service;

public class ServiceDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DirectionId { get; set; }
    public string DirectionNom { get; set; } = string.Empty;
    public int NombreEmployes { get; set; }
}
