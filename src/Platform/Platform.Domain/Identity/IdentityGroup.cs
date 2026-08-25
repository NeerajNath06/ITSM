using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Identity;

/// <summary>A team (docs/database/shared-enterprise-data-model.md).</summary>
public sealed class IdentityGroup : TenantScopedEntity
{
    public Guid? OrganizationId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

#pragma warning disable CS8618
    private IdentityGroup()
    {
    }
#pragma warning restore CS8618

    public static IdentityGroup Create(Guid tenantId, string name, string? description = null, Guid? organizationId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new IdentityGroup
        {
            TenantId = tenantId,
            OrganizationId = organizationId,
            Name = name,
            Description = description,
        };
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
