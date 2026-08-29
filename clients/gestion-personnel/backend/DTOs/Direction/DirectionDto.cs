namespace GestionPersonnel.API.DTOs.Direction;

public class DirectionDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int NombreServices { get; set; }
    public int NombreEmployes { get; set; }
}
