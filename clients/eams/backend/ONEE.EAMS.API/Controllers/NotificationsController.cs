using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Notification;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await _service.GetMyNotificationsAsync(User);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var result = await _service.GetUnreadCountAsync(User);
        return Ok(ApiResponse<UnreadCountDto>.Ok(result));
    }

    [HttpPatch("{id:guid}/lire")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        await _service.MarkAsReadAsync(id, User);
        return Ok(ApiResponse<object>.Ok(new { message = "Notification marquée comme lue." }));
    }

    [HttpPatch("lire-tout")]
    public async Task<IActionResult> MarkAllRead()
    {
        await _service.MarkAllAsReadAsync(User);
        return Ok(ApiResponse<object>.Ok(new { message = "Toutes les notifications marquées comme lues." }));
    }
}
