using ITSM.BuildingBlocks.Domain;

namespace ITSM.BuildingBlocks.Tenancy;

/// <summary>
/// Base for every entity partitioned by tenant. The EF Core tenant query
/// filter (docs/architecture/multi-tenancy.md) is applied once, here, at
/// the shared base type, so every derived entity inherits it automatically
/// instead of each module re-deriving its own <c>TenantId</c> + filter
/// wiring. Every table backing a subtype of this class must carry a
/// matching PostgreSQL Row-Level Security policy in the same migration
/// that introduces its <c>tenant_id</c> column (docs/rules/database-rules.md).
/// </summary>
public abstract class TenantScopedEntity : Entity, ITenantScoped
{
    public Guid TenantId { get; protected init; }
}
