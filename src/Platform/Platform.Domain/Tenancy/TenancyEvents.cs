using ITSM.BuildingBlocks.Domain;

namespace Platform.Domain.Tenancy;

public sealed record TenantProvisioned(Guid TenantId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record TenantSuspended(Guid TenantId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record TenantReactivated(Guid TenantId, DateTimeOffset OccurredAt) : IDomainEvent;
