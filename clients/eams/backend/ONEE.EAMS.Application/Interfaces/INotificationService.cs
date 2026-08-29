using System.Security.Claims;
using ONEE.EAMS.Application.DTOs.Notification;

namespace ONEE.EAMS.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(ClaimsPrincipal user);
    Task<UnreadCountDto> GetUnreadCountAsync(ClaimsPrincipal user);
    Task MarkAsReadAsync(Guid id, ClaimsPrincipal user);
    Task MarkAllAsReadAsync(ClaimsPrincipal user);
    Task CreateAsync(string typeEvenement, string message, Guid ressourceId, string ressourceType, Guid destinataireId);
    Task CheckGarantieExpirationsAsync();
}
