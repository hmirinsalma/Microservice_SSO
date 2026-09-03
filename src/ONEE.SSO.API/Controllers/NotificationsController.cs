using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.Services;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Crée une notification pour un utilisateur (appelé par les applications clientes)
    /// </summary>
    [HttpPost("create")]
    [AllowAnonymous] // Les apps clientes peuvent appeler sans auth
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return BadRequest(new { message = "UserId invalide" });
            }

            await _notificationService.CreateNotificationAsync(
                userId,
                request.Title,
                request.Message,
                request.Type ?? "info",
                request.ClientApplicationName,
                request.IpAddress,
                request.UserAgent
            );

            _logger.LogInformation($"✅ Notification créée pour user {userId} depuis {request.ClientApplicationName}");

            return Ok(new { success = true, message = "Notification créée avec succès" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la création de notification");
            return StatusCode(500, new { success = false, message = "Erreur lors de la création de la notification" });
        }
    }

    /// <summary>
    /// Récupère les notifications d'un utilisateur
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] bool? isRead = null)
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, isRead);
        return Ok(notifications);
    }

    /// <summary>
    /// Récupère le nombre de notifications non lues
    /// </summary>
    [HttpGet("user/{userId}/unread-count")]
    [Authorize]
    public async Task<IActionResult> GetUnreadCount(Guid userId)
    {
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { count });
    }

    /// <summary>
    /// Marque une notification comme lue
    /// </summary>
    [HttpPost("{notificationId}/mark-as-read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        await _notificationService.MarkAsReadAsync(notificationId);
        return Ok(new { success = true });
    }

    /// <summary>
    /// Marque toutes les notifications comme lues
    /// </summary>
    [HttpPost("user/{userId}/mark-all-as-read")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead(Guid userId)
    {
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new { success = true });
    }

    public class CreateNotificationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? ClientApplicationName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
