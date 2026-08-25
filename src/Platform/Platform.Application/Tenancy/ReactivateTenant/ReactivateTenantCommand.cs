using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Tenancy.ReactivateTenant;

public sealed record ReactivateTenantCommand(Guid TenantId) : IRequest<Unit>;
