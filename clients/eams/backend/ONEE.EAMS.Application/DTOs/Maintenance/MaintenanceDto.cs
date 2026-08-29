using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.DTOs.Maintenance;

public record MaintenanceListDto(
    Guid Id, Guid EquipementId, string EquipementNom, string EquipementReference,
    Guid TechnicienId, string TechnicienNom, MaintenanceType Type, MaintenanceStatut Statut,
    DateTime DatePlanifiee, decimal? CoutEstime, DateTime CreatedAt);

public record MaintenanceDetailDto(
    Guid Id, Guid EquipementId, string EquipementNom, string EquipementReference,
    Guid TechnicienId, string TechnicienNom, MaintenanceType Type, MaintenanceStatut Statut,
    DateTime DatePlanifiee, DateTime? DateDebut, DateTime? DateCloture, int? DureeMinutes,
    EquipementEtat? EtatAvant, EquipementEtat? EtatApres, string? Observations,
    string? PiecesRemplacees, decimal? CoutEstime, decimal? CoutReel,
    DateTime? ProchaineMaintenance, DateTime CreatedAt, DateTime UpdatedAt);

public record CreateMaintenanceRequest(
    Guid EquipementId, Guid TechnicienId, MaintenanceType Type, DateTime DatePlanifiee,
    int? DureeMinutes, decimal? CoutEstime, string? Observations);

public record UpdateMaintenanceRequest(
    Guid TechnicienId, MaintenanceType Type, MaintenanceStatut Statut,
    DateTime DatePlanifiee, int? DureeMinutes, string? Observations,
    string? PiecesRemplacees, decimal? CoutEstime);

public record CloturerMaintenanceRequest(
    EquipementEtat EtatAvant, EquipementEtat EtatApres,
    string Observations, string? PiecesRemplacees, decimal? CoutReel,
    DateTime? ProchaineMaintenance);

public record MaintenanceFilterRequest
{
    public Guid? EquipementId { get; init; }
    public Guid? TechnicienId { get; init; }
    public MaintenanceType? Type { get; init; }
    public MaintenanceStatut? Statut { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
