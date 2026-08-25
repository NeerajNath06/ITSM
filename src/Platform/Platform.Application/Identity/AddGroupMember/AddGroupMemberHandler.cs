using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Identity;

namespace Platform.Application.Identity.AddGroupMember;

public sealed class AddGroupMemberHandler(
    IIdentityGroupRepository groups,
    IIdentityUserRepository users,
    IIdentityGroupMemberRepository members,
    IUnitOfWork unitOfWork) : IRequestHandler<AddGroupMemberCommand, Unit>
{
    public async Task<Unit> Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
    {
        _ = await groups.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group {request.GroupId} was not found.");

        _ = await users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        if (await members.ExistsAsync(request.GroupId, request.UserId, cancellationToken))
        {
            return Unit.Value;
        }

        members.Add(IdentityGroupMember.Create(request.TenantId, request.GroupId, request.UserId));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
