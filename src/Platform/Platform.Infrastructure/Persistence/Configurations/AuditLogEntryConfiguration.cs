using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Audit;

namespace Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Maps AuditLogEntry for LINQ query translation, but the physical table is
/// entirely hand-written raw SQL in its own migration (PARTITION BY RANGE
/// isn't expressible via EF's declarative migration API, and the table
/// needs a REVOKE that has no Fluent API equivalent either) —
/// ExcludeFromMigrations stops EF from ever trying to generate its own
/// CreateTable/AlterTable for it.
/// </summary>
public sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("audit_log", t => t.ExcludeFromMigrations());

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action).IsRequired().HasMaxLength(200).HasColumnName("action");
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100).HasColumnName("entity_type");
        builder.Property(a => a.OldValueJson).HasColumnType("jsonb").HasColumnName("old_value");
        builder.Property(a => a.NewValueJson).HasColumnType("jsonb").HasColumnName("new_value");
        builder.Property(a => a.ReasonCode).HasMaxLength(100).HasColumnName("reason_code");
        builder.Property(a => a.SourceIp).HasColumnType("inet").HasColumnName("source_ip");
        builder.Property(a => a.Channel).IsRequired().HasMaxLength(50).HasColumnName("channel");
        builder.Property(a => a.Service).IsRequired().HasMaxLength(100).HasColumnName("service");

        // No FK constraints — matches the verbatim schema in
        // docs/audit/audit-architecture.md. An audit row must remain
        // readable even after the entity/actor it references is gone.
    }
}
