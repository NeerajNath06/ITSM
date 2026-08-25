using Microsoft.EntityFrameworkCore;
using Platform.Application.Acl;
using Platform.Domain.Acl;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class AclRuleRepository(PlatformDbContext context) : IAclRuleRepository
{
    public Task<AclRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.AclRules.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

    public void Add(AclRule rule) => context.AclRules.Add(rule);
}
