using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.Services;

namespace ONEE.SSO.API.ViewComponents;

public class NotificationCountViewComponent : ViewComponent
{
    private readonly INotificationService _notificationService;

    public NotificationCountViewComponent(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        var count = 0;
        
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
        {
            count = await _notificationService.GetUnreadCountAsync(userId);
        }

        return View(count);
    }
}
