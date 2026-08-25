# Bounded Contexts

**Phase:** 5 · Defines the domain boundaries the rest of the program (module
layout, database design, API ownership, ADR-001) is built on.

## How to read this document

For the architecturally load-bearing contexts (the ones every other domain
depends on, or that set the pattern for a whole family), a full profile is
given: Responsibility, Owned/Read/Write data, APIs, Events, Dependencies,
Transaction boundary, Security boundary. For the remaining domains — which
mostly *repeat* an already-established pattern (another `Task` subtype,
another `Case` subtype) — a summary matrix follows, to avoid restating the
same shape 30 times.

---

## Full profiles: load-bearing contexts

### Identity

- **Responsibility:** authenticate users/services, hold the user/group/role
  graph, issue and validate tokens.
- **Owns:** User, Group, Role, Service Identity, Session.
- **Reads:** nothing outside itself for authentication decisions.
- **Writes:** its own tables only.
- **APIs:** `/api/v1/identity/*` (REST), internal gRPC `IdentityService` for
  fast token/claims resolution from other in-process modules.
- **Events:** `UserCreated`, `UserRoleChanged`, `SessionRevoked`.
- **Dependencies:** external IdP (OIDC/SAML) for federation.
- **Transaction boundary:** its own tables; role changes commit atomically
  with the audit entry recording them.
- **Security boundary:** the most privileged context in the system — write
  access to Role/Group assignment is itself gated by ACL, and every write is
  audited (see `docs/audit/audit-architecture.md`).

### Tenant / Organization

- **Responsibility:** represent the customer/organization boundary every
  other domain's data is partitioned by.
- **Owns:** Tenant, Organization/Company (sub-structure within a tenant, e.g.
  business unit), Tenant Configuration, Tenant Feature Flags.
- **Reads:** nothing.
- **Writes:** its own tables; tenant provisioning also triggers schema/RLS
  policy setup (see `multi-tenancy.md`).
- **APIs:** `/api/v1/tenants/*` (platform-admin only, not tenant-exposed).
- **Events:** `TenantProvisioned`, `TenantOffboarded`, `TenantConfigChanged`.
- **Dependencies:** none (root context).
- **Transaction boundary:** tenant creation is a saga (DB objects +
  RLS policy + default metadata seed), not a single transaction — must be
  idempotent and resumable.
- **Security boundary:** platform-operator-only; no tenant can read or write
  another tenant's row in this context, and no tenant user has any role that
  grants access to it at all.

### User / Role / Permission

- Modeled as **part of Identity** for User/Role, with **Permission**
  (the ACL rule set: table/operation/field/condition) as its own
  sub-context because permission *definitions* are metadata edited through
  Admin/Configuration workflows, while User/Role assignment is an identity
  operation. Keeping them logically distinct (even though co-located in the
  same module) avoids the anti-pattern of authorization logic leaking into
  every domain that needs to check it.
- **Owns:** Permission/ACL Rule definitions.
- **Reads:** Role (from Identity) to evaluate a grant.
- **APIs:** internal only — every other context calls the Authorization
  service, never re-implements evaluation logic.
- **Security boundary:** editing a Permission rule itself requires the
  `security_admin` role — a lower-privileged user must never be able to
  grant themselves access by editing a rule (same rule as our earlier
  `01-security-architecture.md` §3.2).

### Task (base abstraction)

- **Responsibility:** the shared "unit of trackable work" every operational
  domain (Incident, Problem, Change, Request, Work Order) extends.
- **Owns:** Task base fields: number, state, priority, assignment,
  timestamps, journal, attachments, watch list.
- **Reads:** nothing structurally — it is the base, not a consumer.
- **Writes:** its own base table (PostgreSQL table-per-hierarchy or
  table-per-type — decided in `docs/domain/task-model.md`, Phase 9).
- **APIs:** generic `/api/v1/table/task` for cross-type queries (e.g. "all my
  open work regardless of type"); type-specific domain APIs for
  Incident/Change/etc. sit on top.
- **Events:** `TaskCreated`, `TaskStateChanged`, `TaskAssigned`,
  `TaskClosed` — generic events every subtype inherits, plus subtype-specific
  events (`IncidentDeclaredMajor`).
- **Dependencies:** Identity (assignee), Audit, Notification, SLA Engine.
- **Transaction boundary:** a single Task's state transition, its journal
  entry, and its audit entry commit together.
- **Security boundary:** ACL evaluated per Task *type* and per field — a
  Security Incident (a Task subtype) is **not** visible under the same ACL
  rules as a general Incident even though both share the base table
  structure (see Phase 2 finding on Security Incident isolation).

### Incident

- **Responsibility:** restore service after unplanned disruption.
- **Owns:** Incident-specific fields (major-incident flag, caller, outage
  window) extending Task.
- **Reads:** CMDB (affected CI), Knowledge (suggested articles), SLA Engine.
- **Writes:** Incident table, Task base table (same transaction).
- **APIs:** `/api/v1/incidents/*` (Domain API) + generic Task/Table API.
- **Events:** `IncidentResolved`, `IncidentDeclaredMajor`.
- **Dependencies:** Task, CMDB, Knowledge, Notification (major incident
  paging), SLA Engine.
- **Transaction boundary:** Incident record + Task base row.
- **Security boundary:** standard tenant ACL; caller PII may be field-level
  restricted for lower-privilege viewers.

### CMDB (Configuration Management)

- **Responsibility:** system of record for technology (CIs) and their
  relationships.
- **Owns:** CI, CI Class, CI Relationship, Service/Application Service
  mapping.
- **Reads:** Discovery ingest payloads (external to this context's write
  path, but the trigger for most writes).
- **Writes:** its own tables only.
- **APIs:** `/api/v1/cmdb/*`, plus a read-optimized "impact graph" query API
  consumed by Incident/Change.
- **Events:** `CICreated`, `CIRelationshipChanged`, `CIRetired`.
- **Dependencies:** none upstream (root context, like Tenant) — everything
  else depends on it, it depends on nothing else in the domain model.
- **Transaction boundary:** a CI + its relationship writes from one
  discovery batch commit together per CI (not the whole batch atomically —
  batches must be resumable/idempotent, not all-or-nothing).
- **Security boundary:** highest blast-radius data in the operational
  domain — a bad write here mis-informs every dependent workflow, so writes
  are restricted to Discovery service identity + `cmdb_admin` role, never
  general agent roles.

### Workflow

- **Responsibility:** execute versioned, data-defined process graphs
  (approval, wait, action, branch) triggered by domain events or direct
  invocation.
- **Owns:** Workflow Definition, Workflow Instance, Workflow Step execution
  state.
- **Reads:** the triggering record just enough to evaluate conditions
  (via a scoped context object, not arbitrary table access).
- **Writes:** its own instance/step tables; calls back into the triggering
  domain via its Application-layer interface to apply outcomes (e.g. "set
  Incident state to Resolved") — never writes directly into another
  context's table.
- **APIs:** `/api/v1/workflows/*` (definitions, admin); instances are
  primarily driven by events, not direct API calls.
- **Events:** consumes domain events as triggers; emits
  `WorkflowInstanceCompleted`, `WorkflowStepFailed`.
- **Dependencies:** Notification (for approval requests), Identity (approver
  resolution).
- **Transaction boundary:** one workflow step's execution + its state
  transition.
- **Security boundary:** editing a Workflow Definition requires
  `workflow_admin`; an instance only acts within the ACL of the identity
  that triggered it (a workflow cannot silently escalate its own privilege).

### Notification (see also `docs/architecture/09-notification-oncall-escalation-engine.md`)

- **Responsibility:** deliver alerts through the right channel, to the right
  person, with escalation if unacknowledged.
- **Owns:** Group, Shift, Escalation Timeline, Device, Alert, Delivery
  Attempt.
- **Reads:** Identity (recipient resolution), the triggering record's
  minimal context (for message templating).
- **Writes:** its own tables only.
- **APIs:** `/api/v1/notifications/*`.
- **Events:** consumes domain events as triggers; emits
  `AlertDelivered`, `AlertAcknowledged`, `AlertEscalated`.
- **Dependencies:** Identity, external channel providers.
- **Security boundary:** device PII is `Confidential`-classified.

### Audit

- **Responsibility:** immutable record of every mutating action across every
  context.
- **Owns:** Audit Log (append-only).
- **Reads:** nothing (write-heavy, read path is for compliance/investigation
  queries only).
- **Writes:** its own append-only store; every other context calls it, it
  never calls back.
- **APIs:** internal write API (called by every context's persistence layer,
  ideally via a shared interceptor, not per-context boilerplate); a
  restricted read API for security/compliance roles.
- **Dependencies:** none.
- **Transaction boundary:** the audit entry is written in the **same
  transaction** as the business change it records (never "best effort,
  after the fact").
- **Security boundary:** immutable — no update/delete code path exists;
  read access is itself audited.

---

## Summary matrix: remaining domains

Each of these follows the **Task or Case pattern already established
above** (extends the shared base, has its own ACL domain, references but
does not own CMDB/Identity/Knowledge) unless the "Notable divergence" column
says otherwise.

| Domain | Base pattern | Owned data | Notable divergence |
|---|---|---|---|
| Problem | Task | Problem, Known Error | Links many Incidents without owning them |
| Change | Task | Change, CAB decision | Segregation-of-duties requirement on approval |
| Request / Requested Item | Task | Request, Requested Item, Cart | Composed of Catalog Item + Variables, not a flat form |
| Approval | Task-adjacent (shared activity, not a domain of its own) | Approval instance | Consumed by Workflow as an activity type, not a standalone bounded context |
| Case (CSM/HR base) | Task | Case | Out of scope for our MVP; documented for pattern completeness only |
| WorkOrder (Field Service-style) | Task | Work Order | Out of scope for MVP |
| Service Catalog | Independent (Platform capability) | Catalog Item, Category, Variable Set | Consumed by Request across every department, owned by no single product |
| Knowledge | Independent (Platform capability) | Article, Category, Version | Read by nearly every domain; write path is its own editorial workflow |
| Asset (ITAM) | Independent, linked to CMDB | Asset, Model, License, Contract | **Not merged with CI** — asset is commercial/lifecycle data, CI is technical/relationship data; linked, not unified |
| Configuration Item / Discovery / Event / Monitoring | CMDB family | See CMDB profile above | Discovery and Event are ingestion/processing contexts that *feed* CMDB, not CMDB itself |
| Integration | Independent (Platform capability) | Inbound/outbound integration definitions, webhook subscriptions | Owns no business data — a routing/transformation layer only |
| Reporting | Independent (read-side) | Saved report/dashboard definitions | Read-only against other contexts' data via a query layer, never a write path |
| Search | Independent (Platform capability) | Search index (derived, not authoritative) | Rebuildable from source-of-truth data at any time — never the system of record for anything |
| Document | Independent | Attachment metadata (object pointer, ACL, retention) | Actual bytes live in MinIO, not the database |
| Customer / Vendor / Contract | Independent-ish, adjacent to Asset/Case | Customer, Vendor, Contract | Out of scope for MVP; documented for CSM/ITAM-adjacent completeness |
| Employee | Identity-adjacent | Employee profile (HR-specific superset of User) | Out of scope for MVP (HRSD family); highest PII sensitivity if built later |
| Risk / Compliance / Security | Independent, isolated ACL domain | Risk register, Control, Security Incident | Deliberately **not** sharing the general Incident ACL domain (Phase 2 finding) |
| Portfolio / Project | Independent | Portfolio, Program, Project, Demand | Out of scope for MVP |

---

## Cross-context rules (apply to every domain above)

1. A context may **read** another context's data only through that
   context's published Application-layer interface or API — never direct
   database access across context boundaries (this is the NetArchTest-
   enforced rule carried from the original master guide, still valid here).
2. A context may **write** only its own tables. Cross-context effects
   happen via domain events consumed by the other context, or via an
   explicit command sent through that context's own API/interface.
3. Every context that participates in the Task/Case family inherits the
   shared ACL and audit mechanics — it does not reimplement them.
4. CMDB and Tenant are the only two "root" contexts with no upstream
   dependencies; everything else ultimately depends on one or both.

---

*Next: `microservice-boundary-rules.md` (Phase 6) — deciding which of these
contexts, if any, become independently deployable services.*
