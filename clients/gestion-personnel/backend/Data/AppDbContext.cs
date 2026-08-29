using GestionPersonnel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>            Users            => Set<User>();
    public DbSet<Role>            Roles            => Set<Role>();
    public DbSet<Employe>         Employes         => Set<Employe>();
    public DbSet<Direction>       Directions       => Set<Direction>();
    public DbSet<Service>         Services         => Set<Service>();
    public DbSet<Conge>           Conges           => Set<Conge>();
    /// <summary>Table temporaire stub — supprimée lors de l'intégration SSO</summary>
    public DbSet<StubCredential>  StubCredentials  => Set<StubCredential>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        base.OnModelCreating(m);

        // ── Role ──────────────────────────────────────────────
        m.Entity<Role>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Nom).IsRequired().HasMaxLength(100);
            e.HasIndex(r => r.Nom).IsUnique();
        });

        // ── User ──────────────────────────────────────────────
        m.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.SsoId).HasMaxLength(256);
            e.HasIndex(u => u.SsoId);
            e.Property(u => u.Username).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.HasOne(u => u.Role).WithMany(r => r.Users)
             .HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── StubCredential (table temporaire — supprimée avec SSO) ──
        m.Entity<StubCredential>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.User).WithMany()
             .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => s.UserId).IsUnique();
        });

        // ── Direction ─────────────────────────────────────────
        m.Entity<Direction>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Nom).IsRequired().HasMaxLength(200);
            e.Property(d => d.Description).HasMaxLength(500);
            e.HasIndex(d => d.Nom).IsUnique();
        });

        // ── Service ───────────────────────────────────────────
        m.Entity<Service>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Nom).IsRequired().HasMaxLength(200);
            e.Property(s => s.Description).HasMaxLength(500);
            e.HasOne(s => s.Direction).WithMany(d => d.Services)
             .HasForeignKey(s => s.DirectionId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Employe ───────────────────────────────────────────
        m.Entity<Employe>(e =>
        {
            e.HasKey(emp => emp.Id);
            e.Property(emp => emp.Matricule).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Nom).IsRequired().HasMaxLength(100);
            e.Property(emp => emp.Prenom).IsRequired().HasMaxLength(100);
            e.Property(emp => emp.Email).IsRequired().HasMaxLength(200);
            e.Property(emp => emp.Telephone).HasMaxLength(20);
            e.Property(emp => emp.Adresse).HasMaxLength(500);
            e.Property(emp => emp.PhotoUrl).HasMaxLength(1000);
            e.Property(emp => emp.Poste).IsRequired().HasMaxLength(100);
            e.Property(emp => emp.Statut).HasConversion<string>();
            e.HasIndex(emp => emp.Matricule).IsUnique();
            e.HasIndex(emp => emp.Email).IsUnique();

            // Direction
            e.HasOne(emp => emp.Direction).WithMany(d => d.Employes)
             .HasForeignKey(emp => emp.DirectionId).OnDelete(DeleteBehavior.Restrict);

            // Service
            e.HasOne(emp => emp.Service).WithMany(s => s.Employes)
             .HasForeignKey(emp => emp.ServiceId).OnDelete(DeleteBehavior.Restrict);

            // Lien User ↔ Employe (1-1 optionnel)
            e.HasOne(emp => emp.User).WithOne(u => u.Employe)
             .HasForeignKey<Employe>(emp => emp.UserId)
             .IsRequired(false).OnDelete(DeleteBehavior.SetNull);

            // Responsable hiérarchique (auto-référence)
            e.HasOne(emp => emp.Responsable).WithMany(emp => emp.Subordonnes)
             .HasForeignKey(emp => emp.ResponsableId)
             .IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        // ── Conge ─────────────────────────────────────────────
        m.Entity<Conge>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Motif).IsRequired().HasMaxLength(500);
            e.Property(c => c.CommentaireChef).HasMaxLength(500);
            e.Property(c => c.CommentaireDirecteur).HasMaxLength(500);
            e.Property(c => c.Statut).HasConversion<string>();

            e.HasOne(c => c.Employe).WithMany(emp => emp.Conges)
             .HasForeignKey(c => c.EmployeId).OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.ChefService).WithMany()
             .HasForeignKey(c => c.ChefServiceId)
             .IsRequired(false).OnDelete(DeleteBehavior.NoAction);

            e.HasOne(c => c.Directeur).WithMany()
             .HasForeignKey(c => c.DirecteurId)
             .IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        // ── Seeds ─────────────────────────────────────────────
        m.Entity<Role>().HasData(
            new Role { Id = 1, Nom = "AdministrateurRH" },
            new Role { Id = 2, Nom = "Directeur" },
            new Role { Id = 3, Nom = "ChefDeService" },
            new Role { Id = 4, Nom = "Employe" }
        );

        m.Entity<Direction>().HasData(
            new Direction { Id = 1, Nom = "Direction RH",          Description = "Direction des Ressources Humaines" },
            new Direction { Id = 2, Nom = "Direction Technique",   Description = "Direction Technique" },
            new Direction { Id = 3, Nom = "Direction Informatique",Description = "Direction Informatique" },
            new Direction { Id = 4, Nom = "Direction Patrimoine",  Description = "Direction du Patrimoine" }
        );

        m.Entity<Service>().HasData(
            new Service { Id = 1, Nom = "Service Recrutement",   Description = "Recrutement des talents",     DirectionId = 1 },
            new Service { Id = 2, Nom = "Service Formation",     Description = "Formation et développement",  DirectionId = 1 },
            new Service { Id = 3, Nom = "Service Paie",          Description = "Gestion de la paie",          DirectionId = 1 },
            new Service { Id = 4, Nom = "Service Maintenance",   Description = "Maintenance technique",       DirectionId = 2 },
            new Service { Id = 5, Nom = "Service Exploitation",  Description = "Exploitation technique",      DirectionId = 2 },
            new Service { Id = 6, Nom = "Service Développement", Description = "Développement logiciel",      DirectionId = 3 },
            new Service { Id = 7, Nom = "Service Infrastructure",Description = "Infrastructure IT",           DirectionId = 3 },
            new Service { Id = 8, Nom = "Service Immobilier",    Description = "Gestion immobilière",         DirectionId = 4 },
            new Service { Id = 9, Nom = "Service Logistique",    Description = "Logistique et achats",        DirectionId = 4 }
        );
    }
}
