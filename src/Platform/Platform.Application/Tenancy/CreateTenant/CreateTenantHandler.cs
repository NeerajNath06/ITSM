using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Application.Identity;
using Platform.Domain.Identity;
using Platform.Domain.Tenancy;

namespace Platform.Application.Tenancy.CreateTenant;

/// <summary>
/// Creates the tenant row and seeds its default roles in one transaction.
/// The full provisioning saga in docs/architecture/multi-tenancy.md (apply
/// RLS if missing, seed metadata, provision a dedicated database) needs a
/// resumable orchestrator this repo doesn't have yet (no Temporal wiring —
/// ADR-023 — exists as of Phase 2); role-seeding is simple and atomic
/// enough to do inline here instead of standing up saga machinery for one
/// step. <see cref="Tenant.MarkProvisioned"/> is called separately once
/// whatever provisioning steps a given tier needs are confirmed complete.
/// </summary>
public sealed class CreateTenantHandler(ITenantRepository tenants, IIdentityRoleRepository roles, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTenantCommand, CreateTenantResult>
{
    public async Task<CreateTenantResult> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        if (await tenants.GetBySlugAsync(request.Slug, cancellationToken) is not null)
        {
            throw new ConflictException($"A tenant with slug '{request.Slug}' already exists.");
        }

        var tenant = request.IsDedicated
            ? Tenant.CreateDedicated(request.Name, request.Slug, request.ConnectionStringSecretRef!)
            : Tenant.CreateStandard(request.Name, request.Slug);

        tenants.Add(tenant);

        foreach (var roleName in IdentityRole.DefaultTenantRoleNames)
        {
            roles.Add(IdentityRole.CreateSystemRole(tenant.Id, roleName));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTenantResult(tenant.Id);
    }
}
