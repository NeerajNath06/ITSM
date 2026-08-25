using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.GetUserById;

public sealed class GetUserByIdHandler(IIdentityUserRepository users) : IRequestHandler<GetUserByIdQuery, IdentityUserDto>
{
    public async Task<IdentityUserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        return new IdentityUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.Status.ToString(),
            user.MfaEnrolled,
            user.LastLoginAt,
            user.CreatedAt);
    }
}
