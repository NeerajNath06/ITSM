using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.AddGroupMember;

public sealed record AddGroupMemberCommand(Guid TenantId, Guid GroupId, Guid UserId) : IRequest<Unit>;
