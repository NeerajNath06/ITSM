# ADR-004 — RabbitMQ Decision

**Status:** Accepted, flagged for re-evaluation · **Full rationale:** `docs/integration/event-driven-architecture.md` (Phase 14), `docs/azure/production-architecture.md` (Phase 25)

## Context
The Master AI Command specifies RabbitMQ as the messaging technology.

## Problem
Which message broker backs the outbox/inbox event architecture and command
queues?

## Options
1. RabbitMQ (self-hosted, now on Kubernetes/K3s per `ADR-022`).
2. Azure Service Bus (managed) — moot as a default once `ADR-022` established the self-hosted-first direction; remains viable only under the documented Azure-alternative deployment path.
3. Kafka (not evaluated in depth — overkill for our current event volume
   and adds significant operational complexity beyond what our use case
   requires).

## Decision
RabbitMQ (Option 1), self-hosted, per the locked stack.

## Why
Mature AMQP semantics fit our outbox/inbox/DLQ pattern directly; no
per-message pricing at our expected volume; matches the explicit
Master AI Command instruction.

## Trade-offs
**Explicitly named as the highest operational-complexity item in the Azure
architecture (Phase 25)** — self-hosted HA/clustering is real, ongoing
operational work a managed Azure Service Bus would remove.

## Consequences
The platform team owns RabbitMQ HA configuration, monitoring, and upgrade
cadence directly; this must be staffed/planned for, not assumed "free."

## Rejected alternative
Azure Service Bus — not rejected outright, explicitly flagged in Phase 25
as the live alternative to revisit via a follow-up ADR if self-hosting
RabbitMQ's operational burden proves higher than expected in practice.
