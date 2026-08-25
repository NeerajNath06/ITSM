using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Acl;
using Platform.Domain.Identity;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class AclRuleConfiguration : IEntityTypeConfiguration<AclRule>
{
    public void Configure(EntityTypeBuilder<AclRule> builder)
    {
        builder.ToTable("acl_rule", t => t.HasCheckConstraint(
            "ck_acl_rule_operation", "operation IN ('read', 'write', 'create', 'delete')"));
        builder.ToTable(t => t.HasCheckConstraint("ck_acl_rule_effect", "effect IN ('allow', 'deny')"));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TableName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.FieldName).HasMaxLength(100);
        builder.Property(r => r.ConditionExpression).HasColumnType("jsonb");

        builder.Property(r => r.Operation)
            .HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<AclOperation>(v, ignoreCase: true))
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.Effect)
            .HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<AclEffect>(v, ignoreCase: true))
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(r => new { r.TenantId, r.TableName, r.Operation });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<IdentityRole>()
            .WithMany()
            .HasForeignKey(r => r.RequiredRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
