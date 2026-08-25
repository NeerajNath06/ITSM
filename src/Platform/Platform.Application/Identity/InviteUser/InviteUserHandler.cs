using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Identity;

namespace Platform.Application.Identity.InviteUser;

public sealed class InviteUserHandler(IIdentityUserRepository users, IUnitOfWork unitOfWork)
    : IRequestHandler<InviteUserCommand, InviteUserResult>
{
    public async Task<InviteUserResult> Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        if (await users.GetByUsernameAsync(request.Username, cancellationToken) is not null)
        {
            throw new ConflictException($"Username '{request.Username}' is already in use in this tenant.");
        }

        if (await users.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            throw new ConflictException($"Email '{request.Email}' is already in use in this tenant.");
        }

        var user = IdentityUser.Invite(
            request.TenantId,
            request.Username,
            request.Email,
            request.DisplayName,
            request.OrganizationId,
            request.IsLocalIdentity);

        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InviteUserResult(user.Id);
    }
}
