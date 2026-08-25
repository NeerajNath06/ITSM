using ITSM.BuildingBlocks.Domain;
using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Acl;

/// <summary>
/// One ACL rule, per the table/operation/role/field/condition/effect shape
/// in docs/security/security-architecture.md (carried forward from
/// 01-security-architecture.md §3.2). Evaluation order (applied by
/// IAclEvaluator, not here): any matching Deny wins; else require at least
/// one matching Allow; else deny (fail closed). Never hard-deleted —
/// <see cref="Deactivate"/> is the only removal path, preserving the audit
/// trail on a metadata type that is itself always-audited.
/// </summary>
public sealed class AclRule : TenantScopedEntity, IAuditable
{
    /// <summary>Matches every table when a rule isn't scoped to one specific table.</summary>
    public const string AllTables = "*";

    public string TableName { get; private set; }
    public AclOperation Operation { get; private set; }
    public Guid RequiredRoleId { get; private set; }

    /// <summary>Null = whole-record rule; set = this rule governs one field only.</summary>
    public string? FieldName { get; private set; }

    /// <summary>
    /// Raw JSON condition expression (e.g. {"op":"in","left":"record.assignmentGroupId","right":"user.groupIds"}).
    /// The interpreter is deferred to Phase 3 — no Task/CMDB data exists yet
    /// to evaluate a condition against, so today a null condition always
    /// matches ("unconditional") and a non-null one is stored but not yet
    /// evaluated by IAclEvaluator.
    /// </summary>
    public string? ConditionExpression { get; private set; }

    public AclEffect Effect { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618
    private AclRule()
    {
    }
#pragma warning restore CS8618

    public static AclRule Create(
        Guid tenantId,
        string tableName,
        AclOperation operation,
        Guid requiredRoleId,
        AclEffect effect,
        string? fieldName = null,
        string? conditionExpression = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        return new AclRule
        {
            TenantId = tenantId,
            TableName = tableName,
            Operation = operation,
            RequiredRoleId = requiredRoleId,
            FieldName = fieldName,
            ConditionExpression = conditionExpression,
            Effect = effect,
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;

    public void Reactivate() => IsActive = true;
}
