# ADR-010 — Authorization

**Status:** Accepted · **Full rationale:** `docs/security/security-architecture.md` (Phase 17), `docs/architecture/bounded-contexts.md`

## Context
Need fine-grained, per-table/field authorization matching the ServiceNow
ACL pattern researched in Phase 1, implemented against our stack.

## Decision
RBAC (roles) for coarse capability + ABAC/policy-based authorization
(table/operation/field/condition rules, ASP.NET Core `IAuthorizationHandler`
policies) for fine-grained decisions, backed by PostgreSQL RLS as a second
wall. Explicit `Deny` always wins; fail closed on missing context.

## Why
Role-only (pure RBAC) can't express "agent can edit incidents in their own
assignment group only" without exploding the role count; policy-based rules
express this as data (metadata), matching the platform's
metadata-over-hardcoded-logic principle.

## Trade-offs
More moving parts than a simple `[Authorize(Roles = "Agent")]` attribute;
accepted because the alternative can't express real ITSM authorization
requirements (field-level strip, condition-based row access) at all.

## Consequences
Every table/operation/field combination has an explicit policy; a table
with zero explicit rules is inaccessible by default (fail closed).

## Rejected alternative
Pure RBAC with no field/condition granularity — rejected as insufficient
for the ACL matrix requirements established in Phase 1/17.
