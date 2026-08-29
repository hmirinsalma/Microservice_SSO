using Microsoft.EntityFrameworkCore;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.Notification;
using TIMS.API.Entities;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _db;
    public NotificationService(ApplicationDbContext db) { _db = db; }

    public async Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(int userId, int page, int pageSize)
    {
        var q = _db.Notifications
            .Include(n => n.Intervention)
            .Where(n => n.UserId == userId)
            .OrderBy(n => n.IsRead).ThenByDescending(n => n.CreatedAt);

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<NotificationDto>
        {
            Items = items.Select(MapDto).ToList(),
            TotalCount = total, Page = page, PageSize = pageSize
        };
    }

    public async Task<int> GetUnreadCountAsync(int userId)
        => await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId)
            ?? throw new NotFoundException("Notification introuvable");
        n.IsRead = true; n.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, DateTime.UtcNow));
    }

    public async Task CreateNotificationAsync(int userId, string title, string message,
        NotificationType type, int? interventionId = null)
    {
        _db.Notifications.Add(new Notification
        {
            UserId = userId, Title = title, Message = message,
            Type = type, InterventionId = interventionId
        });
        await _db.SaveChangesAsync();
    }

    public async Task NotifyInterventionEventAsync(Intervention intervention, NotificationType type, string description)
    {
        var recipientIds = new HashSet<int>();

        // Technicien affecté
        if (intervention.TechnicienId.HasValue)
            recipientIds.Add(intervention.TechnicienId.Value);

        // Responsable
        if (intervention.ResponsableId.HasValue)
            recipientIds.Add(intervention.ResponsableId.Value);

        // Chef de service
        if (intervention.ChefServiceId.HasValue)
            recipientIds.Add(intervention.ChefServiceId.Value);

        // Tous les admins
        var admins = await _db.UserRoles
            .Where(ur => ur.Role.Name == RoleNames.AdminTechnique)
            .Select(ur => ur.UserId).ToListAsync();
        foreach (var a in admins) recipientIds.Add(a);

        var title = type switch
        {
            NotificationType.InterventionCreee => "Nouvelle intervention créée",
            NotificationType.TechnicienAffecte => "Vous avez été affecté à une intervention",
            NotificationType.ChangementTechnicien => "Technicien modifié",
            NotificationType.ChangementResponsable => "Responsable modifié",
            NotificationType.ChangementPriorite => "Priorité modifiée",
            NotificationType.ChangementStatut => "Statut modifié",
            NotificationType.InterventionTerminee => "Intervention terminée",
            _ => "Notification"
        };

        foreach (var uid in recipientIds)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = uid, Title = title,
                Message = $"[{intervention.NumeroIntervention}] {intervention.Objet} — {description}",
                Type = type, InterventionId = intervention.Id
            });
        }
        await _db.SaveChangesAsync();
    }

    private static NotificationDto MapDto(Notification n) => new()
    {
        Id = n.Id, Title = n.Title, Message = n.Message, Type = n.Type.ToString(),
        IsRead = n.IsRead, ReadAt = n.ReadAt, CreatedAt = n.CreatedAt,
        InterventionId = n.InterventionId,
        NumeroIntervention = n.Intervention?.NumeroIntervention
    };
}
