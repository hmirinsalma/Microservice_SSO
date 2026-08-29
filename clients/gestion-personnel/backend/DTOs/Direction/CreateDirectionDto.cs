namespace GestionPersonnel.API.DTOs.Direction;

public class CreateDirectionDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
}
