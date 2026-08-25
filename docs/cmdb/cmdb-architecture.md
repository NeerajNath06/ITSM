# CMDB Architecture

**Phase:** 11 · CI, CI types, relationships, discovery, identification,
reconciliation, dependency mapping, ownership, lifecycle.

## Configuration Item (CI)

```sql
CREATE TABLE cmdb_ci (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    ci_class TEXT NOT NULL,              -- 'Server','Application','Database','NetworkDevice',...
    name TEXT NOT NULL,
    status TEXT NOT NULL,                -- 'InstallStatus': Installed, Retired, In Maintenance, ...
    owner_group_id UUID,
    environment TEXT,                    -- Prod/Staging/Dev, drives blast-radius weighting
    attributes JSONB NOT NULL DEFAULT '{}', -- class-specific attributes, see "CI classes" below
    source TEXT NOT NULL,                 -- 'Discovery','Manual','Import'
    last_discovered_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
```

### CI classes: fixed columns vs. JSONB attributes

- **Fixed columns** for fields every CI needs regardless of class (`name`,
  `status`, `owner_group_id`, `environment`) — queryable, indexable,
  constrained.
- **`attributes JSONB`** for class-specific fields (a Server's CPU/RAM, a
  Database's engine/version) — because the platform must support **new CI
  classes without a schema migration** (metadata-driven, per the platform
  principle carried from the original guide). A `cmdb_ci_class_schema`
  metadata table defines which attribute keys/types are valid per class, so
  the UI and validation stay driven by data, not code, without giving up
  structural guarantees entirely.
- **Rule:** if 90%+ of CIs of a class would populate a given attribute and
  it's queried/filtered often, it graduates to a real column via migration
  — JSONB is for genuine variability, not a permanent escape hatch from
  schema design (same rule as `shared-enterprise-data-model.md`).

## Relationships (the graph)

```sql
CREATE TABLE cmdb_relationship (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    source_ci_id UUID NOT NULL REFERENCES cmdb_ci(id),
    target_ci_id UUID NOT NULL REFERENCES cmdb_ci(id),
    relationship_type TEXT NOT NULL,   -- 'runs on','depends on','hosted by','uses','supports'
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE(source_ci_id, target_ci_id, relationship_type)
);
CREATE INDEX ix_cmdb_rel_source ON cmdb_relationship(source_ci_id, relationship_type);
CREATE INDEX ix_cmdb_rel_target ON cmdb_relationship(target_ci_id, relationship_type);
```

- One generic table, both directions indexed — impact analysis ("what
  depends on this CI") and dependency analysis ("what does this CI depend
  on") are both cheap indexed lookups.
- `relationship_type` is metadata-defined (which types are valid, and which
  are "impact-propagating" vs. purely informational) so new relationship
  semantics don't require code changes — mirrors CSDM's typed-edge
  philosophy without hardcoding the type list in the schema.

## Discovery, identification, reconciliation

| Concept | Design |
|---|---|
| **Discovery** | Runs as a satellite process (Phase 6 decision) — a collector agent (on-prem/cloud, MID-Server-analog) scans infrastructure and posts payloads to an ingest API. Never writes to the database directly — always through the CMDB context's write API so identification/reconciliation rules apply uniformly regardless of source. |
| **Identification** | Determining whether an incoming discovery payload is a *new* CI or an *update to an existing* CI. Uses a configurable identification rule per CI class (e.g. Server identified by MAC address + serial number, not by name — names change, hardware identifiers don't). Must be deterministic and documented per class; ad hoc "match by name" is a named anti-pattern (fails silently on rename, duplicates on hostname reuse). |
| **Reconciliation** | When multiple sources report the same CI (Discovery + manual entry + an import), a reconciliation priority ranks sources per attribute (e.g. Discovery wins for `cpu_cores`, manual entry wins for `owner_group_id`) so the "true" record is deterministic, not last-write-wins by accident. |
| **Dependency mapping** | Populated by Discovery/Service Mapping-equivalent logic that infers `depends on`/`runs on` edges from observed network connections or configuration, written as `cmdb_relationship` rows like any other source. |

## Ownership and lifecycle

- Every CI has an `owner_group_id` (who is accountable for it) — required
  for Change risk assessment (Phase 49-adjacent) and for routing
  CMDB-data-quality remediation work.
- **Lifecycle states:** `Planned → Installed → In Maintenance → Retired`.
  A `Retired` CI is never deleted — relationships and historical
  Incident/Change references must remain resolvable. Retirement is a status
  change, not a row deletion (matches the audit-immutability principle
  applied to operational data, not just the audit log itself).

## Data quality

Full rule set (completeness scoring, staleness detection, orphan-CI
detection, duplicate detection) is in `cmdb-data-quality-rules.md`
(companion Phase 11 document) — kept separate because data quality is an
ongoing **operational discipline** with its own metrics/alerting, not a
one-time schema concern.

## Security boundary (recap from `bounded-contexts.md`)

Writes are restricted to the Discovery service identity and `cmdb_admin`
role — this is the highest-blast-radius data in the operational domain
(Phase 2 finding), so it does **not** inherit general-agent write access
even though CIs are read broadly by Incident/Change/Asset.

---

*Companion: `cmdb-data-quality-rules.md`. Next: `docs/workflow/workflow-engine-architecture.md` (Phase 12).*
