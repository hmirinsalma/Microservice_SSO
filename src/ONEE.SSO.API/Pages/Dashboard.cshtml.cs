using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.API.Pages;

public class DashboardModel : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IClientApplicationRepository _clientApplicationRepository;
    private readonly IRoleRepository _roleRepository;

    public DashboardModel(
        IUserRepository userRepository,
        IClientApplicationRepository clientApplicationRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _clientApplicationRepository = clientApplicationRepository;
        _roleRepository = roleRepository;
    }

    // Statistics
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int ActiveSessions { get; set; }
    public int TotalApplications { get; set; }
    public int ActiveApplications { get; set; }
    public int LoginsToday { get; set; }
    public int LoginsTrend { get; set; }

    // Data Lists
    public List<RecentLoginDto> RecentLogins { get; set; } = new();
    public List<ClientAppDto> ClientApplications { get; set; } = new();
    public List<RoleDto> Roles { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Load statistics
        var users = await _userRepository.GetAllAsync();
        TotalUsers = users.Count();
        NewUsersThisMonth = users.Count(u => u.CreatedAt.Month == DateTime.Now.Month);
        ActiveSessions = 15; // TODO: Implement session tracking

        var applications = await _clientApplicationRepository.GetAllAsync();
        TotalApplications = applications.Count();
        ActiveApplications = applications.Count(a => a.IsActive);

        LoginsToday = 45; // TODO: Implement audit log counting
        LoginsTrend = 12; // TODO: Calculate trend

        // Recent logins (mock data for now)
        RecentLogins = new List<RecentLoginDto>
        {
            new() { UserEmail = "admin@onee.ma", ClientName = "Gestion Personnel", LoginTime = DateTime.Now.AddMinutes(-5) },
            new() { UserEmail = "chef.rh@onee.ma", ClientName = "Gestion Personnel", LoginTime = DateTime.Now.AddMinutes(-12) },
            new() { UserEmail = "tech.1@onee.ma", ClientName = "TIMS", LoginTime = DateTime.Now.AddMinutes(-25) },
            new() { UserEmail = "manager.1@onee.ma", ClientName = "EAMS", LoginTime = DateTime.Now.AddMinutes(-35) },
        };

        // Client applications
        ClientApplications = applications.Select(a => new ClientAppDto
        {
            Name = a.Name,
            IsActive = a.IsActive,
            Color = GetAppColor(a.ClientId),
            Icon = GetAppIcon(a.ClientId),
            UsersToday = GetAppUsersToday(a.ClientId)
        }).ToList();

        // Roles
        var roles = await _roleRepository.GetAllAsync();
        Roles = roles.Select(r => new RoleDto
        {
            Name = r.Name,
            UserCount = r.UserRoles?.Count ?? 0,
            PermissionCount = r.RolePermissions?.Count ?? 0
        }).ToList();
    }

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

    private int GetAppUsersToday(string clientId) => clientId switch
    {
        "gestion-personnel" => 28,
        "tims-app" => 12,
        "eams-spa" => 5,
        _ => 0
    };

    // DTOs
    public class RecentLoginDto
    {
        public required string UserEmail { get; set; }
        public required string ClientName { get; set; }
        public DateTime LoginTime { get; set; }
    }

    public class ClientAppDto
    {
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public required string Color { get; set; }
        public required string Icon { get; set; }
        public int UsersToday { get; set; }
    }

    public class RoleDto
    {
        public required string Name { get; set; }
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
    }
}
