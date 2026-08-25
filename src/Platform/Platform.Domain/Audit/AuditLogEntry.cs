using System.Net;
using ITSM.BuildingBlocks.Tenancy;

namespace Platform.Domain.Audit;

/// <summary>
/// One immutable row in <c>audit_log</c> (schema verbatim from
/// docs/audit/audit-architecture.md). Written exclusively by
/// AuditSaveChangesInterceptor in the same transaction as the business
/// change it records — never constructed by ordinary Application handlers,
/// and never itself audited, updated, or deleted (no UPDATE/DELETE grant
/// exists on this table for any application role at the PostgreSQL level).
/// Deliberately not an <see cref="ITSM.BuildingBlocks.Domain.Entity"/> — it
/// raises no domain events and has no lifecycle beyond being written once.
/// </summary>
public sealed class AuditLogEntry : ITenantScoped
{
    public Guid Id { get; private init; }
    public Guid TenantId { get; private init; }
    public Guid? ActorUserId { get; private init; }
    public Guid? ImpersonatedByUserId { get; private init; }
    public string Action { get; private init; }
    public string EntityType { get; private init; }
    public Guid? EntityId { get; private init; }
    public string? OldValueJson { get; private init; }
    public string? NewValueJson { get; private init; }
    public string? ReasonCode { get; private init; }
    public Guid? CorrelationId { get; private init; }
    public IPAddress? SourceIp { get; private init; }
    public string Channel { get; private init; }
    public string Service { get; private init; }
    public DateTimeOffset OccurredAt { get; private init; }

#pragma warning disable CS8618
    private AuditLogEntry()
    {
    }
#pragma warning restore CS8618

    public static AuditLogEntry Create(
        Guid tenantId,
        string action,
        string entityType,
        string channel,
        string service,
        Guid? actorUserId = null,
        Guid? impersonatedByUserId = null,
        Guid? entityId = null,
        string? oldValueJson = null,
        string? newValueJson = null,
        string? reasonCode = null,
        Guid? correlationId = null,
        IPAddress? sourceIp = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentException.ThrowIfNullOrWhiteSpace(service);

        return new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ActorUserId = actorUserId,
            ImpersonatedByUserId = impersonatedByUserId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValueJson = oldValueJson,
            NewValueJson = newValueJson,
            ReasonCode = reasonCode,
            CorrelationId = correlationId,
            SourceIp = sourceIp,
            Channel = channel,
            Service = service,
            OccurredAt = DateTimeOffset.UtcNow,
        };
    }
}
