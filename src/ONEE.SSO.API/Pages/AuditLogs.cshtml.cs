using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ONEE.SSO.API.Pages;

public class AuditLogsModel : PageModel
{
    public List<AuditLogDto> Logs { get; set; } = new();
    
    public string? SearchTerm { get; set; }
    public string? SelectedAction { get; set; }
    public string? SelectedEntity { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }

    public void OnGet(string? search, string? action, string? entity, string? dateFrom, string? dateTo, int page = 1)
    {
        SearchTerm = search;
        SelectedAction = action;
        SelectedEntity = entity;
        CurrentPage = page;

        if (!string.IsNullOrEmpty(dateFrom))
            DateFrom = DateTime.Parse(dateFrom);
        
        if (!string.IsNullOrEmpty(dateTo))
            DateTo = DateTime.Parse(dateTo);

        // Generate mock data
        var allLogs = GenerateMockLogs();

        // Apply filters
        var filteredLogs = allLogs.AsQueryable();

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            filteredLogs = filteredLogs.Where(l => 
                l.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.UserEmail.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(SelectedAction))
        {
            filteredLogs = filteredLogs.Where(l => l.ActionClass == SelectedAction);
        }

        if (!string.IsNullOrEmpty(SelectedEntity))
        {
            filteredLogs = filteredLogs.Where(l => l.EntityType.ToLower() == SelectedEntity);
        }

        if (DateFrom.HasValue)
        {
            filteredLogs = filteredLogs.Where(l => l.CreatedAt >= DateFrom.Value);
        }

        if (DateTo.HasValue)
        {
            filteredLogs = filteredLogs.Where(l => l.CreatedAt <= DateTo.Value.AddDays(1));
        }

        // Pagination
        var logs = filteredLogs.ToList();
        TotalPages = (int)Math.Ceiling(logs.Count / (double)PageSize);
        Logs = logs
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }

    private List<AuditLogDto> GenerateMockLogs()
    {
        var now = DateTime.Now;
        return new List<AuditLogDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Connexion réussie",
                Description = "L'utilisateur s'est connecté à l'application Gestion Personnel",
                ActionName = "Connexion",
                ActionClass = "login",
                ActionIcon = "fas fa-sign-in-alt",
                EntityType = "Session",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddMinutes(-5),
                Details = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)\nApplication: Gestion Personnel\nSession ID: abc123"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Utilisateur créé",
                Description = "Nouvel utilisateur 'employe.5@onee.ma' créé avec le rôle Employe",
                ActionName = "Création",
                ActionClass = "create",
                ActionIcon = "fas fa-plus",
                EntityType = "User",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddMinutes(-15),
                Details = "Email: employe.5@onee.ma\nNom: John Doe\nRôle: Employe\nStatut: Actif"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Rôle modifié",
                Description = "Permissions du rôle 'Manager' mises à jour",
                ActionName = "Modification",
                ActionClass = "update",
                ActionIcon = "fas fa-edit",
                EntityType = "Role",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddMinutes(-30),
                Details = "Rôle: Manager\nPermissions ajoutées: users.write, roles.read\nPermissions retirées: users.delete"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Connexion réussie",
                Description = "L'utilisateur s'est connecté à l'application TIMS",
                ActionName = "Connexion",
                ActionClass = "login",
                ActionIcon = "fas fa-sign-in-alt",
                EntityType = "Session",
                UserEmail = "tech.1@onee.ma",
                IpAddress = "192.168.1.42",
                CreatedAt = now.AddHours(-1),
                Details = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)\nApplication: TIMS\nSession ID: xyz789"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Utilisateur supprimé",
                Description = "L'utilisateur 'test@onee.ma' a été supprimé définitivement",
                ActionName = "Suppression",
                ActionClass = "delete",
                ActionIcon = "fas fa-trash",
                EntityType = "User",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddHours(-2),
                Details = "Email: test@onee.ma\nRaison: Compte de test obsolète"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Déconnexion",
                Description = "L'utilisateur s'est déconnecté de l'application Gestion Personnel",
                ActionName = "Déconnexion",
                ActionClass = "logout",
                ActionIcon = "fas fa-sign-out-alt",
                EntityType = "Session",
                UserEmail = "chef.rh@onee.ma",
                IpAddress = "192.168.1.25",
                CreatedAt = now.AddHours(-3),
                Details = "Durée de session: 2h 15min\nApplication: Gestion Personnel"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Application activée",
                Description = "L'application 'EAMS' a été réactivée",
                ActionName = "Modification",
                ActionClass = "update",
                ActionIcon = "fas fa-toggle-on",
                EntityType = "Application",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddHours(-4),
                Details = "Application: EAMS\nStatut: Actif\nClient ID: eams-spa"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Permission créée",
                Description = "Nouvelle permission 'reports.export' créée",
                ActionName = "Création",
                ActionClass = "create",
                ActionIcon = "fas fa-key",
                EntityType = "Permission",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddHours(-5),
                Details = "Code: reports.export\nNom: Exporter les rapports\nDescription: Permet d'exporter les rapports au format PDF/Excel"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Connexion réussie",
                Description = "L'utilisateur s'est connecté à l'application EAMS",
                ActionName = "Connexion",
                ActionClass = "login",
                ActionIcon = "fas fa-sign-in-alt",
                EntityType = "Session",
                UserEmail = "manager.1@onee.ma",
                IpAddress = "192.168.1.58",
                CreatedAt = now.AddHours(-6),
                Details = "User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X)\nApplication: EAMS\nSession ID: def456"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Rôle créé",
                Description = "Nouveau rôle 'Superviseur' créé avec 8 permissions",
                ActionName = "Création",
                ActionClass = "create",
                ActionIcon = "fas fa-user-shield",
                EntityType = "Role",
                UserEmail = "admin@onee.ma",
                IpAddress = "192.168.1.10",
                CreatedAt = now.AddDays(-1),
                Details = "Rôle: Superviseur\nPermissions: users.read, dashboard.view, reports.read, reports.export, settings.read, sessions.view, logs.read, applications.view"
            }
        };
    }

    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ActionName { get; set; }
        public required string ActionClass { get; set; }
        public required string ActionIcon { get; set; }
        public required string EntityType { get; set; }
        public required string UserEmail { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Details { get; set; }
    }
}
