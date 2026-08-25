using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Identity;

namespace Platform.Application.Identity.GrantUserRole;

public sealed class GrantUserRoleHandler(
    IIdentityUserRepository users,
    IIdentityRoleRepository roles,
    IUserRoleAssignmentRepository assignments,
    IUnitOfWork unitOfWork) : IRequestHandler<GrantUserRoleCommand, Unit>
{
    public async Task<Unit> Handle(GrantUserRoleCommand request, CancellationToken cancellationToken)
    {
        _ = await users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        _ = await roles.GetByIdAsync(request.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role {request.RoleId} was not found.");

        if (await assignments.FindAsync(request.UserId, request.RoleId, cancellationToken) is not null)
        {
            return Unit.Value;
        }

        assignments.Add(UserRoleAssignment.Grant(request.TenantId, request.UserId, request.RoleId, request.GrantedByUserId));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
