using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.DTOs.Notification;
using TIMS.API.Extensions;
using TIMS.API.Interfaces;

namespace TIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _svc;
    public NotificationsController(INotificationService svc) { _svc = svc; }

    private int UserId => ClaimsHelper.GetTimsUserId(User);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(ApiResponse<PagedResult<NotificationDto>>.Ok(await _svc.GetUserNotificationsAsync(UserId, page, pageSize)));

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> UnreadCount()
        => Ok(ApiResponse<int>.Ok(await _svc.GetUnreadCountAsync(UserId)));

    [HttpPatch("{id:int}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkRead(int id)
    {
        await _svc.MarkAsReadAsync(id, UserId);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllRead()
    {
        await _svc.MarkAllAsReadAsync(UserId);
        return Ok(ApiResponse<object>.Ok(null!, "Toutes les notifications lues"));
    }
}
