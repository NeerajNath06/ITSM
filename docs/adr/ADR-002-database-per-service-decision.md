# ADR-002 — Database-per-Service Decision

**Status:** Accepted · **Full rationale:** `docs/database/database-architecture.md` (Phase 7)

## Context
Following ADR-001's topology, we needed a data-ownership model consistent
with "modular monolith core + a few extracted satellites."

## Problem
Database-per-service is a common default recommendation; does it fit our
actual topology?

## Options
1. Database per service (strict), for every bounded context.
2. One shared database for everything, all services.
3. Hybrid: shared database for the modular-monolith core, dedicated
   database per extracted satellite service only.

## Decision
Option 3 (Hybrid).

## Why
Core domains need native ACID joins for routine operations (impact
analysis, record pages combining Task+CI+SLA). Satellites (Notification,
Search) are read-optimized/async by nature and gain real isolation benefit
from their own database.

## Trade-offs
The core database is a larger blast radius for outages/migrations than N
small databases would be; mitigated by HA (zone-redundant PostgreSQL) and
disciplined expand/contract migrations (`docs/rules/database-rules.md`).

## Consequences
Cross-boundary data access (core ↔ satellite) is always via API/event, never
direct database access, enforced by code review and the absence of
cross-service connection strings in configuration.

## Rejected alternative
Strict database-per-service for every context — rejected: forces
distributed transactions/eventual consistency onto operations that are
genuinely transactional today, and adds N databases' worth of operational
burden with no present benefit for the core domains (Phase 7 cost table).
