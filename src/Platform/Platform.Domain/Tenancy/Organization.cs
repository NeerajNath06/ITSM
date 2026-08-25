using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Tenancy;

/// <summary>
/// Business-unit within a tenant (docs/database/shared-enterprise-data-model.md).
/// </summary>
public sealed class Organization : TenantScopedEntity
{
    public Guid? ParentOrganizationId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public OrganizationStatus Status { get; private set; }

#pragma warning disable CS8618
    private Organization()
    {
    }
#pragma warning restore CS8618

    public static Organization Create(Guid tenantId, string name, string code, Guid? parentOrganizationId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return new Organization
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            ParentOrganizationId = parentOrganizationId,
            Status = OrganizationStatus.Active,
        };
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public void Retire() => Status = OrganizationStatus.Retired;
}
