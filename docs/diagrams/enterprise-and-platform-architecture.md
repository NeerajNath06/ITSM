# Diagrams: Enterprise & Platform Architecture

**Phase:** 42 · Diagrams 1–2 of the required set. Reproduces/centralizes
diagrams that also live inline in their source phase documents (linked per
diagram) so this directory is a complete visual index.

## 1. Enterprise architecture

```mermaid
flowchart TD
  subgraph Experience
    ANG[React apps]
    MOB[Mobile / PWA]
  end
  subgraph Edge
    AFD[Caddy/Nginx + WAF]
    APIM[YARP Gateway]
  end
  subgraph Core["Host.Api modular monolith"]
    MODULES[Domain modules]
    KERNEL[Platform kernel]
  end
  subgraph Satellites
    WORKERS[Host.Workers]
    TEMPORAL[Temporal]
    SEARCH[Search indexer]
    NOTIFY[Notification service]
    AI[AI inference - Ollama]
  end
  DB[(PostgreSQL)]
  CACHE[(Valkey)]
  MQ[(RabbitMQ)]
  BLOB[(MinIO)]

  ANG --> AFD --> APIM --> MODULES
  MOB --> AFD
  MODULES --> KERNEL --> DB
  MODULES --> MQ --> WORKERS & SEARCH & NOTIFY
  MODULES --> BLOB
  KERNEL --> CACHE
```
Source: `docs/architecture/target-enterprise-architecture.md` (Phase 4).

## 2. Platform architecture (kernel capabilities)

```mermaid
flowchart LR
  subgraph Kernel["Platform Kernel — every module consumes, none reimplements"]
    ID[Identity]
    ACL[ACL/Authorization]
    AUD[Audit]
    TEN[Tenancy]
    CFG[Configuration]
  end
  subgraph Modules
    INC[Incident]
    CHG[Change]
    CAT[Catalog]
    CMDB[CMDB]
  end
  INC & CHG & CAT & CMDB --> ID
  INC & CHG & CAT & CMDB --> ACL
  INC & CHG & CAT & CMDB --> AUD
  INC & CHG & CAT & CMDB --> TEN
  INC & CHG & CAT & CMDB --> CFG
```
Source: `docs/architecture/target-enterprise-architecture.md` §Unified capabilities (Phase 4).

---

*Next: `domain-and-microservices-architecture.md`.*
