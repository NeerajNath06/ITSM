using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.RevokeUserRole;

public sealed record RevokeUserRoleCommand(Guid UserId, Guid RoleId) : IRequest<Unit>;
