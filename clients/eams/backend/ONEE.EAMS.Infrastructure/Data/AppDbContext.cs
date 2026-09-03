using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Domain.Entities;

namespace ONEE.EAMS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ServiceEntity> Services => Set<ServiceEntity>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Equipement> Equipements => Set<Equipement>();
    public DbSet<EquipementDocument> EquipementDocuments => Set<EquipementDocument>();
    public DbSet<EquipementPhoto> EquipementPhotos => Set<EquipementPhoto>();
    public DbSet<TechnicienEquipement> TechnicienEquipements => Set<TechnicienEquipement>();
    public DbSet<Maintenance> Maintenances => Set<Maintenance>();
    public DbSet<HistoriqueEntry> HistoriqueEntries => Set<HistoriqueEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ServiceEntity
        modelBuilder.Entity<ServiceEntity>(e =>
        {
            e.ToTable("Services");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nom).IsRequired().HasMaxLength(200);
            e.Property(x => x.Code).IsRequired().HasMaxLength(20);
        });

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Nom).IsRequired().HasMaxLength(100);
            e.Property(x => x.Prenom).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            // PasswordHash supprimé — authentification déléguée au microservice SSO
            e.Property(x => x.SsoId).HasMaxLength(256);
            e.HasIndex(x => x.SsoId);
            e.Property(x => x.Role).HasColumnName("Role").HasMaxLength(100);

            e.HasOne(x => x.Service)
                .WithMany(s => s.Users)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Categorie
        modelBuilder.Entity<Categorie>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nom).IsRequired().HasMaxLength(200);
            e.Property(x => x.Code).IsRequired().HasMaxLength(20);
            e.Property(x => x.Couleur).IsRequired().HasMaxLength(20);
        });

        // Equipement
        modelBuilder.Entity<Equipement>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Reference).IsUnique();
            e.HasIndex(x => x.NumeroSerie).IsUnique();
            e.HasIndex(x => x.ServiceId);
            e.HasIndex(x => x.CategorieId);
            e.HasIndex(x => x.Etat);
            e.Property(x => x.Reference).IsRequired().HasMaxLength(50);
            e.Property(x => x.Nom).IsRequired().HasMaxLength(300);
            e.Property(x => x.Etat).HasConversion<string>();
            e.Property(x => x.ValeurAcquisition).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Categorie)
                .WithMany(c => c.Equipements)
                .HasForeignKey(x => x.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Service)
                .WithMany(s => s.Equipements)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Responsable)
                .WithMany(u => u.EquipementsResponsable)
                .HasForeignKey(x => x.ResponsableId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // EquipementDocument
        modelBuilder.Entity<EquipementDocument>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Equipement)
                .WithMany(eq => eq.Documents)
                .HasForeignKey(x => x.EquipementId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // EquipementPhoto
        modelBuilder.Entity<EquipementPhoto>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Equipement)
                .WithMany(eq => eq.Photos)
                .HasForeignKey(x => x.EquipementId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TechnicienEquipement
        modelBuilder.Entity<TechnicienEquipement>(e =>
        {
            e.HasKey(x => new { x.TechnicienId, x.EquipementId });
            e.HasOne(x => x.Technicien)
                .WithMany(u => u.Affectations)
                .HasForeignKey(x => x.TechnicienId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Equipement)
                .WithMany(eq => eq.Techniciens)
                .HasForeignKey(x => x.EquipementId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Maintenance
        modelBuilder.Entity<Maintenance>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EquipementId);
            e.HasIndex(x => x.TechnicienId);
            e.HasIndex(x => new { x.Statut, x.DatePlanifiee });
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.Statut).HasConversion<string>();
            e.Property(x => x.EtatAvant).HasConversion<string>();
            e.Property(x => x.EtatApres).HasConversion<string>();
            e.Property(x => x.CoutEstime).HasColumnType("decimal(18,2)");
            e.Property(x => x.CoutReel).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Equipement)
                .WithMany(eq => eq.Maintenances)
                .HasForeignKey(x => x.EquipementId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Technicien)
                .WithMany(u => u.Maintenances)
                .HasForeignKey(x => x.TechnicienId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // HistoriqueEntry
        modelBuilder.Entity<HistoriqueEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.EntiteId, x.HorodatageUtc });
            e.Property(x => x.EntiteType).IsRequired().HasMaxLength(100);
            e.Property(x => x.TypeEvenement).IsRequired().HasMaxLength(100);

            e.HasOne(x => x.Auteur)
                .WithMany()
                .HasForeignKey(x => x.AuteurId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Equipement)
                .WithMany(eq => eq.Historique)
                .HasForeignKey(x => x.EntiteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.DestinataireId);
            e.HasIndex(x => new { x.DestinataireId, x.EstLue });
            e.HasIndex(x => new { x.TypeEvenement, x.RessourceId, x.CreatedAt });
            e.Property(x => x.TypeEvenement).IsRequired().HasMaxLength(100);
            e.Property(x => x.Message).IsRequired().HasMaxLength(500);

            e.HasOne(x => x.Destinataire)
                .WithMany(u => u.Notifications)
                .HasForeignKey(x => x.DestinataireId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
