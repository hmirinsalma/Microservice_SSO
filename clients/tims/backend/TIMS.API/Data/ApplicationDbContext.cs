using Microsoft.EntityFrameworkCore;
using TIMS.API.Entities;

namespace TIMS.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ── Tables métier ─────────────────────────────────────────────────────────
    public DbSet<User>                 Users                  => Set<User>();
    public DbSet<Role>                 Roles                  => Set<Role>();
    public DbSet<UserRole>             UserRoles              => Set<UserRole>();
    public DbSet<Entities.Service>     Services               => Set<Entities.Service>();
    public DbSet<Equipe>               Equipes                => Set<Equipe>();
    public DbSet<Intervention>         Interventions          => Set<Intervention>();
    public DbSet<InterventionHistory>  InterventionHistories  => Set<InterventionHistory>();
    public DbSet<Comment>              Comments               => Set<Comment>();
    public DbSet<Attachment>           Attachments            => Set<Attachment>();
    public DbSet<Notification>         Notifications          => Set<Notification>();

    // ── Table temporaire SSO Stub ─────────────────────────────────────────────
    /// <summary>
    /// ⚠️ TEMPORAIRE — Supprimer lors de l'intégration SSO.
    /// Accès uniquement depuis StubAuthService.
    /// </summary>
    public DbSet<StubCredentials> StubCredentials => Set<StubCredentials>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── UserRole (composite key) ───────────────────────────────────────────
        mb.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        mb.Entity<UserRole>().HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
        mb.Entity<UserRole>().HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);

        // ── User ──────────────────────────────────────────────────────────────
        mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<User>().HasIndex(u => u.SsoId);  // Index pour lookup futur SSO
        mb.Entity<User>().HasOne(u => u.Service).WithMany(s => s.Users).HasForeignKey(u => u.ServiceId).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<User>().HasOne(u => u.Equipe).WithMany(e => e.Members).HasForeignKey(u => u.EquipeId).OnDelete(DeleteBehavior.NoAction);

        // ── Equipe ────────────────────────────────────────────────────────────
        mb.Entity<Equipe>().HasOne(e => e.Service).WithMany(s => s.Equipes).HasForeignKey(e => e.ServiceId).OnDelete(DeleteBehavior.Restrict);

        // ── StubCredentials (isolation volontaire) ────────────────────────────
        mb.Entity<StubCredentials>().HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<StubCredentials>().HasIndex(c => c.Email).IsUnique();

        // ── Intervention ──────────────────────────────────────────────────────
        mb.Entity<Intervention>().HasOne(i => i.Responsable).WithMany(u => u.InterventionsAsResponsable).HasForeignKey(i => i.ResponsableId).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<Intervention>().HasOne(i => i.ChefService).WithMany(u => u.InterventionsAsChefService).HasForeignKey(i => i.ChefServiceId).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<Intervention>().HasOne(i => i.Technicien).WithMany(u => u.InterventionsAsTechnicien).HasForeignKey(i => i.TechnicienId).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<Intervention>().HasOne(i => i.CreatedBy).WithMany().HasForeignKey(i => i.CreatedById).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<Intervention>().HasOne(i => i.Equipe).WithMany(e => e.Interventions).HasForeignKey(i => i.EquipeId).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<Intervention>().HasOne(i => i.Service).WithMany(s => s.Interventions).HasForeignKey(i => i.ServiceId).OnDelete(DeleteBehavior.NoAction);

        // ── InterventionHistory ───────────────────────────────────────────────
        mb.Entity<InterventionHistory>().HasOne(h => h.Intervention).WithMany(i => i.History).HasForeignKey(h => h.InterventionId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<InterventionHistory>().HasOne(h => h.Author).WithMany().HasForeignKey(h => h.AuthorId).OnDelete(DeleteBehavior.NoAction);

        // ── Comment ───────────────────────────────────────────────────────────
        mb.Entity<Comment>().HasOne(c => c.Intervention).WithMany(i => i.Comments).HasForeignKey(c => c.InterventionId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<Comment>().HasOne(c => c.Author).WithMany().HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.NoAction);

        // ── Attachment ────────────────────────────────────────────────────────
        mb.Entity<Attachment>().HasOne(a => a.Intervention).WithMany(i => i.Attachments).HasForeignKey(a => a.InterventionId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<Attachment>().HasOne(a => a.UploadedBy).WithMany().HasForeignKey(a => a.UploadedById).OnDelete(DeleteBehavior.NoAction);

        // ── Notification ──────────────────────────────────────────────────────
        mb.Entity<Notification>().HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.NoAction);
        mb.Entity<Notification>().HasOne(n => n.Intervention).WithMany(i => i.Notifications).HasForeignKey(n => n.InterventionId).OnDelete(DeleteBehavior.NoAction);

        // ── Seed Roles ────────────────────────────────────────────────────────
        mb.Entity<Role>().HasData(
            new Role { Id = 1, Name = RoleNames.AdminTechnique,    Description = "Accès complet" },
            new Role { Id = 2, Name = RoleNames.DirecteurTechnique,Description = "Lecture globale" },
            new Role { Id = 3, Name = RoleNames.ChefService,       Description = "Accès à son service" },
            new Role { Id = 4, Name = RoleNames.Technicien,        Description = "Accès à ses interventions" }
        );
    }
}
