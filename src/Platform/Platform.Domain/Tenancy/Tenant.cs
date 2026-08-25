using ITSM.BuildingBlocks.Domain;

namespace Platform.Domain.Tenancy;

/// <summary>
/// Root partition boundary (docs/database/shared-enterprise-data-model.md) —
/// every tenant-scoped table carries a <c>tenant_id</c> pointing here. Not
/// itself tenant-scoped: <see cref="Tenant"/> derives from <see cref="Entity"/>,
/// not <see cref="ITSM.BuildingBlocks.Tenancy.TenantScopedEntity"/>.
/// </summary>
public sealed class Tenant : Entity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public TenantIsolationTier IsolationTier { get; private set; }
    public TenantStatus Status { get; private set; }
    public string? ConnectionStringSecretRef { get; private set; }

    /// <summary>
    /// Marks the single reserved tenant whose identities may hold
    /// <c>platform_admin</c> and reach across tenants via an explicit
    /// <c>?tenantId=</c> parameter (docs/security/security-architecture.md).
    /// Exactly one row may have this set — enforced by a partial unique
    /// index in the migration, not just application code.
    /// </summary>
    public bool IsPlatform { get; private init; }

#pragma warning disable CS8618 // EF Core materialization constructor
    private Tenant()
    {
    }
#pragma warning restore CS8618

    public static Tenant CreateStandard(string name, string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return new Tenant
        {
            Name = name,
            Slug = slug,
            IsolationTier = TenantIsolationTier.Shared,
            Status = TenantStatus.Provisioning,
            IsPlatform = false,
        };
    }

    public static Tenant CreateDedicated(string name, string slug, string connectionStringSecretRef)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionStringSecretRef);

        return new Tenant
        {
            Name = name,
            Slug = slug,
            IsolationTier = TenantIsolationTier.Dedicated,
            Status = TenantStatus.Provisioning,
            ConnectionStringSecretRef = connectionStringSecretRef,
            IsPlatform = false,
        };
    }

    /// <summary>
    /// Used exactly once, by the platform's own seed/migration data, to
    /// create the reserved platform tenant. Not a general-purpose factory —
    /// application code should never call this to create a customer tenant.
    /// </summary>
    public static Tenant CreatePlatformTenant() => new()
    {
        Name = "Platform",
        Slug = "platform",
        IsolationTier = TenantIsolationTier.Shared,
        Status = TenantStatus.Active,
        IsPlatform = true,
    };

    public void MarkProvisioned()
    {
        if (Status != TenantStatus.Provisioning)
        {
            throw new InvalidOperationException($"Tenant {Id} is not in Provisioning status.");
        }

        Status = TenantStatus.Active;
        Raise(new TenantProvisioned(Id, DateTimeOffset.UtcNow));
    }

    public void Suspend()
    {
        if (IsPlatform)
        {
            throw new InvalidOperationException("The platform tenant cannot be suspended.");
        }

        Status = TenantStatus.Suspended;
        Raise(new TenantSuspended(Id, DateTimeOffset.UtcNow));
    }

    public void Reactivate()
    {
        if (Status != TenantStatus.Suspended)
        {
            throw new InvalidOperationException($"Tenant {Id} is not Suspended.");
        }

        Status = TenantStatus.Active;
        Raise(new TenantReactivated(Id, DateTimeOffset.UtcNow));
    }

    public void Deprovision()
    {
        if (IsPlatform)
        {
            throw new InvalidOperationException("The platform tenant cannot be deprovisioned.");
        }

        Status = TenantStatus.Deprovisioned;
    }
}
