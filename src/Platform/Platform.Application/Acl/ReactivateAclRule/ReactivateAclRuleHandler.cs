using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Acl.ReactivateAclRule;

public sealed class ReactivateAclRuleHandler(IAclRuleRepository rules, IUnitOfWork unitOfWork)
    : IRequestHandler<ReactivateAclRuleCommand, Unit>
{
    public async Task<Unit> Handle(ReactivateAclRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await rules.GetByIdAsync(request.AclRuleId, cancellationToken)
            ?? throw new NotFoundException($"ACL rule {request.AclRuleId} was not found.");

        rule.Reactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
