# ADR-014 — Search Engine

**Status:** Superseded by `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`
· **Full rationale:** `docs/platform/search-architecture.md` (Phase 21)

> **Amendment:** `ADR-022` promotes **OpenSearch** to the primary/default
> search backend from day one, superseding the "start with Postgres FTS,
> migrate later" decision below. The `ISearchProvider` abstraction this ADR
> established is exactly what makes that promotion low-risk — no consumer
> code depends on which backend is active. The original reasoning (avoid
> unjustified infrastructure before measured need) no longer applies once
> OpenSearch is already a standing part of the self-hosted stack for other
> reasons (`ADR-022`), so standing it up for search too carries no
> incremental infrastructure cost.

## Decision
Start with PostgreSQL full-text search (`tsvector`/GIN indexes) behind an
`ISearchProvider` abstraction; migrate to Azure AI Search only when relevance,
faceting, or latency needs measurably exceed what PostgreSQL FTS delivers.

## Why
Avoids standing up and operationally owning a second search cluster before
real query volume/relevance pressure justifies it — matches the "don't
assume every Azure service is necessary" discipline (Phase 25).

## Trade-offs
PostgreSQL FTS has weaker relevance tuning/faceting than a dedicated search
engine; accepted as sufficient for MVP scope, with the provider abstraction
specifically paying down the cost of switching later.

## Consequences
Search indexing is still an extracted satellite service (own database/index
store) regardless of which backend is active, consuming domain events.

## Rejected alternative
Adopting Azure AI Search or Elasticsearch/OpenSearch from day one —
rejected as premature infrastructure investment absent a measured need.
