using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.API.Authorization;
using ONEE.SSO.Application.Services;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.API.Pages;

[SsoAdminRequired]
public class NotificationsModel : PageModel
{
    private readonly INotificationService _notificationService;

    public NotificationsModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public List<NotificationDto> Notifications { get; set; } = new();
    public int UnreadCount { get; set; }

    public async Task OnGetAsync()
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            UnreadCount = await _notificationService.GetUnreadCountAsync(userId);

            Notifications = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ClientApplicationName = n.ClientApplicationName
            }).ToList();
        }
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMarkAllAsReadAsync()
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
        {
            await _notificationService.MarkAllAsReadAsync(userId);
        }

        return RedirectToPage();
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ClientApplicationName { get; set; }
    }
}
