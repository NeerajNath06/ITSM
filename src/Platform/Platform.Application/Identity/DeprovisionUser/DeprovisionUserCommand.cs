using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Identity.DeprovisionUser;

public sealed record DeprovisionUserCommand(Guid UserId) : IRequest<Unit>;
