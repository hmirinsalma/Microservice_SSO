using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.API.Authorization;

namespace ONEE.SSO.API.Pages;

[SsoAdminRequired]
public class ClientApplicationsModel : PageModel
{
    private readonly IClientApplicationRepository _clientApplicationRepository;

    public ClientApplicationsModel(IClientApplicationRepository clientApplicationRepository)
    {
        _clientApplicationRepository = clientApplicationRepository;
    }

    public List<AppDto> Applications { get; set; } = new();

    public async Task OnGetAsync()
    {
        var apps = await _clientApplicationRepository.GetAllAsync();

        Applications = apps.Select(a => new AppDto
        {
            Id = a.Id,
            Name = a.Name,
            ClientId = a.ClientId,
            Description = GetDescription(a.ClientId),
            RedirectUri = a.RedirectUri,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt,
            Color = GetAppColor(a.ClientId),
            Icon = GetAppIcon(a.ClientId),
            TotalUsers = GetTotalUsers(a.ClientId),
            LoginsToday = GetLoginsToday(a.ClientId)
        }).ToList();
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(Guid id)
    {
        var app = await _clientApplicationRepository.GetByIdAsync(id);
        if (app == null)
            return NotFound();

        app.IsActive = !app.IsActive;
        _clientApplicationRepository.Update(app);
        await _clientApplicationRepository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Application {(app.IsActive ? "activée" : "désactivée")} avec succès";
        return RedirectToPage();
    }

    private string GetDescription(string clientId) => clientId switch
    {
        "gestion-personnel" => "Système de gestion des ressources humaines et du personnel ONEE",
        "tims-app" => "Système de gestion des interventions techniques et maintenance",
        "eams-spa" => "Système de gestion des équipements et actifs matériels",
        _ => "Application cliente SSO"
    };

    private string GetAppColor(string clientId) => clientId switch
    {
        "gestion-personnel" => "#1e3a8a",
        "tims-app" => "#10b981",
        "eams-spa" => "#f59e0b",
        _ => "#64748b"
    };

    private string GetAppIcon(string clientId) => clientId switch
    {
        "gestion-personnel" => "fas fa-users",
        "tims-app" => "fas fa-tools",
        "eams-spa" => "fas fa-cogs",
        _ => "fas fa-desktop"
    };

    private int GetTotalUsers(string clientId) => clientId switch
    {
        "gestion-personnel" => 156,
        "tims-app" => 89,
        "eams-spa" => 42,
        _ => 0
    };

    private int GetLoginsToday(string clientId) => clientId switch
    {
        "gestion-personnel" => 28,
        "tims-app" => 12,
        "eams-spa" => 5,
        _ => 0
    };

    public class AppDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ClientId { get; set; }
        public required string Description { get; set; }
        public required string RedirectUri { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Color { get; set; }
        public required string Icon { get; set; }
        public int TotalUsers { get; set; }
        public int LoginsToday { get; set; }
    }
}
