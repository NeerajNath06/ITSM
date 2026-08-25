# ADR-000 — Technology Stack Supersession

**Status:** Accepted, Frontend row superseded by `ADR-021-react-over-angular.md`
**Date:** 2026-08-25
**Deciders:** Product owner (via explicit direction), Architecture

> **Amendment:** the Frontend decision below (Angular) was superseded by
> `ADR-021-react-over-angular.md`, which reinstates React + TypeScript.
> Every other row in this ADR stands unchanged. This document is left
> otherwise intact as the historical record of the original decision.

## Context

Two conflicting architecture inputs now exist in this repository:

1. `docs/ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md` (written first) locked:
   ASP.NET Core 10 + **React 19** + **SQL Server** + Redis + OpenSearch +
   Azure Service Bus (RabbitMQ on-prem alternative) + OIDC/SAML.
2. `docs/architecture/ServiceNow-Inspired Enterprise Architecture — Master
   AI Command.md` (written second, more specific and more recently
   authored) specifies: ASP.NET Core / **.NET 10** + **Angular** +
   **PostgreSQL** + Redis + **RabbitMQ** + **REST + gRPC** + OAuth2/OIDC +
   JWT + OpenTelemetry + Docker + **Azure** + Azure DevOps/GitHub Actions +
   Infrastructure as Code.

Per the Master AI Command's own instruction ("If an existing implementation
conflicts with the new architecture, document the conflict first instead of
silently rewriting it"), this ADR records the conflict and the resolution
before the full documentation program (`docs/servicenow-research/`,
`docs/database/`, `docs/rules/`, etc.) is generated against one consistent
stack.

## Decision

**The Master AI Command's stack is authoritative going forward.** All new
documentation produced from this point (the 55-phase program) targets:

| Layer | Decision |
|---|---|
| Backend | ASP.NET Core / .NET 10 Web API |
| Frontend | ~~Angular + TypeScript~~ → **React + TypeScript**, see `ADR-021-react-over-angular.md` |
| Database | PostgreSQL |
| Cache | Redis |
| Messaging | RabbitMQ |
| API | REST (external/UI) + gRPC (internal service-to-service, where justified) |
| AuthN/AuthZ | OAuth2 / OIDC + JWT |
| Observability | OpenTelemetry → Azure Monitor / Application Insights |
| Containerization | Docker |
| Cloud | Microsoft Azure |
| CI/CD | Azure DevOps or GitHub Actions |
| Infrastructure | Infrastructure as Code |
| Service topology | Modular monolith where appropriate + microservices where justified (per `microservice-boundary-rules.md`) |

The **process, principles, domain model, and platform-capability thinking**
in the original `ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md` (Task-as-base
model, CSDM-style CMDB, ACL+RLS tenancy, wave-based delivery discipline,
"don't build a microservice mesh by default") remain valid and are carried
forward — only the concrete technology choices (React→Angular, SQL
Server→PostgreSQL, Service Bus→RabbitMQ, REST-only→REST+gRPC) are
superseded.

## What this means for already-written documents

| Document | Status | Action |
|---|---|---|
| `docs/ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md` | Superseded on stack specifics; wave/process/domain content still valid | Not rewritten in place by this task — flagged here; a future pass should reconcile it or mark it historical once `docs/MASTER-ARCHITECTURE.md` (Phase 55) is the new single source of truth |
| `docs/architecture/01-security-architecture.md` | Principles (zero trust, ACL+RLS, encryption, audit) still valid; written assuming SQL Server RLS and OIDC/SAML | Superseded/extended by `docs/security/security-architecture.md` (Phase 17), which targets PostgreSQL RLS + OAuth2/OIDC/JWT |
| `docs/architecture/09-notification-oncall-escalation-engine.md` | Domain model (Group/Shift/Escalation/Device) and rules still valid; stack-agnostic | Superseded/extended by `docs/platform/notification-service.md` (Phase 20) |

No prior file is deleted. Where content is superseded, the newer
Phase-numbered document under its Master-AI-Command path is the one to
follow; this ADR is the pointer between them.

## Consequences

- PostgreSQL-specific patterns (JSONB, RLS via `CREATE POLICY`, partitioning) replace SQL Server-specific ones (native RLS via security policies, columnstore) wherever the two docs previously diverged.
- ~~Angular's module/component/service model replaces React's component/hook model in all frontend guidance from Phase 30 onward.~~ Superseded by `ADR-021`: React's component/hook model is retained after all — see that ADR.
- RabbitMQ becomes the default messaging technology; Azure Service Bus is not used unless a future ADR justifies it for a specific managed-service reason.
- A dedicated gRPC contract layer is introduced for internal service-to-service calls, which the original guide did not include (it was REST-only, Table/Domain/BFF).

## Rejected alternative

Keep the original guide's stack and treat the Master AI Command as
process/structure guidance only, translating each phase's content onto
React/SQL Server. Rejected because the user explicitly directed execution
against the Master AI Command's file as written, including its named
technology list.
