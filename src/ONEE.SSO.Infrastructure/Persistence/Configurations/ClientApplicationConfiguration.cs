using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Configurations;

public class ClientApplicationConfiguration : IEntityTypeConfiguration<ClientApplication>
{
    public void Configure(EntityTypeBuilder<ClientApplication> builder)
    {
        builder.ToTable("ClientApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ClientId)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.ClientId)
            .IsUnique();

        builder.Property(x => x.ClientSecret)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.RedirectUri)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(x => x.PostLogoutRedirectUri)
            .HasMaxLength(500);

        builder.Property(x => x.AllowedScopes)
            .HasMaxLength(1000);

        builder.Property(x => x.AllowedGrantTypes)
            .HasMaxLength(500);

        builder.Property(x => x.RequirePkce)
            .IsRequired();

        builder.Property(x => x.RequireConsent)
            .IsRequired();

        builder.Property(x => x.AccessTokenLifetime)
            .IsRequired();

        builder.Property(x => x.RefreshTokenLifetime)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(c => c.Roles)
            .WithOne(r => r.Client)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Permissions)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}