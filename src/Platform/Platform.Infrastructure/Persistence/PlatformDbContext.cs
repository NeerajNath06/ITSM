using System.Reflection;
using ITSM.BuildingBlocks.Tenancy;
using Microsoft.EntityFrameworkCore;
using Platform.Domain.Acl;
using Platform.Domain.Audit;
using Platform.Domain.Identity;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence;

/// <summary>
/// The one shared DbContext for Identity+Tenancy+ACL+Audit (Phase 2 design
/// decision — see the phase plan's "DbContext granularity" entry). Every
/// entity implementing <see cref="ITenantScoped"/> gets the tenant query
/// filter applied generically here, not per entity, so a new tenant-scoped
/// type can never accidentally ship without it.
/// </summary>
public sealed class PlatformDbContext(DbContextOptions<PlatformDbContext> options, ITenantContext tenantContext)
    : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<IdentityUser> IdentityUsers => Set<IdentityUser>();
    public DbSet<IdentityGroup> IdentityGroups => Set<IdentityGroup>();
    public DbSet<IdentityGroupMember> IdentityGroupMembers => Set<IdentityGroupMember>();
    public DbSet<IdentityRole> IdentityRoles => Set<IdentityRole>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();
    public DbSet<AclRule> AclRules => Set<AclRule>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlatformDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                ApplyTenantFilterMethod.MakeGenericMethod(entityType.ClrType).Invoke(this, [modelBuilder]);
            }
        }
    }

    private static readonly MethodInfo ApplyTenantFilterMethod = typeof(PlatformDbContext)
        .GetMethod(nameof(ApplyTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// Fail-closed by construction: if <see cref="ITenantContext.TenantId"/>
    /// throws because the tenant is unresolved, the filter predicate itself
    /// throws when evaluated — no tenant-scoped query can silently run
    /// unscoped (docs/architecture/multi-tenancy.md).
    /// </summary>
    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantScoped =>
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
}
