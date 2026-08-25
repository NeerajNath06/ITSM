using Microsoft.EntityFrameworkCore;
using Platform.Application.Tenancy;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(PlatformDbContext context) : ITenantRepository
{
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Tenants.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken) =>
        context.Tenants.SingleOrDefaultAsync(t => t.Slug == slug, cancellationToken);

    public void Add(Tenant tenant) => context.Tenants.Add(tenant);
}
