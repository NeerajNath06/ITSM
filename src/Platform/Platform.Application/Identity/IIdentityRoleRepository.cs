using Platform.Domain.Identity;

namespace Platform.Application.Identity;

public interface IIdentityRoleRepository
{
    Task<IdentityRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IdentityRole?> GetByNameAsync(string name, CancellationToken cancellationToken);

    void Add(IdentityRole role);
}
