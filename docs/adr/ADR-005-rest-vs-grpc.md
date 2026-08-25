# ADR-005 — REST vs. gRPC

**Status:** Accepted · **Full rationale:** `docs/api/rest-grpc-rules.md` (Phase 15)

## Context
The target stack allows both REST and gRPC; without a rule, gRPC risks
creeping into places that don't need it (or vice versa).

## Decision
REST for all external/UI/partner-facing traffic. gRPC only for internal
service-to-service calls between the core and an extracted satellite where
a measured or clearly-anticipated latency/throughput need exists, per the
four-question checklist in `rest-grpc-rules.md`.

## Why
React, partners, and webhooks all need broad HTTP compatibility and
debuggability; gRPC's binary protocol only pays for its added complexity on
genuinely high-volume internal calls we control end-to-end.

## Trade-offs
Two protocols in the codebase instead of one adds a small amount of
tooling/onboarding overhead, accepted because forcing REST everywhere would
cost real internal-call performance, and forcing gRPC everywhere would
break external/browser compatibility.

## Consequences
Any new gRPC contract proposal must cite which checklist condition
justifies it, in code review, or defaults to REST.

## Rejected alternative
gRPC-everywhere, or REST-everywhere — both rejected as failing to fit at
least one of our real traffic classes.
