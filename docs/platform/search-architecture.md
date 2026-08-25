# Search Architecture

**Phase:** 21 · Enterprise search across Tasks, Knowledge, Catalog, and CMDB.

## Technology evaluation

| Option | Fit |
|---|---|
| **PostgreSQL full-text search (`tsvector`/`tsquery`, `pg_trgm`)** | Adequate for structured ticket/CI search, no new infrastructure — but weaker relevance tuning/faceting than a dedicated engine |
| **OpenSearch** | Better relevance tuning, faceting, and scale for large knowledge bases and high-cardinality free-text search |

## Decision: **OpenSearch from day one**

Per `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`
(superseding `ADR-014`'s original "start with Postgres FTS, migrate later"
plan): OpenSearch is already a standing part of the self-hosted
infrastructure stack, so the original reasoning for deferring it (avoid
operationally owning a second, unjustified search cluster) no longer
applies — there is no incremental infrastructure cost to using it for
search too.

- Incident/Problem/Change short description, Knowledge article body, and
  CMDB CI name/attributes are indexed into per-tenant OpenSearch indices
  (or a shared index with a mandatory `tenant_id` filter — see Tenant
  isolation below) from day one.
- The search layer still sits behind an internal `ISearchProvider`
  abstraction — not because a migration is anticipated, but because it
  keeps consumers decoupled from the specific backend regardless, and
  costs nothing to keep.
- PostgreSQL FTS remains available as a documented fallback for a
  deployment that specifically wants to avoid running OpenSearch (e.g. a
  minimal single-node on-prem installation) — but is no longer the
  default.

## Support requirements (apply to whichever backend is active)

| Requirement | Design |
|---|---|
| **Global search** | Cross-entity-type search from the React shell's search bar, results grouped by type (Incident, Article, CI) |
| **Domain search** | Scoped search within a single module (e.g. "search my incidents") — reuses the same index with a narrower query |
| **Security trimming** | Search results are filtered through the same ACL evaluation as a direct table read — a search index is a projection, not a bypass; a user must never see a result in search they couldn't see via the normal API |
| **Tenant isolation** | Every index (or, for PostgreSQL FTS, every query) is `tenant_id`-scoped identically to every other data access path — see `multi-tenancy.md` isolation checklist |
| **Indexing** | Event-driven (consumes domain events via RabbitMQ, per `event-driven-architecture.md`), near-real-time, eventually consistent by design (lag is monitored, not assumed zero) |
| **Reindexing** | A full reindex job (per tenant, resumable, rate-limited so it doesn't starve live indexing traffic) exists for schema changes, data-quality fixes, or provider migration |

## Security trimming implementation

Because search results are a projection **derived from** source data, not
independently authoritative, the safest implementation stores enough ACL
metadata in the index (owning group, classification, tenant) to pre-filter
the query itself (fast path), **and** the API layer re-validates ACL on the
returned result set before responding (defense in depth, matching the
multi-layer principle from `security-architecture.md`) — a stale index entry
for a record whose ACL later tightened must never leak.

## Satellite service placement

Search indexing is an extracted satellite service (`microservice-boundary-
rules.md` decision) — read-optimized, rebuildable, and tolerant of eventual
consistency, the textbook case for extraction. It owns its own database/
index store, consumes events, and is never queried directly by the core for
writes.

---

*Next: `docs/platform/knowledge-management.md` (Phase 22).*
