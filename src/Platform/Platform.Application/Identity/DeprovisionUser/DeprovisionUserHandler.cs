using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Identity.DeprovisionUser;

public sealed class DeprovisionUserHandler(IIdentityUserRepository users, IUnitOfWork unitOfWork)
    : IRequestHandler<DeprovisionUserCommand, Unit>
{
    public async Task<Unit> Handle(DeprovisionUserCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        user.Deprovision();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
