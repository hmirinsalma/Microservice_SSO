using GestionPersonnel.API.DTOs.Conge;
using GestionPersonnel.API.DTOs.Employe;

namespace GestionPersonnel.API.DTOs.Dashboard;

// Admin RH
public class AdminDashboardDto
{
    public int TotalEmployes { get; set; }
    public int TotalDirections { get; set; }
    public int TotalServices { get; set; }
    public int TotalCongesEnAttente { get; set; }
    public int TotalConges { get; set; }
    public IEnumerable<EmployeDto> DerniersEmployes { get; set; } = [];
    public IEnumerable<DirectionStatDto> EmployesParDirection { get; set; } = [];
}

// Directeur
public class DirecteurDashboardDto
{
    public int TotalEmployes { get; set; }
    public int TotalServices { get; set; }
    public string DirectionNom { get; set; } = string.Empty;
    public int CongesEnAttente { get; set; }
    public IEnumerable<EmployeDto> DerniersRecrutes { get; set; } = [];
    public IEnumerable<ServiceStatDto> EmployesParService { get; set; } = [];
}

// Chef de service
public class ChefServiceDashboardDto
{
    public int TotalEmployes { get; set; }
    public string ServiceNom { get; set; } = string.Empty;
    public int CongesEnAttente { get; set; }
    public int CongesAcceptes { get; set; }
    public int CongesRefuses { get; set; }
    public IEnumerable<EmployeDto> Employes { get; set; } = [];
}

// Employé
public class EmployeDashboardDto
{
    public EmployeDto? Profil { get; set; }
    public int CongesEnAttente { get; set; }
    public int CongesAcceptes { get; set; }
    public int CongesRefuses { get; set; }
    public IEnumerable<CongeDto> DernieresDemandesConge { get; set; } = [];
}

// Statistiques
public class DirectionStatDto { public string Nom { get; set; } = ""; public int NombreEmployes { get; set; } }
public class ServiceStatDto   { public string Nom { get; set; } = ""; public int NombreEmployes { get; set; } }
