using Microsoft.EntityFrameworkCore;
using Platform.Application.Identity;
using Platform.Domain.Identity;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class IdentityGroupRepository(PlatformDbContext context) : IIdentityGroupRepository
{
    public Task<IdentityGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.IdentityGroups.SingleOrDefaultAsync(g => g.Id == id, cancellationToken);

    public Task<IdentityGroup?> GetByNameAsync(string name, CancellationToken cancellationToken) =>
        context.IdentityGroups.SingleOrDefaultAsync(g => g.Name == name, cancellationToken);

    public void Add(IdentityGroup group) => context.IdentityGroups.Add(group);
}
