using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.API.Authorization;

namespace ONEE.SSO.API.Pages;

[SsoAdminRequired]
public class SessionsModel : PageModel
{
    public List<SessionDto> Sessions { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? SelectedApp { get; set; }
    
    public int ActiveSessions { get; set; }
    public int ActiveApplications { get; set; }
    public int AverageDuration { get; set; }

    public void OnGet(string? search, string? app)
    {
        SearchTerm = search;
        SelectedApp = app;

        // Mock data for demonstration
        var allSessions = GenerateMockSessions();

        // Apply filters
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            allSessions = allSessions
                .Where(s => s.UserEmail.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrEmpty(SelectedApp))
        {
            allSessions = allSessions
                .Where(s => s.AppId == SelectedApp)
                .ToList();
        }

        Sessions = allSessions;

        // Calculate stats
        ActiveSessions = Sessions.Count;
        ActiveApplications = Sessions.Select(s => s.AppId).Distinct().Count();
        AverageDuration = Sessions.Any() ? (int)Sessions.Average(s => s.Duration) : 0;
    }

    public IActionResult OnPostRevokeSession(Guid sessionId)
    {
        // TODO: Implement session revocation
        // 1. Find session in database
        // 2. Mark as revoked
        // 3. Invalidate JWT (add to blacklist)
        
        TempData["SuccessMessage"] = "Session révoquée avec succès";
        return RedirectToPage();
    }

    public IActionResult OnPostRevokeAllSessions()
    {
        // TODO: Implement revoke all sessions
        // 1. Mark all sessions as revoked
        // 2. Clear session store
        
        TempData["SuccessMessage"] = "Toutes les sessions ont été révoquées";
        return RedirectToPage();
    }

    public IActionResult OnGetExportCsv(string? search, string? app)
    {
        // Get sessions with same filters
        var allSessions = GenerateMockSessions();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
        {
            allSessions = allSessions
                .Where(s => s.UserEmail.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrEmpty(app))
        {
            allSessions = allSessions
                .Where(s => s.AppId == app)
                .ToList();
        }

        // Generate CSV
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Email Utilisateur,Application,Adresse IP,Navigateur,Démarré le,Dernière activité,Durée (min)");

        foreach (var session in allSessions)
        {
            csv.AppendLine($"\"{session.UserEmail}\",\"{session.AppName}\",\"{session.IpAddress}\",\"{session.BrowserName}\",\"{session.StartedAt:dd/MM/yyyy HH:mm:ss}\",\"{session.LastActivity:dd/MM/yyyy HH:mm:ss}\",\"{session.Duration}\"");
        }

        var fileName = $"sessions_actives_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        
        return File(bytes, "text/csv", fileName);
    }

    private List<SessionDto> GenerateMockSessions()
    {
        var now = DateTime.Now;
        return new List<SessionDto>
        {
            new()
            {
                SessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserEmail = "admin@onee.ma",
                AppId = "gestion-personnel",
                AppName = "Gestion Personnel",
                AppColor = "#1e3a8a",
                AppIcon = "fas fa-users",
                IpAddress = "192.168.1.10",
                BrowserName = "Chrome",
                BrowserIcon = "chrome",
                StartedAt = now.AddMinutes(-45),
                LastActivity = now.AddMinutes(-2),
                Duration = 45,
                LastActivityAgo = "il y a 2 min"
            },
            new()
            {
                SessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserEmail = "chef.rh@onee.ma",
                AppId = "gestion-personnel",
                AppName = "Gestion Personnel",
                AppColor = "#1e3a8a",
                AppIcon = "fas fa-users",
                IpAddress = "192.168.1.25",
                BrowserName = "Firefox",
                BrowserIcon = "firefox",
                StartedAt = now.AddMinutes(-120),
                LastActivity = now.AddMinutes(-5),
                Duration = 120,
                LastActivityAgo = "il y a 5 min"
            },
            new()
            {
                SessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserEmail = "tech.1@onee.ma",
                AppId = "tims-app",
                AppName = "TIMS",
                AppColor = "#10b981",
                AppIcon = "fas fa-tools",
                IpAddress = "192.168.1.42",
                BrowserName = "Edge",
                BrowserIcon = "edge",
                StartedAt = now.AddMinutes(-30),
                LastActivity = now.AddMinutes(-1),
                Duration = 30,
                LastActivityAgo = "il y a 1 min"
            },
            new()
            {
                SessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserEmail = "manager.1@onee.ma",
                AppId = "eams-spa",
                AppName = "EAMS",
                AppColor = "#f59e0b",
                AppIcon = "fas fa-cogs",
                IpAddress = "192.168.1.58",
                BrowserName = "Safari",
                BrowserIcon = "safari",
                StartedAt = now.AddMinutes(-90),
                LastActivity = now.AddMinutes(-10),
                Duration = 90,
                LastActivityAgo = "il y a 10 min"
            },
            new()
            {
                SessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserEmail = "employe.1@onee.ma",
                AppId = "gestion-personnel",
                AppName = "Gestion Personnel",
                AppColor = "#1e3a8a",
                AppIcon = "fas fa-users",
                IpAddress = "192.168.1.73",
                BrowserName = "Opera",
                BrowserIcon = "opera",
                StartedAt = now.AddMinutes(-15),
                LastActivity = now.AddMinutes(-3),
                Duration = 15,
                LastActivityAgo = "il y a 3 min"
            }
        };
    }

    public class SessionDto
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public required string UserEmail { get; set; }
        public required string AppId { get; set; }
        public required string AppName { get; set; }
        public required string AppColor { get; set; }
        public required string AppIcon { get; set; }
        public required string IpAddress { get; set; }
        public required string BrowserName { get; set; }
        public required string BrowserIcon { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public int Duration { get; set; }
        public required string LastActivityAgo { get; set; }
    }
}
