using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Identity;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        builder.ToTable("identity_user", t => t.HasCheckConstraint(
            "ck_identity_user_status", "status IN ('invited', 'active', 'suspended', 'deprovisioned')"));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.ExternalSubject).HasMaxLength(200);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasColumnType("citext").HasMaxLength(320);
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(200);

        builder.Property(u => u.Status)
            .HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<IdentityUserStatus>(v, ignoreCase: true))
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(u => new { u.TenantId, u.Username }).IsUnique();
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
        builder.HasIndex(u => new { u.TenantId, u.ExternalSubject })
            .IsUnique()
            .HasFilter("external_subject IS NOT NULL");

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
