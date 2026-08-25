# Shared Enterprise Data Model

**Phase:** 8 · How ServiceNow avoids duplicating common entities, and our
derived reusable foundation.

## How ServiceNow avoids duplication

Studying the concepts named in the Master AI Command (Task hierarchy, User,
Company, Service, Service Offering, CI, CMDB, Relationships, Case, Incident,
Request, Problem, Change, SLA, Contract, Entitlement, Asset), the consistent
pattern is:

1. **A small set of "root" tables** (`task`, `cmdb_ci`, `sys_user`,
   `core_company`) that many products extend or reference — never
   reimplemented per product.
2. **Table inheritance for "is-a" relationships** (Incident *is a* Task) so
   every subtype automatically gets number, state, assignment, SLA,
   journal, and attachments without redefining them.
3. **Reference (foreign-key-like) relationships for "has-a" / "relates-to"**
   (an Asset *links to* a CI, it does not *become* one; a Contract
   *covers* an Entitlement, it does not *contain* it as owned data).
4. **A generic relationship table for graph-shaped data** (CI Relationships)
   instead of a fixed set of foreign keys per possible relationship type —
   this is what lets CMDB represent arbitrary "runs on / depends on / hosted
   by" graphs without a schema migration every time a new relationship type
   is needed.

**Architectural lesson:** duplication is avoided by getting the **is-a vs.
has-a vs. relates-to** distinction right for each entity, not by forcing
everything into one mega-table. Getting this wrong in either direction
(over-normalizing into a rigid `task` monolith with 200 nullable columns, or
under-normalizing into copy-pasted `incident_number`, `problem_number`,
`change_number` fields) is the single most common shared-data-model mistake.

## Our derived foundation

### Root tables (no upstream dependency, in-database-per Phase 7 hybrid model)

| Table | Represents | Extended/referenced by |
|---|---|---|
| `task` | Base "unit of trackable work" | Incident, Problem, Change, Request, Requested Item (is-a) |
| `cmdb_ci` | Configuration item | Incident.affected_ci, Change.affected_ci, Asset.linked_ci (relates-to, never is-a) |
| `cmdb_relationship` | Generic CI↔CI graph edge (type, source, target) | Service mapping, impact analysis |
| `identity_user`, `identity_group` | Person / team | Task.assignee, Task.assignment_group, everywhere an actor is recorded |
| `tenant`, `organization` (company/business-unit within tenant) | Partition boundary | every tenant-scoped table via `tenant_id` |

### `task` hierarchy (is-a)

```
task (number, state, priority, impact, urgency, assignment_group,
      assignee, opened_at, resolved_at, closed_at, channel)
 ├── incident (caller, major_incident_flag, outage_start, outage_end)
 ├── problem (root_cause, workaround, known_error_flag)
 ├── change (change_type, risk, cab_id, window_start, window_end, backout_plan)
 └── requested_item (catalog_item_id, variables_jsonb, fulfillment_stage)
```

**PostgreSQL/EF Core strategy:** table-per-hierarchy-type (TPT) — a `task`
table with the shared columns, and a subtype table
(`incident`, `problem`, `change`, `requested_item`) holding only the
subtype-specific columns, joined 1:1 on the shared primary key. This is
elaborated with full EF Core mapping guidance in `docs/domain/task-model.md`
(Phase 9); this document only establishes *that* the hierarchy exists and
*why* (avoids a single sprawling nullable-column table, avoids duplicated
`number`/`state`/`journal` logic per subtype).

### Service model (has-a / relates-to, not is-a)

| Entity | Relationship |
|---|---|
| Business Service | Owns one or more Application Services (has-a) |
| Application Service | Runs on one or more CIs (relates-to, via `cmdb_relationship`) |
| Service Offering | A specific committed variant of a Business Service (e.g. "Email — Standard" vs "Email — Premium"), each with its own SLA/entitlement |
| SLA Definition | References a Service Offering + a Task type (relates-to) |
| Contract | Covers one or more Entitlements (has-a); an Entitlement grants access to a Service Offering for a Customer/Company (relates-to) |
| Asset | Links to a CI (relates-to) — **never merged into CI**, because Asset carries commercial/lifecycle data (cost, PO, warranty) that is meaningless to a CI's technical/relationship identity, and a CI can exist (a discovered server) before any Asset record does |

### Case (is-a, same shape as Task, different root — see below)

`Case` is architecturally identical to `Task` (number, state, assignment,
journal) but is used for the CSM/HR family, which is **out of scope for our
MVP** (see `roadmap/mvp.md`). We document the pattern here rather than build
a second parallel base table: **if/when we build Case-family products, they
extend the same `task` root**, not a duplicate hierarchy — this is the
direct lesson from Phase 2's finding that every "X Service Delivery"
product ServiceNow ships is Task/Case + a domain extension + the same portal
shell.

## Rules to prevent re-duplication as the platform grows

1. Before adding a new "record type," check: is this actually a new *root*
   entity, or a subtype of `task`/`cmdb_ci`/an existing relates-to pattern?
   Default answer is subtype.
2. Never add a `*_number` sequence, a `*_state` enum, or a `*_journal` table
   per new domain — these come from the `task` base by inheritance.
3. Relationship types (CI↔CI, Asset↔CI, Contract↔Entitlement) are modeled as
   **rows in a generic relationship/link table with a typed relationship
   kind**, not as a new foreign-key column per relationship type — this is
   what let ServiceNow's CMDB scale to arbitrary graph shapes without
   constant migrations, and it's the concrete lesson we're deliberately
   copying (Phase 10 elaborates this fully for CMDB specifically).

---

*Next: `docs/domain/task-model.md` (Phase 9) — full EF Core / PostgreSQL
mapping detail for the Task hierarchy.*
