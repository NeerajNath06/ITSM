using ITSM.BuildingBlocks.Domain;

namespace Platform.Domain.Identity;

public sealed record IdentityUserInvited(Guid TenantId, Guid UserId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record IdentityUserSuspended(Guid TenantId, Guid UserId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record IdentityUserReactivated(Guid TenantId, Guid UserId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record IdentityUserDeprovisioned(Guid TenantId, Guid UserId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record UserRoleGranted(Guid TenantId, Guid UserId, Guid RoleId, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record UserRoleRevoked(Guid TenantId, Guid UserId, Guid RoleId, DateTimeOffset OccurredAt) : IDomainEvent;
