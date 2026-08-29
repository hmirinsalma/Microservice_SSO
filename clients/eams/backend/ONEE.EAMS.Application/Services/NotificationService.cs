using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Notification;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;
using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IAppDbContext _db;

    public NotificationService(IAppDbContext db) => _db = db;

    public async Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        return await _db.Notifications
            .Where(n => n.DestinataireId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.TypeEvenement, n.Message, n.RessourceId, n.RessourceType, n.EstLue, n.CreatedAt))
            .ToListAsync();
    }

    public async Task<UnreadCountDto> GetUnreadCountAsync(ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        var count = await _db.Notifications.CountAsync(n => n.DestinataireId == userId && !n.EstLue);
        return new UnreadCountDto(count);
    }

    public async Task MarkAsReadAsync(Guid id, ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.DestinataireId == userId)
            ?? throw new NotFoundException("Notification introuvable.");
        n.EstLue = true;
        await _db.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        var notifs = await _db.Notifications.Where(n => n.DestinataireId == userId && !n.EstLue).ToListAsync();
        foreach (var n in notifs) n.EstLue = true;
        await _db.SaveChangesAsync();
    }

    public async Task CreateAsync(string typeEvenement, string message, Guid ressourceId, string ressourceType, Guid destinataireId)
    {
        _db.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            TypeEvenement = typeEvenement,
            Message = message,
            RessourceId = ressourceId,
            RessourceType = ressourceType,
            DestinataireId = destinataireId,
            EstLue = false,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task CheckGarantieExpirationsAsync()
    {
        var soon = DateTime.UtcNow.AddDays(30);
        var cutoff = DateTime.UtcNow.AddDays(-7);

        // Une seule requête : équipements dont la garantie expire bientôt
        // ET qui n'ont pas déjà une notification récente
        var equipements = await _db.Equipements
            .Where(e => e.DateFinGarantie.HasValue
                && e.DateFinGarantie.Value <= soon
                && e.DateFinGarantie.Value >= DateTime.UtcNow)
            .Select(e => new { e.Id, e.Nom, e.DateFinGarantie })
            .ToListAsync();

        if (!equipements.Any()) return;

        var adminIds = await _db.Users
            .Where(u => u.RoleMetier == UserRole.Admin_Patrimoine && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync();

        if (!adminIds.Any()) return;

        var equipementIds = equipements.Select(e => e.Id).ToList();

        // Récupère en une seule requête toutes les notifications déjà envoyées
        var alreadySent = await _db.Notifications
            .Where(n => n.TypeEvenement == "GarantieExpirante"
                && equipementIds.Contains(n.RessourceId)
                && n.CreatedAt >= cutoff)
            .Select(n => new { n.RessourceId, n.DestinataireId })
            .ToListAsync();

        var toCreate = new List<Notification>();
        foreach (var eq in equipements)
        {
            foreach (var adminId in adminIds)
            {
                bool sent = alreadySent.Any(x => x.RessourceId == eq.Id && x.DestinataireId == adminId);
                if (!sent)
                {
                    toCreate.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        TypeEvenement = "GarantieExpirante",
                        Message = $"La garantie de l'équipement '{eq.Nom}' expire le {eq.DateFinGarantie!.Value:dd/MM/yyyy}.",
                        RessourceId = eq.Id,
                        RessourceType = "Equipement",
                        DestinataireId = adminId,
                        EstLue = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        if (toCreate.Any())
        {
            _db.Notifications.AddRange(toCreate);
            await _db.SaveChangesAsync();
        }
    }
}
