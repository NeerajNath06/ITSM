# ADR-003 — PostgreSQL Decision

**Status:** Accepted · **Full rationale:** `docs/adr/ADR-000-technology-stack-supersession.md`, `docs/database/database-architecture.md`

## Context
The Master AI Command's target stack specifies PostgreSQL, superseding the
original guide's SQL Server choice (ADR-000).

## Problem
Which relational database backs the core platform and every extracted
service?

## Options
1. SQL Server (original guide's choice).
2. PostgreSQL (Master AI Command's choice).
3. A managed cloud-native alternative (e.g. CockroachDB) for global
   distribution.

## Decision
PostgreSQL, self-hosted on Kubernetes/K3s (via an operator such as
CloudNativePG or the Zalando Postgres Operator) per
`docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md` — supersedes
the originally-assumed Azure Database for PostgreSQL Flexible Server as the
default target; a managed PostgreSQL service remains viable for a
deployment that opts into the documented Azure alternative
(`docs/azure/production-architecture.md`).

## Why
Open licensing model at enterprise scale, mature native Row-Level Security
(the mechanism our tenancy model depends on, Phase 18), strong JSONB support
for metadata-driven fields, and mature declarative partitioning for
high-volume append-only tables (audit log, delivery attempts).

## Trade-offs
Team familiarity/tooling ecosystem differs from SQL Server; mitigated by
EF Core's provider abstraction keeping most application code database-agnostic
in practice, and by this being a deliberate, documented, one-time switch
rather than an ongoing multi-database-support burden.

## Consequences
All schema design (Phase 7-11, 33) is PostgreSQL-native (RLS via `CREATE
POLICY`, `snake_case` naming, native UUID generation) — not a
database-agnostic lowest-common-denominator design.

## Rejected alternative
SQL Server — superseded per ADR-000, the user's explicit direction to build
from the Master AI Command's stack as written.
