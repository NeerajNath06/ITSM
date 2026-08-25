# ADR-011 — Audit Architecture

**Status:** Accepted · **Full rationale:** `docs/audit/audit-architecture.md` (Phase 19)

## Decision
A centralized, immutable, partitioned `audit_log` table, written via an EF
Core `SaveChanges` interceptor in the **same transaction** as the business
change it records. No application role has `UPDATE`/`DELETE` grants on it.

## Why
Compliance and forensic value both require that an audit entry can never be
silently altered after the fact, and that it's impossible for a business
change to commit without its audit entry (or vice versa).

## Trade-offs
Every mutating transaction pays a small additional write cost; accepted as
non-negotiable for an enterprise ITSM/compliance-adjacent product.

## Consequences
No module implements its own bespoke audit table/trigger — this is a
platform kernel capability every context calls into.

## Rejected alternative
Best-effort async audit logging (write business change, publish an event,
audit consumer writes later) — rejected: creates a window where a change
can be committed with no corresponding audit record if the consumer fails.
