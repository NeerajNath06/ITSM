# ADR-001 — Modular Monolith vs. Microservices

**Status:** Accepted · **Full rationale:** `docs/architecture/microservice-boundary-rules.md` (Phase 6)

## Context
ServiceNow itself (Phase 3 research) runs as one platform with
application-scoped modules, not a service mesh. We had to decide our own
default topology before any module boundary work could proceed.

## Problem
Should Incident/Problem/Change/Request/CMDB/Identity/ACL each be an
independently deployable microservice, or modules inside one host?

## Options
1. Microservice per bounded context.
2. Single modular monolith for everything.
3. Modular monolith for tightly-coupled core domains + microservices for
   genuinely independent, justified edges.

## Decision
Option 3. Core ITSM/CMDB/platform-kernel domains live in `Host.Api`, a
modular monolith with enforced module boundaries (NetArchTest-equivalent).
Notification, Search indexing, and (later) AI inference are extracted
satellite services from day one because they meet the extraction criteria
in `microservice-boundary-rules.md` (provider isolation, async/eventual
consistency, independent scaling).

## Why
Incident/Problem/Change/CMDB constantly join in single UI views and share
transactions; forcing them apart would create distributed transactions and
API-composition overhead for the platform's most common operations, with no
present pressure justifying the cost.

## Trade-offs
Slower path to per-domain independent deploy cadence; mitigated by strict
in-process module boundaries that make a future extraction mechanical if a
real trigger appears.

## Consequences
All Incident/Problem/Change/Request/CMDB/Identity/ACL code lives in one
deployable; extraction is re-evaluated per module against the named
triggers in Phase 6, never by default.

## Rejected alternative
Microservice-per-domain — rejected as the default per Master AI Command
Rule 1 and the concrete cost analysis in Phase 6/7.
