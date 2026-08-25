# ADR-018 — OpenTelemetry

**Status:** Accepted, export backend superseded by `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`
· **Full rationale:** `docs/observability/observability.md` (Phase 24)

> **Amendment:** exports now target the self-hosted **Prometheus** (metrics)
> + **Grafana Loki** (logs) + **Grafana Tempo** (traces) + **Grafana OSS**
> (dashboards) stack instead of Azure Monitor/Application Insights. This is
> exactly the backend-portability payoff the original decision below
> banked on — the OpenTelemetry SDK choice needed zero changes to swap
> backends.

## Decision
OpenTelemetry SDK for all logs/metrics/traces, across `Host.Api`,
`Host.Workers`, every satellite service, and the React apps (Web SDK),
exported via the OTel Collector to Prometheus/Loki/Tempo, visualized in
Grafana.

## Why
Vendor-neutral instrumentation (avoids locking the entire codebase's
telemetry API to one backend), with W3C Trace Context propagation giving
end-to-end tracing across the outbox → RabbitMQ → satellite service chain
that a proprietary-SDK-only approach would fragment.

## Trade-offs
Slightly more setup than an Application-Insights-SDK-only approach;
accepted for the backend portability it preserves.

## Consequences
Every new process (including future extracted services) must wire
OpenTelemetry before being considered production-ready (Phase 54
Definition of Done).

## Rejected alternative
Application Insights SDK directly, no OpenTelemetry layer — rejected: ties
every instrumentation call site to one vendor's API with no abstraction
(and would have blocked the Prometheus/Loki/Tempo migration entirely).
