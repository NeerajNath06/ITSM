using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.CreateRole;

public sealed record CreateRoleCommand(Guid TenantId, string Name, string? Description) : IRequest<CreateRoleResult>;

public sealed record CreateRoleResult(Guid RoleId);
