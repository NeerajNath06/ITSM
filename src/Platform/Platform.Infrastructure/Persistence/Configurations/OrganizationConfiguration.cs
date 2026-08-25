using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organization", t => t.HasCheckConstraint(
            "ck_organization_status", "status IN ('active', 'retired')"));

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Code).IsRequired().HasMaxLength(50);

        builder.Property(o => o.Status)
            .HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<OrganizationStatus>(v, ignoreCase: true))
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(o => new { o.TenantId, o.Code }).IsUnique();
        builder.HasIndex(o => o.TenantId);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(o => o.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(o => o.ParentOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
