using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Identity;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class IdentityGroupMemberConfiguration : IEntityTypeConfiguration<IdentityGroupMember>
{
    public void Configure(EntityTypeBuilder<IdentityGroupMember> builder)
    {
        builder.ToTable("identity_group_member");

        builder.HasKey(m => new { m.GroupId, m.UserId });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<IdentityGroup>()
            .WithMany()
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
