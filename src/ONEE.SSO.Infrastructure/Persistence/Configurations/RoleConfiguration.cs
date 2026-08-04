using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(r => new { r.ClientId, r.Name })
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsSystemRole)
            .IsRequired();

        builder.Property(r => r.ClientId)
           .IsRequired();

        builder.HasOne(r => r.Client)
           .WithMany(c => c.Roles)
           .HasForeignKey(r => r.ClientId)
           .OnDelete(DeleteBehavior.Restrict);
       
    }
}