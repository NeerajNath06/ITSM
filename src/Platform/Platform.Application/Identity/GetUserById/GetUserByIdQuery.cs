using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<IdentityUserDto>;

public sealed record IdentityUserDto(
    Guid Id,
    string Username,
    string Email,
    string DisplayName,
    string Status,
    bool MfaEnrolled,
    DateTimeOffset? LastLoginAt,
    DateTimeOffset CreatedAt);
