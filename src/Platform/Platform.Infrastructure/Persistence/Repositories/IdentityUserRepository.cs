using Microsoft.EntityFrameworkCore;
using Platform.Application.Identity;
using Platform.Domain.Identity;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class IdentityUserRepository(PlatformDbContext context) : IIdentityUserRepository
{
    public Task<IdentityUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.IdentityUsers.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<IdentityUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken) =>
        context.IdentityUsers.SingleOrDefaultAsync(u => u.Username == username, cancellationToken);

    public Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        context.IdentityUsers.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

    public void Add(IdentityUser user) => context.IdentityUsers.Add(user);
}
