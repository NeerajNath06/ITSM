# Documentation Program Status

Tracks execution of the 55-phase program defined in
`docs/architecture/ServiceNow-Inspired Enterprise Architecture — Master AI
Command.md`. Updated as each phase's file(s) are delivered so work can
resume across sessions without re-deriving what's already done.

**Stack authority:** `docs/adr/ADR-000-technology-stack-supersession.md` —
.NET 10 + PostgreSQL + Redis + RabbitMQ + REST/gRPC + Azure supersedes the
earlier guide's SQL Server choice. Frontend: `docs/adr/ADR-021-react-over-angular.md` —
React + TypeScript (reverting `ADR-000`'s Angular choice back to the
original guide's React decision).

| Phase | Deliverable(s) | Status |
|---|---|---|
| 0 | Research discipline (embedded in Phase 1 doc header) | ✅ |
| 1 | `docs/servicenow-research/product-landscape.md` | ✅ |
| 2 | `docs/servicenow-research/product-capability-matrix.md` | ✅ |
| 3 | `docs/architecture/servicenow-reference-architecture.md`, `servicenow-layered-architecture.md` | ✅ |
| 4 | `docs/architecture/target-enterprise-architecture.md` | ✅ |
| 5 | `docs/architecture/bounded-contexts.md` | ✅ |
| 6 | `docs/architecture/microservice-boundary-rules.md` | ✅ |
| 7 | `docs/database/database-architecture.md` | ✅ |
| 8 | `docs/database/shared-enterprise-data-model.md` | ✅ |
| 9 | `docs/domain/task-model.md` | ✅ |
| 10 | `docs/cmdb/csdm-inspired-service-model.md` | ✅ |
| 11 | `docs/cmdb/cmdb-architecture.md`, `cmdb-data-quality-rules.md` | ✅ |
| 12 | `docs/workflow/workflow-engine-architecture.md` | ✅ |
| 13 | `docs/workflow/business-rule-engine.md` | ✅ |
| 14 | `docs/integration/event-driven-architecture.md` | ✅ |
| 15 | `docs/api/rest-grpc-rules.md` | ✅ |
| 16 | `docs/api/api-design-rules.md` | ✅ |
| 17 | `docs/security/security-architecture.md` | ✅ |
| 18 | `docs/architecture/multi-tenancy.md` | ✅ |
| 19 | `docs/audit/audit-architecture.md` | ✅ |
| 20 | `docs/platform/notification-service.md` | ✅ |
| 21 | `docs/platform/search-architecture.md` | ✅ |
| 22 | `docs/platform/knowledge-management.md` | ✅ |
| 23 | `docs/platform/file-storage.md` | ✅ |
| 24 | `docs/observability/observability.md` | ✅ |
| 25 | `docs/azure/production-architecture.md` | ✅ |
| 26 | `docs/architecture/disaster-recovery.md` | ✅ |
| 27 | `docs/governance/data-governance.md` | ✅ |
| 28 | `docs/platform/configuration-management.md` | ✅ |
| 29 | `docs/platform/feature-flags.md` | ✅ |
| 30 | `docs/frontend/react-architecture.md` (originally Angular; superseded by `ADR-021`) | ✅ |
| 31 | `docs/backend/dotnet-architecture.md` | ✅ |
| 32 | `docs/rules/coding-rules.md` | ✅ |
| 33 | `docs/rules/database-rules.md` | ✅ |
| 34 | `docs/rules/api-rules.md` | ✅ |
| 35 | `docs/rules/microservice-rules.md` | ✅ |
| 36 | `docs/rules/security-rules.md` | ✅ |
| 37 | `docs/rules/git-rules.md` | ✅ |
| 38 | `docs/testing/testing-strategy.md` | ✅ |
| 39 | `docs/devops/cicd.md` | ✅ |
| 40 | `docs/domains/*.md` (18 per-domain docs) | ✅ |
| 41 | `docs/adr/ADR-001..020` | ✅ (plus ADR-000 and ADR-021, out-of-band) |
| 42 | `docs/diagrams/*` (7 files, 20 diagrams) | ✅ |
| 43 | `docs/roadmap/implementation-roadmap.md` | ✅ |
| 44 | `docs/roadmap/mvp.md` | ✅ |
| 45 | `docs/architecture/repository-structure.md` | ✅ |
| 46 | `docs/ai/ai-architecture.md` | ✅ |
| 47 | `docs/ai/ai-governance.md` | ✅ |
| 48 | `docs/platform/service-catalog.md` | ✅ |
| 49 | `docs/platform/sla-engine.md` | ✅ |
| 50 | `docs/governance/architecture-governance.md` | ✅ |
| 51 | `CLAUDE.md` | ✅ |
| 52 | `docs/rules/ai-agent-rules.md` | ✅ |
| 53 | `docs/rules/anti-patterns.md` | ✅ |
| 54 | `docs/governance/definition-of-done.md` | ✅ |
| 55 | `docs/MASTER-ARCHITECTURE.md` | ✅ |

**PROGRAM COMPLETE — 55/55 phases delivered.**

## Post-completion amendments

| Date/Trigger | Change | Files touched |
|---|---|---|
| User direction: use React, not Angular | `docs/adr/ADR-021-react-over-angular.md` created, superseding the Frontend row of `ADR-000`. `docs/frontend/angular-architecture.md` → superseded stub; content moved to `docs/frontend/react-architecture.md`. Every other document naming "Angular" as the frontend technology updated to "React" (terminology only — no other architectural decision changed). | `CLAUDE.md`, `MASTER-ARCHITECTURE.md`, `target-enterprise-architecture.md`, `repository-structure.md`, `multi-tenancy.md`, `servicenow-layered-architecture.md`, `security/security-architecture.md`, `api/api-design-rules.md`, `api/rest-grpc-rules.md`, `testing/testing-strategy.md`, `devops/cicd.md`, `platform/service-catalog.md`, `platform/search-architecture.md`, `platform/notification-service.md`, `platform/feature-flags.md`, `rules/security-rules.md`, `rules/api-rules.md`, `rules/coding-rules.md`, `rules/anti-patterns.md`, `rules/ai-agent-rules.md`, `roadmap/implementation-roadmap.md`, `adr/ADR-005/009/018`, all `diagrams/*` files referencing the frontend |
| User direction: self-hosted OSS infrastructure stack (full propagation verified via repo-wide sweep for "Azure", "Redis", "Key Vault", "Blob Storage", "Container Apps", "Application Insights", "API Management", "Front Door", "Entra ID" — all remaining hits are intentional historical/amendment text or the two documented-alternative Azure docs) (Valkey, OpenBao, Keycloak, MinIO, OpenSearch, YARP, Caddy/Nginx, Temporal, Hangfire/Quartz.NET, OpenFeature+Unleash, Grafana stack, Kubernetes/K3s, Harbor, OpenTofu, Jenkins/Gitea Actions+Gitea, Mailpit, ClamAV, Ollama, Qdrant, MkDocs Material) | `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md` created (supersedes the Cloud/CI-CD rows of `ADR-000` and the infra-product rows of `ADR-014/015/017/018/019/020`, each amended in place with a pointer). `docs/adr/ADR-023-temporal-for-workflow-execution.md` created as an explicit synthesis — Temporal becomes the durable-execution runtime while the metadata-driven, tenant-editable Workflow Definition graph (`ADR-013`) is preserved as the authoring layer, interpreted by one generic `GraphExecutorWorkflow`; this was a genuine paradigm conflict (workflow-as-data vs. workflow-as-code), documented rather than silently resolved either direction. New primary infra doc: `docs/infrastructure/self-hosted-production-architecture.md`, superseding `docs/azure/production-architecture.md` as the default (Azure kept as a documented alternative, not deleted). | `CLAUDE.md`, `MASTER-ARCHITECTURE.md`, `target-enterprise-architecture.md`, `repository-structure.md`, `security/security-architecture.md`, `platform/file-storage.md` (rewritten for MinIO/ClamAV), `platform/search-architecture.md` (OpenSearch promoted to default), `observability/observability.md` (Grafana stack), `platform/feature-flags.md` (OpenFeature+Unleash), `platform/configuration-management.md`, `platform/notification-service.md`, `devops/cicd.md`, `ai/ai-architecture.md` (Ollama/Qdrant), `backend/dotnet-architecture.md` (Hangfire/Quartz.NET), `architecture/microservice-boundary-rules.md` (Workflow row), `architecture/disaster-recovery.md`, `architecture/multi-tenancy.md`, `rules/security-rules.md`, `testing/testing-strategy.md`, `roadmap/implementation-roadmap.md`, `adr/ADR-003/004/016`, `diagrams/enterprise-and-platform-architecture.md`, `diagrams/cicd-and-infrastructure.md`, `workflow/workflow-engine-architecture.md` (superseding note) |

## Pre-existing documents (from before this program; status per ADR-000)

| Document | Relationship to this program |
|---|---|
| `docs/ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md` | Stack superseded; wave/process/domain thinking still referenced |
| `docs/architecture/00-INDEX.md`, `01-security-architecture.md`, `09-notification-oncall-escalation-engine.md` | Superseded/extended by Phase 17 and Phase 20 respectively; not deleted |

---

*Batches are delivered in dependency order (research → architecture →
data/domain → platform capabilities → rules → domain docs/ADRs/diagrams →
roadmap → governance → CLAUDE.md → master doc), per the phase numbering
above, not necessarily 1-2-3 strictly file-by-file within a batch.*
