namespace ONEE.EAMS.Application.DTOs.Notification;

public record NotificationDto(Guid Id, string TypeEvenement, string Message, Guid RessourceId, string RessourceType, bool EstLue, DateTime CreatedAt);

public record UnreadCountDto(int Count);
