namespace GestionPersonnel.API.DTOs.Employe;

public class EmployeQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public int? DirectionId { get; set; }
    public int? ServiceId { get; set; }
    public string? Statut { get; set; }
}
