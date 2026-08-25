# SLA Engine

**Phase:** 49 · Reusable SLA engine, consumed by Incident/Request/Change
per their domain profiles, owned by no single product (per
`bounded-contexts.md`).

## Data model

```sql
CREATE TABLE sla_definition (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    name TEXT NOT NULL,
    applies_to_task_type TEXT NOT NULL,       -- 'Incident', 'Change', ...
    condition JSONB NOT NULL,                  -- e.g. { "priority": 1 }
    duration_minutes INT NOT NULL,
    business_calendar_id UUID NOT NULL,
    start_condition JSONB NOT NULL,
    pause_conditions JSONB NOT NULL,
    stop_condition JSONB NOT NULL
);

CREATE TABLE sla_instance (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    task_id UUID NOT NULL,
    sla_definition_id UUID NOT NULL REFERENCES sla_definition(id),
    status TEXT NOT NULL,                       -- Running, Paused, Met, Breached, Cancelled
    started_at TIMESTAMPTZ NOT NULL,
    paused_duration_minutes INT NOT NULL DEFAULT 0,
    due_at TIMESTAMPTZ NOT NULL,
    completed_at TIMESTAMPTZ
);

CREATE TABLE business_calendar (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    name TEXT NOT NULL,
    timezone TEXT NOT NULL,
    working_hours JSONB NOT NULL,               -- per-weekday windows
    holidays JSONB NOT NULL                      -- date list, tenant-editable
);
```

## Start / pause / stop conditions

- Expressed as **metadata condition expressions** evaluated against the
  Task's current state (same expression engine used by Business Rule
  conditions and Workflow Condition activities — one condition-evaluation
  implementation reused across the platform, not three separate ones).
- Example: `start_condition = { "on": "TaskCreated" }`,
  `pause_conditions = [{ "state": "OnHold" }, { "state": "AwaitingCustomer" }]`,
  `stop_condition = { "on": "TaskResolved" }`.

## Business calendar and holidays

- `duration_minutes` is measured against **working hours only** by default
  (a calendar-aware SLA), configurable per tenant (24/7 SLA also
  supported via a calendar with no non-working hours defined).
- Holidays are tenant-editable rows, not hardcoded — a tenant's regional
  holiday calendar differs from another's, and MVP must support that from
  day one given multi-tenant SaaS reality.

## Escalation and breach

- On breach (`due_at` passed with `status` still `Running`), an
  `SlaBreached` event fires — consumed by Notification (escalation to the
  configured recipient, reusing the Group/Shift/Escalation machinery,
  Phase 20) and by Reporting (breach metrics).
- A **percentage-consumed** warning threshold (e.g. "75% of SLA time
  elapsed") fires an earlier, lower-severity notification — proactive
  warning, not just after-the-fact breach reporting.

## Evaluation model

- SLA evaluation (start/pause/stop transitions, breach detection) runs as a
  **`Host.Workers` scheduled process** (Phase 6/31 decision: evaluation
  logic close to the core for Task-context access, but the *timer* that
  checks "has this SLA now breached" runs durably in workers, surviving API
  process restarts) — not a synchronous check on every API request, which
  would be both wasteful and miss breaches that occur with no request in
  flight.
- Pause/resume is driven by Task state-change events (Phase 14) — an SLA
  instance reacts to `TaskStateChanged`, it does not poll the Task table.

## Why SLA is not a column on Task

Per `docs/domain/task-model.md`: SLA state is a **separate, referencing
context** (`sla_instance.task_id`), not columns on `Incident`/`Change`
directly — this keeps Task focused on its own lifecycle and lets a single
Task have **multiple concurrent SLA instances** (e.g. a "response time" SLA
and a separate "resolution time" SLA against the same Incident) without
schema contortion.

---

*Next: `docs/governance/architecture-governance.md` (Phase 50).*
