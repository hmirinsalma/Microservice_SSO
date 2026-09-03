using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.API.Authorization;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Pages;

[SsoAdminRequired]
public class AuditLogsModel : PageModel
{
    private readonly IAuditLogService _auditLogService;
    private readonly IUserService _userService;

    public AuditLogsModel(IAuditLogService auditLogService, IUserService userService)
    {
        _auditLogService = auditLogService;
        _userService = userService;
    }

    public List<AuditLogDto> Logs { get; set; } = new();
    
    public string? SearchTerm { get; set; }
    public string? SelectedAction { get; set; }
    public string? SelectedEntity { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(string? search, string? action, string? entity, string? dateFrom, string? dateTo, int page = 1)
    {
        SearchTerm = search;
        SelectedAction = action;
        SelectedEntity = entity;
        CurrentPage = page;

        if (!string.IsNullOrEmpty(dateFrom))
            DateFrom = DateTime.Parse(dateFrom);
        
        if (!string.IsNullOrEmpty(dateTo))
            DateTo = DateTime.Parse(dateTo);

        // Get real audit logs from database
        var realLogs = await _auditLogService.GetAllAsync();
        var allLogs = await ConvertToDisplayLogs(realLogs);

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
            filteredLogs = filteredLogs.Where(l => l.EntityType.ToLower() == SelectedEntity.ToLower());
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
        var logs = filteredLogs.OrderByDescending(l => l.CreatedAt).ToList();
        TotalPages = (int)Math.Ceiling(logs.Count / (double)PageSize);
        Logs = logs
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }

    public async Task<IActionResult> OnGetExportCsvAsync(string? search, string? action, string? entity, string? dateFrom, string? dateTo)
    {
        // Get logs with same filters as display
        var realLogs = await _auditLogService.GetAllAsync();
        var allLogs = await ConvertToDisplayLogs(realLogs);

        // Apply filters
        var filteredLogs = allLogs.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            filteredLogs = filteredLogs.Where(l => 
                l.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                l.UserEmail.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(action))
        {
            filteredLogs = filteredLogs.Where(l => l.ActionClass == action);
        }

        if (!string.IsNullOrEmpty(entity))
        {
            filteredLogs = filteredLogs.Where(l => l.EntityType.ToLower() == entity.ToLower());
        }

        if (!string.IsNullOrEmpty(dateFrom))
        {
            var dateFromParsed = DateTime.Parse(dateFrom);
            filteredLogs = filteredLogs.Where(l => l.CreatedAt >= dateFromParsed);
        }

        if (!string.IsNullOrEmpty(dateTo))
        {
            var dateToParsed = DateTime.Parse(dateTo);
            filteredLogs = filteredLogs.Where(l => l.CreatedAt <= dateToParsed.AddDays(1));
        }

        var logs = filteredLogs.OrderByDescending(l => l.CreatedAt).ToList();

        // Generate CSV
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Date/Heure,Utilisateur,Action,Type d'entité,Description,Adresse IP");

        foreach (var log in logs)
        {
            csv.AppendLine($"\"{log.CreatedAt:dd/MM/yyyy HH:mm:ss}\",\"{log.UserEmail}\",\"{log.Title}\",\"{log.EntityType}\",\"{log.Description}\",\"{log.IpAddress ?? "N/A"}\"");
        }

        var fileName = $"audit_logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        
        return File(bytes, "text/csv", fileName);
    }

    private async Task<List<AuditLogDto>> ConvertToDisplayLogs(IEnumerable<Application.DTOs.AuditLogDto> realLogs)
    {
        var displayLogs = new List<AuditLogDto>();

        foreach (var log in realLogs)
        {
            var userEmail = "Système";
            if (log.UserId.HasValue && log.UserId != Guid.Empty)
            {
                try
                {
                    var user = await _userService.GetByIdAsync(log.UserId.Value);
                    userEmail = user?.Email ?? "Utilisateur inconnu";
                }
                catch
                {
                    userEmail = "Utilisateur inconnu";
                }
            }

            var (title, actionClass, icon) = GetActionDetails(log.Action);
            var description = BuildDescription(log);

            displayLogs.Add(new AuditLogDto
            {
                Id = log.Id,
                Title = title,
                Description = description,
                ActionName = log.Action,
                ActionClass = actionClass,
                ActionIcon = icon,
                EntityType = log.EntityName,
                UserEmail = userEmail,
                IpAddress = log.IpAddress,
                CreatedAt = log.CreatedAt,
                Details = BuildDetails(log)
            });
        }

        return displayLogs;
    }

    private static (string Title, string ActionClass, string Icon) GetActionDetails(string action)
    {
        return action.ToLower() switch
        {
            "login" or "connexion" => ("Connexion réussie", "login", "fas fa-sign-in-alt"),
            "logout" or "déconnexion" => ("Déconnexion", "logout", "fas fa-sign-out-alt"),
            "create" or "création" or "créer" => ("Création", "create", "fas fa-plus"),
            "update" or "modification" or "modifier" => ("Modification", "update", "fas fa-edit"),
            "delete" or "suppression" or "supprimer" => ("Suppression", "delete", "fas fa-trash"),
            "unlock" or "débloquer" => ("Déblocage de compte", "update", "fas fa-unlock"),
            _ => (action, "other", "fas fa-info-circle")
        };
    }

    private static string BuildDescription(Application.DTOs.AuditLogDto log)
    {
        var entityName = log.EntityName;
        var action = log.Action.ToLower();

        return action switch
        {
            "login" or "connexion" => $"L'utilisateur s'est connecté au système SSO",
            "logout" or "déconnexion" => $"L'utilisateur s'est déconnecté du système SSO",
            "create" or "création" or "créer" => $"{entityName} créé(e) avec succès",
            "update" or "modification" or "modifier" => $"{entityName} modifié(e) avec succès",
            "delete" or "suppression" or "supprimer" => $"{entityName} supprimé(e)",
            "unlock" or "débloquer" => $"Compte utilisateur débloqué",
            _ => $"Action '{log.Action}' effectuée sur {entityName}"
        };
    }

    private static string BuildDetails(Application.DTOs.AuditLogDto log)
    {
        var details = new List<string>();

        if (!string.IsNullOrEmpty(log.EntityId))
            details.Add($"ID: {log.EntityId}");

        if (!string.IsNullOrEmpty(log.IpAddress))
            details.Add($"Adresse IP: {log.IpAddress}");

        if (!string.IsNullOrEmpty(log.UserAgent))
            details.Add($"User-Agent: {log.UserAgent}");

        if (!string.IsNullOrEmpty(log.OldValues))
            details.Add($"Anciennes valeurs:\n{log.OldValues}");

        if (!string.IsNullOrEmpty(log.NewValues))
            details.Add($"Nouvelles valeurs:\n{log.NewValues}");

        return details.Any() ? string.Join("\n", details) : "Aucun détail disponible";
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
