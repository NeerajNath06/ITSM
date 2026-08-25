# Multi-Tenancy Architecture

**Phase:** 18 · Comparison and decision for PostgreSQL-based tenancy,
per `docs/database/database-architecture.md`'s hybrid topology.

## The four options, evaluated

| Option | Isolation strength | Operational cost | Cross-tenant reporting | Our fit |
|---|---|---|---|---|
| **1. Database per tenant** | Strongest — physical isolation | Highest — N databases to patch/monitor/back up/migrate | Requires federated queries across N databases | Reserved for **enterprise/regulated tenants** who require it contractually |
| **2. Schema per tenant** | Strong — logical isolation, same instance | Moderate — migrations must run per-schema; connection routing needed | Requires cross-schema queries | Not chosen as the default — pays most of the "database per tenant" migration/ops cost without full physical isolation |
| **3. Shared tables with `tenant_id`** | Weaker on paper, strong in practice **when backed by RLS** | Lowest — one schema, one migration pipeline | Trivial — one query, `tenant_id` filter | **Default for standard tenants** |
| **4. Hybrid** | Matches tenant tier to isolation need | Moderate — two operational models to run, but each individually simple | Standard-tier trivial; enterprise-tier federated only when explicitly needed | **Chosen** |

## Decision: Hybrid

- **Standard tenants:** shared PostgreSQL database, shared schema, `tenant_id`
  column + **Row-Level Security** on every tenant-scoped table (mechanics in
  `docs/security/security-architecture.md`).
- **Enterprise / regulated tenants:** dedicated PostgreSQL database
  (potentially dedicated server/cluster for the largest or most regulated),
  same schema and migration set as standard tenants — a tenant's isolation
  tier is a deployment/connection-routing decision, **not** a different data
  model. This is the same principle the original ENTERPRISE_ITSM guide
  established and it survives the stack change unchanged.

## Tenant isolation checklist

- [ ] `ITenantContext` resolved from JWT `tid` claim (or hostname for
      portal traffic), and **fails closed** (throws) if unresolved before
      any query executes.
- [ ] EF Core global query filter on every tenant-scoped `DbSet`, applied at
      the shared base type (`TaskBase`, `cmdb_ci`, etc.) so derived types
      inherit it automatically.
- [ ] PostgreSQL RLS policy on every tenant-scoped table — CI-enforced (a
      migration adding a `tenant_id` column without a matching `CREATE
      POLICY` fails the build).
- [ ] Dedicated-tenant connection routing: a `TenantConnectionResolver`
      selects the correct connection string per request based on the
      tenant's isolation tier, cached, never resolved by client-supplied
      input alone (always cross-checked against the authenticated JWT's
      `tid`).
- [ ] Valkey cache keys are tenant-prefixed (`tenant:{id}:...`) — no shared
      cache key space across tenants.
- [ ] RabbitMQ messages carry `tenant_id`; consumers process tenant-scoped
      work, and any fan-out background job iterates tenants explicitly
      rather than scanning a global, unscoped table.
- [ ] Blob storage containers/paths are tenant-prefixed.
- [ ] Search index (Phase 21) is tenant-partitioned — no query can override
      its tenant scope.
- [ ] **Automated isolation test suite**: two tenants, identical business
      record numbers, prove zero leakage across API, search, files,
      background jobs, and (later) AI/RAG retrieval.

## Authorization interaction

Tenant isolation is the **outermost** authorization layer — evaluated
before any table/field-level ACL policy (`security-architecture.md` §
Authorization). A user cannot be granted a role that reaches across tenant
boundaries; roles are always tenant-scoped grants, even for a platform
support engineer, who instead uses a distinct, fully-audited
platform-operator identity rather than a tenant role with "see everything"
scope.

## Configuration, branding, quotas per tenant

| Concern | Design |
|---|---|
| **Configuration** | `tenant_configuration` table (feature flags, business hours calendar, SLA defaults) — read through a cached configuration service, never hardcoded per-tenant conditionals in application code |
| **Branding** | Logo/theme metadata served to React at portal load, stored as tenant configuration + blob reference — no per-tenant React build |
| **Quotas** | Per-tenant rate limits (YARP), storage quotas (MinIO bucket quotas), and connection-pool fairness (PostgreSQL) — enforced centrally, not per-module |
| **Tenant-specific workflows/rules** | Workflow Definitions and Business Rule parameters are tenant-scoped rows (a tenant customizes *data*, not code) — matches the "metadata over hardcoded screens" platform principle carried through this entire program |

## Tenant lifecycle

- **Provisioning** is a saga (create tenant row → apply RLS policies if not
  already present on shared tables → seed default metadata/roles → for
  dedicated tenants, provision the database and run migrations) — resumable
  and idempotent, not a single unrecoverable transaction.
- **Offboarding**: data export window, then crypto-shred (destroy
  tenant-scoped encryption keys) + row purge for shared-tenant data, or
  database decommission for dedicated tenants — full runbook in
  `docs/architecture/disaster-recovery.md` and
  `docs/governance/data-governance.md`.

---

*Next: `docs/audit/audit-architecture.md` (Phase 19).*
