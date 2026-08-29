namespace ONEE.EAMS.Application.DTOs.Dashboard;

public record AdminDashboardDto(
    int TotalEquipements,
    Dictionary<string, int> ParEtat,
    int MaintenancesPlanifiees,
    int MaintenancesEnRetard,
    decimal CoutTotalEstime);

public record DirecteurDashboardDto(
    int TotalEquipements,
    IEnumerable<StatItem> ParCategorie,
    IEnumerable<StatItem> ParService,
    Dictionary<string, int> EtatGlobal,
    IEnumerable<EquipementAlerte> EquipementsEnIntervention);

public record ChefServiceDashboardDto(
    int TotalEquipementsService,
    int MaintenancesAVenir7j,
    int EquipementsIndisponibles,
    int EquipementsRecents30j);

public record TechnicienDashboardDto(
    int EquipementsAffectes,
    int MaintenancesAujourdhui,
    int ProchainesMaintenances7j,
    int Interventions30j);

public record StatItem(string Nom, int Count);
public record EquipementAlerte(Guid Id, string Nom, string Reference, string Etat, string ServiceNom);
