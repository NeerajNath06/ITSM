using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.GrantUserRole;

public sealed record GrantUserRoleCommand(Guid TenantId, Guid UserId, Guid RoleId, Guid? GrantedByUserId) : IRequest<Unit>;
