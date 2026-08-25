using Platform.Domain.Tenancy;

namespace Platform.Application.Tenancy;

/// <summary>
/// No explicit tenant id parameter — every query is scoped by the ambient
/// ITenantContext through EF Core's global query filter (and backstopped by
/// PostgreSQL RLS), not by callers remembering to filter manually
/// (docs/architecture/multi-tenancy.md). For a platform-admin request the
/// filter's tenant is whichever tenant the request's <c>?tenantId=</c>
/// resolved to, set by middleware before this repository ever runs.
/// </summary>
public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Organization?> GetByCodeAsync(string code, CancellationToken cancellationToken);

    void Add(Organization organization);
}
