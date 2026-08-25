namespace ITSM.BuildingBlocks.Domain;

/// <summary>
/// Marker for entities whose mutations must be captured in <c>audit_log</c>.
/// The audit SaveChanges interceptor (docs/audit/audit-architecture.md) scans
/// the change tracker for entities implementing this interface and writes
/// their old/new values in the same transaction as the business change — no
/// members are needed here, the interceptor works purely off EF Core's own
/// change-tracking metadata.
/// </summary>
public interface IAuditable;
