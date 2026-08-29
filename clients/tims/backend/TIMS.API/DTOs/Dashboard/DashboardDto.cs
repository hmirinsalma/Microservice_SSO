namespace TIMS.API.DTOs.Dashboard;

public class AdminDashboardDto
{
    public int TotalInterventions { get; set; }
    public int Nouvelles { get; set; }
    public int EnCours { get; set; }
    public int Suspendues { get; set; }
    public int Terminees { get; set; }
    public int Annulees { get; set; }
    public int Urgentes { get; set; }
    public int Critiques { get; set; }
    public List<StatItem> ByPriority { get; set; } = new();
    public List<StatItem> ByEquipe { get; set; } = new();
    public List<StatItem> ByStatus { get; set; } = new();
}

public class DirecteurDashboardDto
{
    public int TotalInterventions { get; set; }
    public int InterventionsCritiques { get; set; }
    public int InterventionsCloturees30j { get; set; }
    public List<StatItem> ByEquipe { get; set; } = new();
    public List<StatItem> ByService { get; set; } = new();
    public List<StatItem> ByStatus { get; set; } = new();
}

public class ChefServiceDashboardDto
{
    public int TotalServiceInterventions { get; set; }
    public int Urgentes { get; set; }
    public int EnAttente { get; set; }
    public int TechniciensDisponibles { get; set; }
    public int TechniciensOccupes { get; set; }
    public List<StatItem> ByStatus { get; set; } = new();
}

public class TechnicienDashboardDto
{
    public int TotalAffectees { get; set; }
    public int EnCours { get; set; }
    public int Terminees { get; set; }
    public int Urgentes { get; set; }
    public List<ProchainInterventionDto> Prochaines { get; set; } = new();
}

public class StatItem
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProchainInterventionDto
{
    public int Id { get; set; }
    public string NumeroIntervention { get; set; } = string.Empty;
    public string Objet { get; set; } = string.Empty;
    public DateTime DatePrevue { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
}
