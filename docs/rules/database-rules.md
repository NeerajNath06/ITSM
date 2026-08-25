# Database Rules

**Phase:** 33 · PostgreSQL-specific, binding rules.

## Naming

- `snake_case` for tables and columns (PostgreSQL convention, avoids
  quoting).
- Table names: singular (`incident`, not `incidents`) — matches the
  entity, not the collection; consistent across the schema.
- Foreign key columns: `{referenced_table}_id` (`assignment_group_id`, not
  `group`).
- Junction/relationship tables: `{a}_{b}` or a descriptive name for a typed
  relationship (`cmdb_relationship`, not `ci_ci`).

## Primary keys, GUID strategy

- **UUID primary keys** (`gen_random_uuid()`, PostgreSQL native via
  `pgcrypto`/`uuid-ossp`) for every table — required for the multi-tenant,
  potentially-multi-database-per-tenant topology (Phase 7/18), where
  sequential integer IDs would collide or leak cross-tenant row-count
  information.
- No client-generated IDs trusted for uniqueness without server-side
  verification — the server is the authority on ID assignment for new
  rows.

## Foreign keys and referential integrity

- Foreign key constraints are **enforced at the database level** — the
  application layer's validation is a UX nicety, not a substitute for a
  real FK constraint. "We'll enforce it in code" without the FK is a named
  anti-pattern.
- `ON DELETE` behavior is explicit and reviewed per relationship
  (`RESTRICT` default; `CASCADE` only where child rows genuinely have no
  independent lifecycle, e.g. `knowledge_article_version` rows on article
  delete — which itself should be rare given the soft-lifecycle preference
  below).

## Timestamps

- Every table: `created_at TIMESTAMPTZ NOT NULL DEFAULT now()`; mutable
  tables also get `updated_at TIMESTAMPTZ NOT NULL DEFAULT now()`,
  maintained by an EF Core `SaveChanges` interceptor (not a DB trigger, to
  keep the audit interceptor and timestamp update in the same reviewable
  code path).
- All timestamps `TIMESTAMPTZ` (timezone-aware), never `TIMESTAMP` — a
  multi-tenant, multi-region system cannot afford ambiguous local time.

## Soft delete vs. hard delete

- **Task-family and CMDB data**: never hard-deleted — a status/lifecycle
  change (`Closed`, `Retired`) represents removal from active use; the row
  remains for audit, relationship integrity, and historical reporting.
- **Purely operational/derived data** (search index entries, cache rows,
  notification delivery attempts past their retention window): hard-deleted
  per the retention policy (`docs/governance/data-governance.md`) — soft-delete
  everywhere indiscriminately is its own anti-pattern (unbounded table
  growth with no real audit value).
- A soft-deleted/retired row is excluded from default queries via a global
  query filter, not by every call site remembering a `WHERE status !=
  'Retired'` clause.

## Audit

- Handled by the shared interceptor (`docs/audit/audit-architecture.md`) —
  no table implements its own bespoke audit columns/triggers.

## Migrations

- **EF Core migrations only** — no manual DDL against any environment above
  `dev` (Master AI Command Rule 13). Every migration is reviewed like code,
  runs in CI against a disposable database before merge, and is applied via
  the deployment pipeline, never by an engineer running `dotnet ef database
  update` directly against `staging`/`prod`.
- Every migration that adds a `tenant_id` column includes the corresponding
  `ENABLE ROW LEVEL SECURITY` + `CREATE POLICY` statements in the **same
  migration** — CI fails the build otherwise (Phase 17/18).
- Destructive migrations (column/table drop) ship as a two-step process:
  stop writing/reading the column in application code first (deployed and
  verified), drop it in a later migration — never in the same deploy as the
  code change that stops using it.

## Transactions and concurrency

- A single `SaveChangesAsync` call per command handler — one transaction
  per business operation, not multiple partial commits.
- **Optimistic concurrency** via a `xmin`-based (PostgreSQL system column)
  or explicit `row_version` concurrency token on entities subject to
  concurrent edit (Task state, CMDB CI) — a conflicting concurrent update
  fails with a specific `ConcurrencyException` mapped to a `409 Conflict`,
  not a silent last-write-wins.
- Long-running operations (bulk import, reindex) never hold a single
  long-lived transaction — batched with periodic commits.

## Partitioning

- Applied to genuinely high-volume, time-ordered, append-mostly tables
  (`audit_log`, `delivery_attempt`) once volume metrics justify it — not
  speculatively on every table (adds query-planning and maintenance
  overhead not worth paying on low-volume tables).

## JSONB usage

- Reserved for genuinely dynamic, schema-varying data (CI class-specific
  `attributes`, catalog item `variables`, tenant configuration values) —
  see the explicit rule in `shared-enterprise-data-model.md`: if a field is
  populated for 90%+ of rows of a type and is frequently filtered/sorted on,
  it graduates to a real column.
- JSONB columns that are queried/filtered get a **GIN index**
  (`jsonb_path_ops` for containment queries) — an unindexed JSONB column
  used in a `WHERE` clause is a review-blocking finding.

## Normalization / denormalization

- Core transactional tables (`task`, `cmdb_ci`, `incident`, etc.): normalized
  per the is-a/has-a/relates-to modeling in `shared-enterprise-data-model.md`.
- Deliberate denormalization is allowed **only** at documented boundaries:
  satellite services storing snapshots of core data they consume
  (`docs/database/database-architecture.md` cross-boundary rules), and
  read-optimized reporting projections — never inside the core transactional
  schema itself "for query convenience."

## Query optimization

- Every new list/search endpoint's query plan (`EXPLAIN ANALYZE`) is
  reviewed before merge for anything touching a table expected to exceed
  100k rows per tenant.
- Composite indexes match actual query predicates (tenant_id first, per the
  RLS/access pattern, then the most selective filter column) — indexes are
  added with evidence (a real or anticipated query), not speculatively on
  every column.

---

*Next: `docs/rules/api-rules.md` (Phase 34).*
