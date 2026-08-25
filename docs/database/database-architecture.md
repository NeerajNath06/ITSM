# Database Architecture

**Phase:** 7 · PostgreSQL, following the modular-monolith decision in
`microservice-boundary-rules.md`. This document covers **service/module data
ownership topology**; tenant partitioning specifics are in
`docs/architecture/multi-tenancy.md` (Phase 18) — the two are related but
answer different questions ("who owns this table" vs. "how is one tenant's
rows isolated within it").

## The four options, evaluated honestly

| Option | Description | When it's right | When it's wrong |
|---|---|---|---|
| **1. Shared database, shared schema** | All modules' tables in one schema, one database | Modular monolith with heavy cross-module joins (our situation for Task/CMDB/Incident/Change) | When modules genuinely need independent scaling, migration cadence, or hard security isolation |
| **2. Database per service** | Each service owns a fully separate PostgreSQL database/cluster | True independently-deployed microservices with no cross-service joins (Notification, Search, AI inference in our design) | Applied to modules that constantly join each other — forces distributed transactions/API composition for routine queries |
| **3. Schema per service (same database instance)** | Logical separation via PostgreSQL schemas, one physical database | A middle ground: enforces ownership boundaries in code/tooling while keeping backup/DR/ops simple | Doesn't solve independent scaling or failure isolation — only useful as an ownership-discipline tool, not a scaling tool |
| **4. Hybrid** | Core modular-monolith tables in one shared schema; extracted satellite services get their own database | Matches a system that has *already* decided which contexts are extracted (our situation, per Phase 6) | Requires the extraction decision to be made first — this is why Phase 6 had to come before Phase 7 |

## Our decision: **Hybrid (Option 4)**

- **Core `Host.Api` modular monolith** (Task, Incident, Problem, Change,
  Request, CMDB, Identity, ACL, Audit, Workflow definitions/evaluation, SLA
  definitions/evaluation, Catalog, Knowledge, Asset): **one shared
  PostgreSQL database, shared schema**, partitioned per-tenant via Row-Level
  Security (see `multi-tenancy.md`). Schema-per-module namespacing
  (`itsm.incident`, `cmdb.configuration_item`) is used for **ownership
  clarity in tooling and migrations**, not for physical isolation — Option 3's
  discipline layered on top of Option 1's physical topology.
- **Satellite services** extracted per Phase 6 (Notification, Search
  indexer, AI inference, and eventually Discovery ingest at scale): **each
  gets its own PostgreSQL database.** They never share a schema with the
  core, and the core never queries their tables directly — only through
  their APIs/events.

### Why not database-per-microservice for everything

For Incident/Problem/Change/CMDB, "database per service" would force:

- **Distributed transactions or eventual consistency** for operations that
  are genuinely single-transaction today (resolving an Incident and closing
  its journal entry; assessing Change risk against the CMDB impact graph).
- **API composition for every agent-workspace record page**, which currently
  needs Incident + related CI + related Problem + SLA state in one view —
  turning a single indexed join into 4 network calls with their own failure
  modes.
- **Reporting complexity** — cross-domain reports (MTTR by CI class,
  SLA attainment by service) would require a data warehouse/ETL step from
  day one instead of a query against the operational store.

This is the same evidence-based reasoning Phase 6 used — the cost is real
and immediate, the benefit is hypothetical.

## Cost and complexity comparison

| Factor | Shared DB (core) | Per-service DB (satellites) |
|---|---|---|
| Transactions | Native ACID across modules | None across boundary — must use outbox/events |
| Reporting | Direct SQL joins/views | Requires read-side aggregation or async projection |
| Referential integrity | Enforced by FK constraints | Enforced by application logic + eventual consistency |
| Data duplication | Minimal (single source of truth per table) | Necessary at the boundary (e.g. Notification stores a denormalized snapshot of "who to notify," not a live FK to Identity) |
| Backup | One backup/restore unit for the core | Independent backup per satellite — must coordinate restore order if a satellite's data references core IDs |
| Migration | One EF Core / migration pipeline, ordered | Independent migration pipeline per satellite |
| Disaster recovery | One RPO/RTO target for the core | Each satellite can have its own RPO/RTO tuned to its criticality (Notification's delivery history may tolerate more loss than core ticket data) |
| Operational overhead | Lower — one database to run well | Higher — N databases, N sets of ops runbooks, N monitoring dashboards |

## Cross-boundary data rules (core ↔ satellites)

1. **No satellite reads the core database directly, and no core module reads
   a satellite's database directly.** All cross-boundary access is via API
   or async event.
2. **Satellites store denormalized snapshots of the data they need**, kept
   current via domain events (e.g. Notification's `AlertRecipient` stores
   the recipient's display name/device list snapshot at delivery time, not a
   live join to Identity) — this is deliberate duplication, not an
   oversight, and it's what keeps satellite failure from cascading into the
   core.
3. **Eventual consistency is acceptable and expected** at this boundary
   (e.g. it's fine if the search index lags the core by seconds) but must be
   monitored (lag metrics — see `docs/observability/observability.md`).

## PostgreSQL-specific decisions carried into later phases

- **Row-Level Security** for tenant partitioning within the shared core
  schema (`multi-tenancy.md`).
- **JSONB** for genuinely dynamic/metadata-driven fields (e.g. catalog item
  variables, custom fields defined per tenant) — never for data that has a
  fixed, queryable shape; over-using JSONB in place of real columns is a
  named anti-pattern (`docs/rules/anti-patterns.md`).
- **Partitioning** (declarative range/list partitioning) considered for the
  highest-volume append-only tables (Audit Log, Alert/Delivery Attempt) once
  volume metrics justify it — not applied speculatively on day one.
- **Logical replication** for feeding the Search indexer and Reporting read
  replicas without adding load to the primary write path.

## Backup / migration / DR baseline

- Automated point-in-time-recoverable backups on the core database
  (PostgreSQL WAL archiving), tuned RPO documented in
  `docs/architecture/disaster-recovery.md` (Phase 26).
- EF Core migrations are the **only** sanctioned way to change schema —
  no manual DDL against any environment above `dev` (see `docs/rules/database-rules.md`).
- Each satellite service owns its own migration pipeline and backup policy,
  reviewed independently — a satellite's DR posture must not be assumed to
  match the core's.

---

*Next: `shared-enterprise-data-model.md` (Phase 8) — the actual shared tables
this topology decision protects.*
