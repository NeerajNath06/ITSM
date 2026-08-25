# ADR-008 — Multi-Tenancy

**Status:** Accepted · **Full rationale:** `docs/architecture/multi-tenancy.md` (Phase 18)

## Context
Standard SaaS tenants and enterprise/regulated tenants have materially
different isolation requirements and budgets.

## Decision
Hybrid: shared database + `tenant_id` + PostgreSQL Row-Level Security for
standard tenants; dedicated database (same schema/migrations) for
enterprise/regulated tenants.

## Why
Shared-schema-with-RLS gives standard tenants low operational cost and
trivial cross-tenant-safe reporting; dedicated databases give the tenants
who contractually require physical isolation exactly that, without forcing
every tenant onto the more expensive model.

## Trade-offs
Two operational models to run and test (isolation suite must cover both);
accepted because each model individually stays simple, and the alternative
(one-size-fits-all) either overspends for standard tenants or
under-delivers for regulated ones.

## Consequences
Tenant isolation tier is a provisioning/connection-routing decision, never
a different data model or codebase branch.

## Rejected alternative
Schema-per-tenant — rejected: pays most of dedicated-database's operational
cost (per-tenant migrations, connection routing) without full physical
isolation benefit.
