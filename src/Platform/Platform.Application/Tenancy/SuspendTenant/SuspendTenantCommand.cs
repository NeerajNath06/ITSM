using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Tenancy.SuspendTenant;

public sealed record SuspendTenantCommand(Guid TenantId) : IRequest<Unit>;
