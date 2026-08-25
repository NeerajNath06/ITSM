using ITSM.BuildingBlocks.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Infrastructure.Persistence;

/// <summary>
/// Used only by `dotnet ef migrations add` / `database update` tooling to
/// build the model outside a running Host.Api — no DI container, no real
/// tenant. Migrations only diff schema shape; they never execute the query
/// filter, so a tenant context that always throws is safe here.
/// </summary>
public sealed class PlatformDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        // Overridable so `dotnet ef` works against whatever port
        // deploy/docker-compose.yml happens to map postgres to locally,
        // without hardcoding a value that will inevitably drift from it.
        var connectionString = Environment.GetEnvironmentVariable("ITSM_PLATFORM_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=itsm;Username=itsm;Password=itsm_dev_only";

        var optionsBuilder = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new PlatformDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid TenantId => throw new InvalidOperationException("No tenant context at design time.");

        public bool IsResolved => false;
    }
}
