using Platform.Domain.Tenancy;

namespace Platform.Application.Tenancy;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    void Add(Tenant tenant);
}
