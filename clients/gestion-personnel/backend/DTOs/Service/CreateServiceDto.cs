namespace GestionPersonnel.API.DTOs.Service;

public class CreateServiceDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DirectionId { get; set; }
}
