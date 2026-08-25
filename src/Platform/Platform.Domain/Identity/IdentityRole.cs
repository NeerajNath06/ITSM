using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Identity;

/// <summary>
/// A grantable role, tenant-scoped and seeded per tenant during provisioning
/// (docs/architecture/multi-tenancy.md's provisioning saga: "seed default
/// metadata/roles") rather than drawn from one global catalog — keeps RLS
/// uniform across every table and lets a tenant_admin extend roles without
/// cross-tenant blast radius. Seed set per standard tenant:
/// itil_agent, cmdb_admin, security_admin, catalog_admin, tenant_admin,
/// api_integration (docs/security/security-architecture.md). platform_admin
/// exists only in the reserved platform tenant.
/// </summary>
public sealed class IdentityRole : TenantScopedEntity
{
    /// <summary>Seeded into every newly created standard tenant. See class remarks.</summary>
    public static readonly IReadOnlyList<string> DefaultTenantRoleNames =
    [
        "itil_agent",
        "cmdb_admin",
        "security_admin",
        "catalog_admin",
        "tenant_admin",
        "api_integration",
    ];

    /// <summary>Exists only in the reserved platform tenant — never seeded into a standard tenant.</summary>
    public const string PlatformAdminRoleName = "platform_admin";

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }

#pragma warning disable CS8618
    private IdentityRole()
    {
    }
#pragma warning restore CS8618

    public static IdentityRole CreateSystemRole(Guid tenantId, string name, string? description = null) =>
        new()
        {
            TenantId = tenantId,
            Name = name,
            Description = description,
            IsSystemRole = true,
        };

    public static IdentityRole CreateCustomRole(Guid tenantId, string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new IdentityRole
        {
            TenantId = tenantId,
            Name = name,
            Description = description,
            IsSystemRole = false,
        };
    }
}
