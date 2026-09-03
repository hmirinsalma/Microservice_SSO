using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Configurations;

public class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
{
    public void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        builder.ToTable("UserConsents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClientId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Scopes)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.GrantedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(50);

        // Index composite pour recherche rapide
        builder.HasIndex(x => new { x.UserId, x.ClientId })
            .IsUnique();

        // Relation avec User
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
