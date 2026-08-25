# Task Model

**Phase:** 9 · Full design for the reusable `Task` abstraction introduced in
`shared-enterprise-data-model.md`.

## Task hierarchy

```
Task
 ├── Incident
 ├── Problem
 ├── Change
 ├── Request / RequestedItem
 ├── WorkOrder            (future — Field Service-style, out of MVP)
 └── other domain tasks   (future Case-family, out of MVP)
```

## Inheritance vs. composition — the actual decision

Two real options were evaluated:

| Approach | Description | Verdict |
|---|---|---|
| **Inheritance (is-a)** | `Incident` *is a* `Task`; shares identity, gets base behavior automatically | **Chosen** for the state/number/assignment/journal/attachment machinery — these are structurally identical across every subtype, and inheritance lets a single "all my open work" query span every subtype with zero duplication |
| **Composition (has-a)** | `Incident` *has a* `TaskInfo` component instead of inheriting | Rejected as the primary strategy — it would require every generic Task API (Table API `/task`, cross-type list views, SLA engine, Notification) to reach through a `.TaskInfo` indirection on every subtype, adding friction with no offsetting benefit here |

**Where composition IS used:** subtype-specific *capabilities* that are not
universal — e.g. `MajorIncidentDetails` is composed onto `Incident` (not
every Incident is a major incident), and `CabDecision` is composed onto
`Change`. The rule: **inherit the identity and lifecycle every subtype
shares; compose the capabilities only some instances of a subtype need.**

## PostgreSQL strategy: Table-per-Type (TPT)

```
task                          incident                    problem
------------------           ------------------           ------------------
id (uuid, PK)                task_id (uuid, PK, FK→task)  task_id (uuid, PK, FK→task)
tenant_id (uuid)              caller_id (uuid)             root_cause (text)
number (text, unique per      major_incident (bool)        known_error (bool)
  tenant)                     outage_start (timestamptz)
task_type (text)              outage_end (timestamptz)
state (text)
priority (smallint)
impact (smallint)
urgency (smallint)
assignment_group_id (uuid)
assignee_id (uuid)
opened_at (timestamptz)
resolved_at (timestamptz)
closed_at (timestamptz)
channel (text)
```

- **Why TPT over Table-per-Hierarchy (TPH):** TPH (one giant `task` table
  with nullable columns for every subtype) was explicitly rejected — it
  produces a table where 80% of columns are NULL for any given row, defeats
  `NOT NULL` constraints that should exist per-subtype, and makes every
  index a judgment call about which partial subset of rows it actually
  covers. TPT keeps the shared base tight and each subtype's own columns
  properly constrained.
- **Why not Table-per-Concrete-Type (TPC):** TPC would duplicate the
  `number`/`state`/`assignment` columns into every subtype table, which is
  exactly the duplication the shared foundation exists to avoid, and it
  breaks the "one query across all task types" use case without a `UNION`.
- **Cost accepted:** TPT requires a join to load a full Incident record
  (`task` ⋈ `incident`). This is an acceptable, indexed, single-hop join —
  not a cross-service network call — and PostgreSQL handles it natively.

## EF Core mapping (.NET 10)

```csharp
public abstract class TaskBase
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Number { get; set; } = default!;
    public TaskState State { get; set; }
    public Priority Priority { get; set; }
    public Guid? AssignmentGroupId { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTimeOffset OpenedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public List<JournalEntry> Journal { get; set; } = [];
}

public class Incident : TaskBase
{
    public Guid CallerId { get; set; }
    public bool IsMajorIncident { get; set; }
    public MajorIncidentDetails? MajorIncidentDetails { get; set; } // composed, nullable
}

// OnModelCreating
modelBuilder.Entity<TaskBase>().UseTptMappingStrategy();
modelBuilder.Entity<TaskBase>().ToTable("task");
modelBuilder.Entity<Incident>().ToTable("incident");
modelBuilder.Entity<Problem>().ToTable("problem");
modelBuilder.Entity<Change>().ToTable("change");

modelBuilder.Entity<TaskBase>()
    .HasQueryFilter(t => t.TenantId == _tenantContext.TenantId); // fail-closed tenant filter, see multi-tenancy.md
```

- `TaskBase` is `abstract` — there is no way to persist a bare `Task` row
  without a concrete subtype, matching the domain reality that "work" is
  always some specific kind of work.
- The tenant query filter is applied **once, on the base type**, and EF Core
  propagates it to every derived query automatically — this is the
  application-layer half of the tenant isolation wall (RLS is the database
  half; see `multi-tenancy.md`).

## Cross-cutting fields explained

| Concern | Design |
|---|---|
| **Status/state** | `TaskState` enum stored as `text` (not `smallint`) in PostgreSQL for readability in ad hoc queries/audits, with a CHECK constraint restricting valid values per `task_type`. Valid transitions are enforced in the Application layer via an explicit state machine per subtype (see `docs/workflow/workflow-engine-architecture.md` for how transitions can optionally be workflow-driven), never a bare property setter. |
| **Priority** | Derived from Impact × Urgency via a configurable (metadata-driven) matrix, not hardcoded — mirrors ServiceNow's priority-lookup-table pattern (Phase 1 finding: metadata over hardcoded screens applies to logic, not just UI). |
| **Assignment** | `AssignmentGroupId`/`AssigneeId` are nullable FKs to Identity; assignment rules (auto-assign by skill/load) are a Workflow/Business-Rule concern, not baked into the Task entity itself. |
| **SLA** | Task does **not** own SLA state. An `SlaInstance` row (owned by the SLA Engine context, Phase 49) references `TaskId` — SLA start/pause/stop/breach is a separate concern reacting to Task events, not a column on Task. This avoids the anti-pattern of one entity accumulating every cross-cutting concern's state. |
| **Audit** | Every Task mutation is captured by the shared Audit interceptor (`docs/audit/audit-architecture.md`) at the EF Core `SaveChanges` level — not hand-written per subtype. |
| **Comments/Journal** | Modeled as an append-only `JournalEntry` child table (`task_id`, `author_id`, `body`, `is_customer_visible`, `created_at`) — never edited or deleted, only appended, matching the immutable-history requirement that also governs Audit. |
| **Attachments** | `Attachment` table stores metadata (`task_id`, `object_key`, `content_type`, `size`, `virus_scan_status`) pointing to MinIO; bytes never live in PostgreSQL (see `docs/platform/file-storage.md`, Phase 23). |
| **Relationships** | Task-to-Task relationships (Problem→Incidents, Change→Incidents caused) use a generic `task_relationship` table (`source_task_id`, `target_task_id`, `relationship_type`) — same generic-relationship-table pattern used for CMDB (Phase 10), not a bespoke FK column per relationship type. |

## What subtypes must NOT do

- Must not redefine `Number`, `State`, `Priority`, `Journal`, or
  `Attachments` — these come from `TaskBase` only.
- Must not embed SLA state directly on the subtype table — SLA is a
  separate owned context (Phase 49) that references `TaskId`.
- Must not bypass the shared state-machine validation to "quick-set" a
  state directly via SQL/migration in any environment above `dev`.

---

*Next: `docs/cmdb/csdm-inspired-service-model.md` (Phase 10) — the same
is-a/has-a/relates-to discipline applied to the technology/service graph.*
