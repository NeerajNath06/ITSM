# ADR-015 — Azure Container Apps vs. AKS

**Status:** Superseded by `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`
· **Full rationale:** `docs/infrastructure/self-hosted-production-architecture.md`
(replaces `docs/azure/production-architecture.md` as primary reference)

> **Amendment:** the self-hosted stack direction (`ADR-022`) rules out
> Azure Container Apps as a managed PaaS dependency. The "fallback" this
> ADR named — Kubernetes — is now the primary decision: **Kubernetes**,
> via **K3s** for lighter-weight clusters, is used for production, with
> **Docker Compose** for local/dev. The trade-off this ADR originally
> accepted against AKS (operating a Kubernetes control plane) is now
> accepted deliberately as part of the broader self-hosting trade-off
> `ADR-022` documents, not avoided.

## Decision
Azure Container Apps (ACA) for `Host.Api`, `Host.Workers`, and all
satellite services.

## Why
Serverless-container scale-to-zero for lightly-loaded satellites, built-in
service discovery, and materially lower operational overhead than running a
Kubernetes control plane — appropriate for our topology (a modular monolith
+ a handful of satellites, not a large fleet of independently-orchestrated
microservices needing AKS-grade scheduling control).

## Trade-offs
Less fine-grained orchestration control (custom schedulers, complex sidecar
service-mesh patterns) than AKS; accepted because our current topology
doesn't need that control, and re-evaluated (per this ADR) if it later does.

## Consequences
No Kubernetes manifests/Helm charts to maintain at this stage; deployment
described via ACA-native configuration + Infrastructure as Code.

## Rejected alternative
AKS — not rejected permanently; explicitly named as the fallback if our
service count/orchestration complexity grows beyond what ACA comfortably
handles.
