using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Acl.ReactivateAclRule;

public sealed record ReactivateAclRuleCommand(Guid AclRuleId) : IRequest<Unit>;
