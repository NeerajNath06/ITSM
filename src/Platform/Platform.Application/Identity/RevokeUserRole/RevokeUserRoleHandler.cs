using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Identity.RevokeUserRole;

public sealed class RevokeUserRoleHandler(IUserRoleAssignmentRepository assignments, IUnitOfWork unitOfWork)
    : IRequestHandler<RevokeUserRoleCommand, Unit>
{
    public async Task<Unit> Handle(RevokeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var assignment = await assignments.FindAsync(request.UserId, request.RoleId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} does not hold role {request.RoleId}.");

        assignment.Revoke();
        assignments.Remove(assignment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
