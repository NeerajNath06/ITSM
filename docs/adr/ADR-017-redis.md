# ADR-017 — Redis (now Valkey)

**Status:** Superseded (product) by `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`;
role/usage below unchanged · **Full rationale:** `docs/infrastructure/self-hosted-production-architecture.md`

> **Amendment:** self-hosted **Valkey** (the Redis-protocol-compatible,
> Linux-Foundation-governed fork) replaces Azure Cache for Redis. Valkey
> speaks the same protocol our code already targets via the standard Redis
> client library, so no application code changes — only the deployed
> image/connection endpoint changes. Everything below (role, trade-offs,
> consequences) is otherwise unchanged.

## Decision
Self-hosted Valkey for: BFF session storage, distributed locks
(e.g. duplicate-alert suppression), rate-limit counters, and tenant
configuration cache.

## Why
Sub-millisecond latency for the high-frequency reads (session validation,
config lookups) on the request hot path; self-hosted per the OSS stack
direction (`ADR-022`) rather than a managed dependency.

## Trade-offs
Redis is explicitly **not** a system of record — anything stored there must
be safely rebuildable/re-derivable from PostgreSQL or re-authentication.

## Consequences
No business data is ever stored in Redis as its sole copy; cache
invalidation is event-driven (`TenantConfigChanged`, etc.), not TTL-only
where correctness matters.

## Rejected alternative
Azure Cache for Redis (managed) — rejected once `ADR-022` established the
self-hosted-first direction; would have reintroduced a managed-cloud
dependency the rest of the stack no longer has.
