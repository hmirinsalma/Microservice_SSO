using TIMS.API.Common;
using TIMS.API.DTOs.Notification;
using TIMS.API.Entities;

namespace TIMS.API.Interfaces;

public interface INotificationService
{
    Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(int userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAllAsReadAsync(int userId);
    Task CreateNotificationAsync(int userId, string title, string message, NotificationType type, int? interventionId = null);
    Task NotifyInterventionEventAsync(Intervention intervention, NotificationType type, string description);
}
