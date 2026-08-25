using ITSM.BuildingBlocks.Messaging;
using Platform.Domain.Acl;

namespace Platform.Application.Acl.CreateAclRule;

public sealed record CreateAclRuleCommand(
    Guid TenantId,
    string TableName,
    AclOperation Operation,
    Guid RequiredRoleId,
    AclEffect Effect,
    string? FieldName,
    string? ConditionExpression) : IRequest<CreateAclRuleResult>;

public sealed record CreateAclRuleResult(Guid AclRuleId);
