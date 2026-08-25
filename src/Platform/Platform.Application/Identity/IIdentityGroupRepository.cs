using Platform.Domain.Identity;

namespace Platform.Application.Identity;

public interface IIdentityGroupRepository
{
    Task<IdentityGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IdentityGroup?> GetByNameAsync(string name, CancellationToken cancellationToken);

    void Add(IdentityGroup group);
}
