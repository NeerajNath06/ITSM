using ITSM.BuildingBlocks.Domain;
using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Identity;

/// <summary>
/// A role grant. Modeled as its own tracked entity (with a synthetic <c>Id</c>
/// rather than a raw <c>(user_id, role_id)</c> skip-navigation) because role
/// grant/revoke is on the audit architecture's non-negotiable always-audited
/// list, and <c>audit_log.entity_id</c> needs a single id to reference
/// (docs/audit/audit-architecture.md). Revocation removes the row — the
/// audit interceptor captures a deletion via EF Core's own change tracking
/// the same way it captures an insert or update.
/// </summary>
public sealed class UserRoleAssignment : TenantScopedEntity, IAuditable
{
    public Guid UserId { get; private init; }
    public Guid RoleId { get; private init; }
    public Guid? GrantedByUserId { get; private init; }
    public DateTimeOffset GrantedAt { get; private init; }

#pragma warning disable CS8618
    private UserRoleAssignment()
    {
    }
#pragma warning restore CS8618

    public static UserRoleAssignment Grant(Guid tenantId, Guid userId, Guid roleId, Guid? grantedByUserId)
    {
        var assignment = new UserRoleAssignment
        {
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId,
            GrantedByUserId = grantedByUserId,
            GrantedAt = DateTimeOffset.UtcNow,
        };

        assignment.Raise(new UserRoleGranted(tenantId, userId, roleId, assignment.GrantedAt));
        return assignment;
    }

    /// <summary>Signals revocation intent; the Application handler removes the row.</summary>
    public void Revoke() => Raise(new UserRoleRevoked(TenantId, UserId, RoleId, DateTimeOffset.UtcNow));
}
