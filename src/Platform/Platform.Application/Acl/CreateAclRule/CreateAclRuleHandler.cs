using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Acl;

namespace Platform.Application.Acl.CreateAclRule;

public sealed class CreateAclRuleHandler(IAclRuleRepository rules, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAclRuleCommand, CreateAclRuleResult>
{
    public async Task<CreateAclRuleResult> Handle(CreateAclRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = AclRule.Create(
            request.TenantId,
            request.TableName,
            request.Operation,
            request.RequiredRoleId,
            request.Effect,
            request.FieldName,
            request.ConditionExpression);

        rules.Add(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAclRuleResult(rule.Id);
    }
}
