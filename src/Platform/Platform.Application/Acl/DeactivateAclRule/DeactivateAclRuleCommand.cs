using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Acl.DeactivateAclRule;

public sealed record DeactivateAclRuleCommand(Guid AclRuleId) : IRequest<Unit>;
