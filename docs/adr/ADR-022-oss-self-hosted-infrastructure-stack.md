# ADR-022 — OSS-First, Self-Hosted Infrastructure Stack

**Status:** Accepted · **Supersedes:** the Cloud and CI/CD rows of
`ADR-000`, and the infrastructure-technology rows of `ADR-014` (Search),
`ADR-015` (Container platform), `ADR-017` (Cache), `ADR-018`
(Observability backend), `ADR-019` (Object storage), `ADR-020` (API
Gateway) — each of those ADRs' original *reasoning* (why a cache is
needed, why RLS-aware search-trimming matters, why a gateway centralizes
rate limiting) stays valid; only the *product* named in each is replaced.
Workflow execution is addressed separately in
`ADR-023-temporal-for-workflow-execution.md` because it is not a simple
product swap.

## Context

`ADR-000`/`ADR-025` (Azure production architecture) assumed a fully
Azure-managed-PaaS deployment (Front Door, API Management, Container Apps,
Key Vault, Blob Storage, Application Insights). The user has since
directed a specific, comprehensive open-source, self-hostable stack
instead — one that runs identically on-prem, on bare-metal, or on any
cloud's plain VMs/Kubernetes, without depending on any single cloud
vendor's managed services.

## Decision

Adopt the following stack. `.NET 10`/PostgreSQL/RabbitMQ/REST+gRPC/
OAuth2-OIDC-JWT/OpenTelemetry/Docker (already locked) are unaffected; the
table below is what changes.

| Requirement | Was (Azure-managed) | Now (self-hosted OSS) |
|---|---|---|
| Cache | Azure Cache for Redis | **Valkey** (Redis-protocol-compatible fork — no application-code change, only the deployed binary/image changes) |
| Secrets | Azure Key Vault | **OpenBao** (Vault-compatible fork) |
| Identity / IAM | Microsoft Entra ID (federation target) | **Keycloak** as the self-hosted IdP (still speaks OIDC/SAML — our platform token service federates with it exactly as it would with any OIDC IdP; enterprise tenants may still federate their *own* Entra ID/Okta tenant into Keycloak as an upstream identity broker, so this does not remove enterprise-SSO capability) |
| Object storage | Azure Blob Storage | **MinIO** (S3-compatible) — our design already used signed-URL upload/download and a provider-abstracted metadata table (`docs/platform/file-storage.md`), so this is a low-impact swap |
| Search | PostgreSQL FTS → (deferred) Azure AI Search | **OpenSearch**, promoted to the primary/default choice from day one — supersedes `ADR-014`'s "start with Postgres FTS" reasoning; see below |
| API Gateway / reverse proxy | Azure Front Door + API Management | **YARP** (in-process/edge reverse proxy + rate limiting, .NET-native) fronted by **Caddy** or **Nginx** for TLS/edge termination |
| Container platform | Azure Container Apps | **Docker Compose** for local/dev, **Kubernetes** (via **K3s** for lighter-weight clusters) for production |
| Kubernetes UI | — | **Headlamp** |
| Container registry | Azure Container Registry | **Harbor** |
| IaC | Azure-specific Bicep/Terraform | **OpenTofu** |
| CI/CD | Azure DevOps / GitHub Actions | **Jenkins** or **Gitea Actions** |
| Git server | GitHub | **Gitea** |
| Observability backend | Azure Monitor / Application Insights | **Prometheus** (metrics) + **Grafana Loki** (logs) + **Grafana Tempo** (traces) + **Grafana OSS** (dashboards) — OpenTelemetry (already locked, `ADR-018`) is the instrumentation layer regardless of backend, so no application code changes, only the OTel Collector's export target |
| Workflow orchestration | Custom-only (Phase 12) | **Temporal**, integrated per `ADR-023` |
| Background jobs | `Host.Workers` `BackgroundService`s only | `Host.Workers` implemented with **Hangfire** or **Quartz.NET** for scheduled/recurring job execution (refines, does not replace, the Phase 31 `Host.Workers` design) |
| Feature flags | Azure App Configuration feature management | **OpenFeature** (vendor-neutral SDK/API) + **Unleash** (self-hosted flag server) |
| Local email (dev only) | — | **Mailpit** — local dev/test SMTP capture, added to `deploy/docker-compose.yml` |
| Antivirus | Unnamed "virus scan" placeholder | **ClamAV** — names the concrete engine behind the quarantine-scan pipeline already designed in `docs/platform/file-storage.md` |
| AI model runtime | Azure OpenAI only | **Ollama** for local/self-hosted model inference, as the default; a cloud model provider remains pluggable via the existing `IModelProvider` abstraction (`docs/ai/ai-architecture.md`) for tenants/deployments that want a larger hosted model |
| Vector DB | Deferred (`pgvector` candidate) | **Qdrant** — supersedes the "evaluate `pgvector` first" note in `docs/platform/search-architecture.md` §RAG |
| Documentation site | — | **MkDocs Material** — publishes this `docs/` tree as a browsable site |
| API docs | OpenAPI/Swagger (unchanged) | OpenAPI/Swagger (unchanged) |
| Testing | xUnit + Testcontainers (unchanged) | xUnit + Testcontainers (unchanged) |

## Why

- **No single-vendor lock-in.** Every layer is either OSS-self-hosted or a
  protocol-compatible fork of a well-known product, so the platform is
  deployable on any infrastructure a customer requires (on-prem, air-gapped,
  a specific sovereign cloud) — directly relevant for the enterprise/
  regulated tenant tier already designed into `docs/architecture/multi-tenancy.md`.
- **Cost control at scale.** Managed PaaS per-operation/per-GB pricing
  (Key Vault operations, Application Insights ingestion, Blob transactions)
  is replaced by fixed self-hosted infrastructure cost, which is more
  predictable for a multi-tenant SaaS's largest cost driver.
- **Protocol compatibility minimizes application-code impact.** Valkey
  speaks the Redis protocol, OpenBao speaks the Vault API, MinIO speaks S3,
  Keycloak speaks OIDC/SAML — our code already programs against those
  protocols/abstractions (`IModelProvider`-style interfaces throughout this
  program), not against an Azure SDK, so most of this swap is a deployment
  and configuration change, not an application rewrite.

## Trade-offs

- **We now operate every layer of our own stack** (identity, secrets,
  object storage, search, observability, CI/CD, git hosting) instead of
  consuming managed equivalents — real, ongoing operational staffing cost
  that a managed-PaaS approach would have avoided. This is the same
  trade-off already flagged for RabbitMQ in `ADR-004`/`ADR-016`, now
  extended across the whole stack deliberately, not by accident.
- **Kubernetes/K3s** replaces Container Apps' simpler scale-to-zero
  serverless model with a platform the team must operate (control plane
  upgrades, node management) — the exact concern `ADR-015` originally
  raised against AKS now applies to K3s too, accepted because the
  self-hosting goal requires it.
- Two acceptable CI/CD options are named (Jenkins or Gitea Actions) rather
  than one — a follow-up ADR should pick one before Wave 0 scaffolding to
  avoid running two CI systems by default drift.

## Consequences

- `docs/azure/production-architecture.md` is retitled/reframed: Azure is
  no longer the assumed deployment target. See
  `docs/infrastructure/self-hosted-production-architecture.md` (new,
  supersedes it as the primary reference); the Azure document is kept as a
  documented *alternative* deployment target (a tenant or environment that
  specifically wants Azure PaaS is not architecturally forbidden — the
  abstractions this program already built, e.g. `IModelProvider`,
  `INotificationProvider`, `ISearchProvider`, are exactly what make that
  still possible — but it is no longer the default).
- `docs/security/security-architecture.md`, `docs/platform/file-storage.md`,
  `docs/platform/search-architecture.md`, `docs/observability/observability.md`,
  `docs/devops/cicd.md`, `docs/platform/feature-flags.md`,
  `docs/ai/ai-architecture.md` are updated to name the new default
  technology, per file.
- `CLAUDE.md` and `docs/MASTER-ARCHITECTURE.md` stack lines updated.

## Rejected alternative

Keep the Azure-managed stack as default and treat the OSS list as a
documented alternative only — rejected per explicit user direction; the OSS
list is the default, Azure-managed becomes the documented alternative
(the inverse of what `ADR-000`/Phase 25 originally established).
