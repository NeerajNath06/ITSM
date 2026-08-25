using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Identity;

namespace Platform.Application.Identity.CreateRole;

/// <summary>Custom, tenant-defined role — the seeded system roles are created only by CreateTenantHandler.</summary>
public sealed class CreateRoleHandler(IIdentityRoleRepository roles, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoleCommand, CreateRoleResult>
{
    public async Task<CreateRoleResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roles.GetByNameAsync(request.Name, cancellationToken) is not null)
        {
            throw new ConflictException($"A role named '{request.Name}' already exists in this tenant.");
        }

        var role = IdentityRole.CreateCustomRole(request.TenantId, request.Name, request.Description);
        roles.Add(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoleResult(role.Id);
    }
}
