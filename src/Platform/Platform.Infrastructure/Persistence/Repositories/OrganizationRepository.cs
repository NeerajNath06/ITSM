using Microsoft.EntityFrameworkCore;
using Platform.Application.Tenancy;
using Platform.Domain.Tenancy;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository(PlatformDbContext context) : IOrganizationRepository
{
    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Organizations.SingleOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<Organization?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        context.Organizations.SingleOrDefaultAsync(o => o.Code == code, cancellationToken);

    public void Add(Organization organization) => context.Organizations.Add(organization);
}
