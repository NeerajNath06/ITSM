using Platform.Application.Common;

namespace Platform.Infrastructure.Persistence;

public sealed class UnitOfWork(PlatformDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
