using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.InviteUser;

public sealed record InviteUserCommand(
    Guid TenantId,
    string Username,
    string Email,
    string DisplayName,
    Guid? OrganizationId,
    bool IsLocalIdentity) : IRequest<InviteUserResult>;

public sealed record InviteUserResult(Guid UserId);
