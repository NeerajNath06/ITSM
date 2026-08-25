# ADR-012 — CMDB

**Status:** Accepted · **Full rationale:** `docs/cmdb/cmdb-architecture.md`, `docs/cmdb/csdm-inspired-service-model.md` (Phase 10–11)

## Decision
A single `cmdb_ci` table (fixed columns + class-specific JSONB attributes)
plus a generic typed `cmdb_relationship` graph-edge table, layered per a
simplified CSDM model (Business Service → Service Offering → Application →
CI), populated by Discovery (satellite) and manual entry, governed by
explicit data-quality rules.

## Why
A flat, ungoverned CI table produces an untrustworthy CMDB that agents stop
using; the layered model + generic relationship table is the concrete
mechanism that makes impact analysis (Incident/Change → affected Business
Service) a short, typed graph walk rather than a guess.

## Trade-offs
More upfront schema/process discipline (identification rules, reconciliation
priority) than "just insert whatever Discovery finds"; accepted because an
untrustworthy CMDB has near-zero value to the rest of the platform.

## Consequences
CMDB writes are restricted to `cmdb_admin`/Discovery identity; every CI
class must declare required attributes before use.

## Rejected alternative
Merging Asset and CI into one entity — rejected per
`shared-enterprise-data-model.md`: they have independent lifecycles and
different sensitivity/ownership.
