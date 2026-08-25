using Platform.Domain.Acl;

namespace Platform.Application.Acl;

public interface IAclRuleRepository
{
    Task<AclRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void Add(AclRule rule);
}
