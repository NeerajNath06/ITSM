# ADR-007 — Event-Driven Architecture

**Status:** Accepted · **Full rationale:** `docs/integration/event-driven-architecture.md` (Phase 14)

## Context
Cross-context effects (core → Notification, core → Search) need a reliable
propagation mechanism that doesn't couple the core to satellite
availability.

## Decision
Outbox pattern on publish (same-transaction durability), inbox pattern on
consume (idempotency), RabbitMQ topic exchanges for domain/integration
events, work queues for commands, per-queue DLQs, correlation ID +
causation ID on every message.

## Why
This is the only way to guarantee "the business transaction committed" and
"the fact was durably queued for publish" happen together, without a
distributed transaction across PostgreSQL and RabbitMQ.

## Trade-offs
Adds an outbox-relay worker and inbox bookkeeping per consumer — real
implementation cost, accepted because the alternative (fire-and-forget
publish) silently loses events on any publish-time failure.

## Consequences
Every domain event a bounded context wants to be durable and cross-boundary
goes through the outbox — no context publishes directly to RabbitMQ from
inside a request handler.

## Rejected alternative
Direct synchronous publish inside the request transaction's commit path —
rejected: RabbitMQ unavailability would then block or corrupt core business
transactions.
