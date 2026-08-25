# Audit Architecture

**Phase:** 19 · Centralized, immutable audit — a platform kernel capability
every bounded context uses, none reimplements (per `bounded-contexts.md`).

## What every audit entry captures

| Field | Purpose |
|---|---|
| `who` | `ActorUserId` (+ `ImpersonatedByUserId` if applicable) |
| `what` | `Action` (e.g. `Incident.Resolve`, `Acl.RuleUpdated`), `EntityType`, `EntityId` |
| `when` | `Timestamp` — UTC, server-generated only, never client-supplied |
| `where` | `SourceIp`, `Channel` (UI/API/integration/workflow) |
| `why` | `ReasonCode` where applicable (e.g. impersonation reason, emergency change justification) |
| `old value` / `new value` | Field-level diff for updates |
| `correlation id` | Ties the entry to the distributed trace/event chain that produced it (`event-driven-architecture.md`) |
| `tenant` | `TenantId` — every audit row is tenant-scoped like any other data |
| `service` | Which process wrote the entry (`Host.Api`, `Host.Workers`, `NotificationService`) |

## Schema

```sql
CREATE TABLE audit_log (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    actor_user_id UUID,
    impersonated_by_user_id UUID,
    action TEXT NOT NULL,
    entity_type TEXT NOT NULL,
    entity_id UUID,
    old_value JSONB,
    new_value JSONB,
    reason_code TEXT,
    correlation_id UUID,
    source_ip INET,
    channel TEXT NOT NULL,
    service TEXT NOT NULL,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now()
) PARTITION BY RANGE (occurred_at);
```

- **Partitioned by month** (PostgreSQL declarative partitioning) — audit is
  the highest-volume append-only table in the system; partitioning keeps
  writes/queries fast and makes retention/archival a partition-drop, not a
  row-by-row delete.
- **No `UPDATE` or `DELETE` grant exists on this table for any application
  role** — enforced at the PostgreSQL role/grant level, not merely by
  omitting the code path, so even a bug or a direct psql session by an
  over-privileged account cannot alter history.

## Write path: same transaction, not "best effort"

- Audit entries are written via an **EF Core `SaveChanges` interceptor**
  that inspects the `ChangeTracker` for tracked entities implementing
  `IAuditable`, builds the audit row(s), and includes them in the **same
  `DbContext.SaveChangesAsync` call** as the business change — so a
  transaction either persists both the change and its audit record, or
  neither (matches the requirement established in
  `docs/architecture/servicenow-layered-architecture.md`: audit must
  reflect what was actually committed).
- For changes originating in a satellite service (Notification delivery,
  Search reindex outcomes), the audit write happens **in that service's own
  transaction**, tagged with its `service` name — audit is a capability
  every process calls into (via a shared library + its own `audit_log`
  partition or a dedicated audit-ingest queue), not a single shared database
  connection every satellite reaches across a network boundary for (that
  would violate the cross-boundary data rule in
  `docs/database/database-architecture.md`).

## Read path

- A restricted read API (`security_admin`/`compliance_admin` roles only)
  supports investigation and compliance queries.
- General users see a filtered *activity stream* view (a UX projection of
  relevant audit entries onto a record's page, e.g. an Incident's history
  tab) — not raw audit table access.

## What is always audited (non-negotiable list)

- Every Task-family mutation (state, assignment, priority changes).
- Every ACL/permission rule change.
- Every role grant/revoke.
- Every impersonation start/end.
- Every CMDB CI/relationship change.
- Every Workflow Definition edit (not just instance execution).
- Every data export.
- Every login and failed login attempt (security-relevant, feeds anomaly
  detection in `docs/observability/observability.md`).

---

*Next: `docs/platform/notification-service.md` (Phase 20).*
