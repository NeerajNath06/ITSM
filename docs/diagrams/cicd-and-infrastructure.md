# Diagrams: CI/CD, Self-Hosted Production, Disaster Recovery, Observability

**Phase:** 42 · Diagrams 15–18. Infrastructure per
`docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`.

## 15. CI/CD

```mermaid
flowchart TD
  DEV[Developer] --> GIT[Gitea PR]
  GIT --> BUILD[Build .NET + React]
  BUILD --> TEST[Unit/Integration/Component tests]
  TEST --> SEC[Security scan: SAST, deps, secrets]
  SEC --> IMG[Container build]
  IMG --> REG[Registry - Harbor, image scan]
  REG --> DDEV[Deploy Dev - automatic]
  DDEV --> ITEST[Integration tests incl. isolation suite]
  ITEST --> DQA[Deploy Staging]
  DQA --> APPROVE[Manual approval]
  APPROVE --> PROD[Production - progressive rollout]
```
Source: `docs/devops/cicd.md` (Phase 39).

## 16. Self-hosted production architecture

```mermaid
flowchart TD
  EDGE[Caddy/Nginx + WAF] --> YARP[YARP Gateway] --> K8S[Kubernetes/K3s: Host.Api, Workers, satellites]
  K8S --> PG[(PostgreSQL)]
  K8S --> VALKEY[(Valkey)]
  K8S --> MQ[(RabbitMQ cluster)]
  K8S --> MINIO[(MinIO)]
  K8S --> OPENBAO[OpenBao]
  K8S -.-> KEYCLOAK[Keycloak]
  HARBOR[Harbor Registry] --> K8S
  K8S --> GRAFANA[Prometheus/Loki/Tempo/Grafana]
  K8S -.NetworkPolicy-isolated.-> PG & VALKEY & MINIO
```
Source: `docs/infrastructure/self-hosted-production-architecture.md` (Phase 25).

## 17. Disaster recovery

```mermaid
flowchart LR
  PRIMARY[Primary site] -->|continuous WAL + replicated backup| BACKUP[(Backup store)]
  PRIMARY -->|site outage| FAILOVER[Manual failover]
  FAILOVER --> SECONDARY[Secondary site: restore + redeploy via OpenTofu]
  SECONDARY -->|DNS cutover| EDGE[Caddy/Nginx edge]
  SECONDARY -.planned, monitored.-> FAILBACK[Failback to primary]
```
Source: `docs/architecture/disaster-recovery.md` (Phase 26).

## 18. Observability

```mermaid
flowchart TD
  LOGS[Structured Logs] --> OTEL[OpenTelemetry SDK]
  METRICS[Metrics] --> OTEL
  TRACES[Traces] --> OTEL
  OTEL --> COLLECTOR[OTel Collector]
  COLLECTOR --> PROM[Prometheus]
  COLLECTOR --> LOKI[Grafana Loki]
  COLLECTOR --> TEMPO[Grafana Tempo]
  PROM & LOKI & TEMPO --> GRAFANA[Grafana OSS]
  GRAFANA --> DASH[Dashboards]
  PROM --> ALERTS[Alerts / SLO burn-rate]
```
Source: `docs/observability/observability.md` (Phase 24).

---

*Next: `frontend-and-backend-architecture.md`.*
