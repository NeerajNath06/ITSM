namespace Platform.Domain.Tenancy;

/// <summary>
/// Per docs/architecture/multi-tenancy.md: a tenant's isolation tier is a
/// deployment/connection-routing decision, not a different data model —
/// every tenant runs the same schema and migrations regardless of tier.
/// </summary>
public enum TenantIsolationTier
{
    Shared,
    Dedicated,
}
