using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Identity;

namespace Platform.Application.Identity.CreateGroup;

public sealed class CreateGroupHandler(IIdentityGroupRepository groups, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGroupCommand, CreateGroupResult>
{
    public async Task<CreateGroupResult> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        if (await groups.GetByNameAsync(request.Name, cancellationToken) is not null)
        {
            throw new ConflictException($"A group named '{request.Name}' already exists in this tenant.");
        }

        var group = IdentityGroup.Create(request.TenantId, request.Name, request.Description, request.OrganizationId);
        groups.Add(group);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateGroupResult(group.Id);
    }
}
