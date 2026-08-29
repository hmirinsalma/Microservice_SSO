using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Domain.Entities;

namespace ONEE.EAMS.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<ServiceEntity> Services { get; }
    DbSet<Categorie> Categories { get; }
    DbSet<Equipement> Equipements { get; }
    DbSet<EquipementDocument> EquipementDocuments { get; }
    DbSet<EquipementPhoto> EquipementPhotos { get; }
    DbSet<TechnicienEquipement> TechnicienEquipements { get; }
    DbSet<Maintenance> Maintenances { get; }
    DbSet<HistoriqueEntry> HistoriqueEntries { get; }
    DbSet<Notification> Notifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
