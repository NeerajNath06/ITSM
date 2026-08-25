using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Identity;

/// <summary>
/// Group membership junction. Composite-keyed (<c>group_id</c>, <c>user_id</c>)
/// — no independent lifecycle of its own, so it implements <see cref="ITenantScoped"/>
/// directly rather than deriving from <see cref="TenantScopedEntity"/> (no
/// synthetic <c>Id</c> needed).
/// </summary>
public sealed class IdentityGroupMember : ITenantScoped
{
    public Guid TenantId { get; private init; }
    public Guid GroupId { get; private init; }
    public Guid UserId { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }

#pragma warning disable CS8618
    private IdentityGroupMember()
    {
    }
#pragma warning restore CS8618

    public static IdentityGroupMember Create(Guid tenantId, Guid groupId, Guid userId) => new()
    {
        TenantId = tenantId,
        GroupId = groupId,
        UserId = userId,
        CreatedAt = DateTimeOffset.UtcNow,
    };
}
