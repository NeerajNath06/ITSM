using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Tenancy.CreateTenant;

public sealed record CreateTenantCommand(
    string Name,
    string Slug,
    bool IsDedicated,
    string? ConnectionStringSecretRef) : IRequest<CreateTenantResult>;

public sealed record CreateTenantResult(Guid TenantId);
