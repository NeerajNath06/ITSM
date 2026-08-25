using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Acl.DeactivateAclRule;

public sealed class DeactivateAclRuleHandler(IAclRuleRepository rules, IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateAclRuleCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateAclRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await rules.GetByIdAsync(request.AclRuleId, cancellationToken)
            ?? throw new NotFoundException($"ACL rule {request.AclRuleId} was not found.");

        rule.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
