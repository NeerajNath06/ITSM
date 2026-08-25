using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Tenancy.GetTenantById;

public sealed record GetTenantByIdQuery(Guid TenantId) : IRequest<TenantDto>;

public sealed record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    string IsolationTier,
    string Status,
    DateTimeOffset CreatedAt);
