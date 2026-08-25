using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Identity.ReactivateUser;

public sealed class ReactivateUserHandler(IIdentityUserRepository users, IUnitOfWork unitOfWork)
    : IRequestHandler<ReactivateUserCommand, Unit>
{
    public async Task<Unit> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        user.Reactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
