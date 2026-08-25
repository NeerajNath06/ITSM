namespace ITSM.BuildingBlocks.Tenancy;

/// <summary>
/// Resolves the current request's tenant. Implementations MUST fail closed:
/// throw rather than return a default/empty tenant when resolution fails.
/// See docs/architecture/multi-tenancy.md — this is the application-layer
/// half of the tenant isolation wall (PostgreSQL Row-Level Security is the
/// other half).
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }

    bool IsResolved { get; }
}
