using ITSM.BuildingBlocks.Domain;
using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Identity;

/// <summary>
/// A person (docs/database/shared-enterprise-data-model.md) — referenced by
/// FK from Task.AssigneeId etc. elsewhere in the platform, never a subtype
/// of TaskBase itself (docs/domain/task-model.md).
/// </summary>
public sealed class IdentityUser : TenantScopedEntity, IAuditable
{
    public Guid? OrganizationId { get; private set; }

    /// <summary>Keycloak <c>sub</c> claim. Null for a not-yet-federated local identity.</summary>
    public string? ExternalSubject { get; private set; }

    public string Username { get; private set; }
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public IdentityUserStatus Status { get; private set; }
    public bool IsLocalIdentity { get; private set; }
    public bool MfaEnrolled { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

#pragma warning disable CS8618
    private IdentityUser()
    {
    }
#pragma warning restore CS8618

    public static IdentityUser Invite(
        Guid tenantId,
        string username,
        string email,
        string displayName,
        Guid? organizationId = null,
        bool isLocalIdentity = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        var user = new IdentityUser
        {
            TenantId = tenantId,
            OrganizationId = organizationId,
            Username = username,
            Email = email,
            DisplayName = displayName,
            Status = IdentityUserStatus.Invited,
            IsLocalIdentity = isLocalIdentity,
            MfaEnrolled = false,
        };

        user.Raise(new IdentityUserInvited(tenantId, user.Id, DateTimeOffset.UtcNow));
        return user;
    }

    /// <summary>Links this user to its Keycloak identity on first successful federated sign-in.</summary>
    public void LinkExternalSubject(string externalSubject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalSubject);
        ExternalSubject = externalSubject;
        if (Status == IdentityUserStatus.Invited)
        {
            Status = IdentityUserStatus.Active;
        }
    }

    public void RecordLogin(DateTimeOffset occurredAt) => LastLoginAt = occurredAt;

    public void Suspend()
    {
        Status = IdentityUserStatus.Suspended;
        Raise(new IdentityUserSuspended(TenantId, Id, DateTimeOffset.UtcNow));
    }

    public void Reactivate()
    {
        if (Status != IdentityUserStatus.Suspended)
        {
            throw new InvalidOperationException($"User {Id} is not Suspended.");
        }

        Status = IdentityUserStatus.Active;
        Raise(new IdentityUserReactivated(TenantId, Id, DateTimeOffset.UtcNow));
    }

    public void Deprovision()
    {
        Status = IdentityUserStatus.Deprovisioned;
        Raise(new IdentityUserDeprovisioned(TenantId, Id, DateTimeOffset.UtcNow));
    }
}
