using Microsoft.EntityFrameworkCore;
using Platform.Application.Identity;
using Platform.Domain.Identity;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class IdentityRoleRepository(PlatformDbContext context) : IIdentityRoleRepository
{
    public Task<IdentityRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.IdentityRoles.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<IdentityRole?> GetByNameAsync(string name, CancellationToken cancellationToken) =>
        context.IdentityRoles.SingleOrDefaultAsync(r => r.Name == name, cancellationToken);

    public void Add(IdentityRole role) => context.IdentityRoles.Add(role);
}
