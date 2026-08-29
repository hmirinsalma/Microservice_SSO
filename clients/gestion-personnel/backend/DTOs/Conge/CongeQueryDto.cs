namespace GestionPersonnel.API.DTOs.Conge;

public class CongeQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Statut { get; set; }
    public int? EmployeId { get; set; }
    public int? DirectionId { get; set; }
    public int? ServiceId { get; set; }
}
