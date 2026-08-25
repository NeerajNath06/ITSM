namespace ITSM.BuildingBlocks.Tenancy;

/// <summary>
/// Implemented by every entity partitioned by tenant, including composite-key
/// junction/grant tables that can't derive from <see cref="TenantScopedEntity"/>
/// (no synthetic <c>Id</c>). EF Core configuration applies the tenant query
/// filter to every type implementing this interface, not just subclasses of
/// <see cref="TenantScopedEntity"/>, so junction tables get the same filter
/// without a special case.
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; }
}
