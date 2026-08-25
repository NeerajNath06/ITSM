using Platform.Domain.Identity;

namespace Platform.Application.Identity;

public interface IIdentityUserRepository
{
    Task<IdentityUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IdentityUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    void Add(IdentityUser user);
}
