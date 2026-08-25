using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenant", t => t.HasCheckConstraint(
            "ck_tenant_isolation_tier", "isolation_tier IN ('shared', 'dedicated')"));
        builder.ToTable(t => t.HasCheckConstraint(
            "ck_tenant_status", "status IN ('provisioning', 'active', 'suspended', 'deprovisioned')"));

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Slug).IsRequired().HasMaxLength(100);
        builder.Property(t => t.ConnectionStringSecretRef).HasMaxLength(500);

        builder.Property(t => t.IsolationTier)
            .HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<TenantIsolationTier>(v, ignoreCase: true))
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Status)
            .HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<TenantStatus>(v, ignoreCase: true))
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => t.Slug).IsUnique();

        // Partial unique index — enforces "exactly one platform tenant" at
        // the database level, not just by convention.
        builder.HasIndex(t => t.IsPlatform)
            .IsUnique()
            .HasFilter("is_platform = true")
            .HasDatabaseName("ix_tenant_is_platform_singleton");
    }
}
