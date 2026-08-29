using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;

namespace ONEE.EAMS.Infrastructure.Data;

/// <summary>
/// Adapts AppDbContext to IAppDbContext for DI / testability.
/// </summary>
public class AppDbContextAdapter : IAppDbContext
{
    private readonly AppDbContext _ctx;
    public AppDbContextAdapter(AppDbContext ctx) => _ctx = ctx;

    public DbSet<User> Users => _ctx.Users;
    public DbSet<ServiceEntity> Services => _ctx.Services;
    public DbSet<Categorie> Categories => _ctx.Categories;
    public DbSet<Equipement> Equipements => _ctx.Equipements;
    public DbSet<EquipementDocument> EquipementDocuments => _ctx.EquipementDocuments;
    public DbSet<EquipementPhoto> EquipementPhotos => _ctx.EquipementPhotos;
    public DbSet<TechnicienEquipement> TechnicienEquipements => _ctx.TechnicienEquipements;
    public DbSet<Maintenance> Maintenances => _ctx.Maintenances;
    public DbSet<HistoriqueEntry> HistoriqueEntries => _ctx.HistoriqueEntries;
    public DbSet<Notification> Notifications => _ctx.Notifications;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _ctx.SaveChangesAsync(cancellationToken);
}
