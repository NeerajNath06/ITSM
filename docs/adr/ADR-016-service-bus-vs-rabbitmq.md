# ADR-016 — Azure Service Bus vs. RabbitMQ

**Status:** Accepted (RabbitMQ), explicitly open for revisit · **Full rationale:** `docs/adr/ADR-004-rabbitmq-decision.md`, `docs/azure/production-architecture.md`

## Decision
RabbitMQ (self-hosted, on Kubernetes/K3s per `ADR-022`) is the current messaging technology,
per the locked stack. This ADR exists specifically as the named revisit
point Phase 25 pointed to.

## Why
Matches the explicit target stack; avoids per-message pricing; AMQP model
fits the outbox/inbox/DLQ pattern directly.

## Trade-offs
Self-hosted HA/clustering is genuine, ongoing operational burden — the
single most operationally complex component in the current architecture
(Phase 25 finding).

## Consequences
The platform team must actively monitor RabbitMQ operational cost over time
against this ADR's assumption that it's worth the trade-off.

## Rejected alternative (for now)
Azure Service Bus — fully viable, would remove the self-hosting burden at
the cost of per-operation pricing and a managed-service-specific feature
set; to be revisited via a superseding ADR if RabbitMQ's operational cost
proves higher than budgeted.
