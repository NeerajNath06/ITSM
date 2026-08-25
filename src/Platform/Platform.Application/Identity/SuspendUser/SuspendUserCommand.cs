using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.SuspendUser;

public sealed record SuspendUserCommand(Guid UserId) : IRequest<Unit>;
