using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Acl;
using Platform.Application.Common;
using Platform.Application.Identity;
using Platform.Application.Tenancy;
using Platform.Infrastructure.Persistence.Repositories;

namespace Platform.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="PlatformDbContext"/> and every repository/unit-of-work
    /// implementation. Does NOT register <see cref="ITSM.BuildingBlocks.Tenancy.ITenantContext"/> —
    /// that's added by the tenancy-specific registration in a later milestone,
    /// since it depends on HTTP context that isn't this method's concern.
    /// </summary>
    public static IServiceCollection AddPlatformPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PlatformDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();
        services.AddScoped<IIdentityGroupRepository, IdentityGroupRepository>();
        services.AddScoped<IIdentityGroupMemberRepository, IdentityGroupMemberRepository>();
        services.AddScoped<IIdentityRoleRepository, IdentityRoleRepository>();
        services.AddScoped<IUserRoleAssignmentRepository, UserRoleAssignmentRepository>();
        services.AddScoped<IAclRuleRepository, AclRuleRepository>();

        return services;
    }
}
