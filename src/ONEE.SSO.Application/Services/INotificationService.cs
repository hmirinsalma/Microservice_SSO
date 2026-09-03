using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(Guid userId, string title, string message, string type = "info", string? clientApplicationName = null, string? ipAddress = null, string? userAgent = null);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool? isRead = null);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
}
