namespace Platform.Application.Common;

/// <summary>
/// The one commit boundary per command handler (docs/rules/database-rules.md:
/// "one SaveChangesAsync call per command handler"). Implemented in
/// Platform.Infrastructure as a thin wrapper around PlatformDbContext —
/// Application can't reference the DbContext directly without pulling in
/// EF Core, which the layering Architecture.Tests forbid here.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
