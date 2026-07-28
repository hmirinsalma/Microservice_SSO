using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SessionId)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.SessionId)
            .IsUnique();

        builder.Property(x => x.Device)
            .HasMaxLength(200);

        builder.Property(x => x.Browser)
            .HasMaxLength(100);

        builder.Property(x => x.OperatingSystem)
            .HasMaxLength(100);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(50);

        builder.Property(x => x.LoginAt)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserSessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}