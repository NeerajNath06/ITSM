using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.ReactivateUser;

public sealed record ReactivateUserCommand(Guid UserId) : IRequest<Unit>;
