using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.CreateGroup;

public sealed record CreateGroupCommand(
    Guid TenantId,
    string Name,
    string? Description,
    Guid? OrganizationId) : IRequest<CreateGroupResult>;

public sealed record CreateGroupResult(Guid GroupId);
