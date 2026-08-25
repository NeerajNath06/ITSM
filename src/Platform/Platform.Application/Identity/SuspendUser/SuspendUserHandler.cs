using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Identity.SuspendUser;

public sealed class SuspendUserHandler(IIdentityUserRepository users, IUnitOfWork unitOfWork)
    : IRequestHandler<SuspendUserCommand, Unit>
{
    public async Task<Unit> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        user.Suspend();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
