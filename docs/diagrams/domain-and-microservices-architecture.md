# Diagrams: Domain & Microservices Architecture

**Phase:** 42 · Diagrams 3–4.

## 3. Domain architecture (bounded contexts and their relationships)

```mermaid
flowchart TD
  TENANT[Tenant] --> TASK[Task base]
  TASK --> INC[Incident]
  TASK --> PRB[Problem]
  TASK --> CHG[Change]
  TASK --> REQ[RequestedItem]
  CMDB[CMDB] -.relates-to.-> INC
  CMDB -.relates-to.-> CHG
  ASSET[Asset] -.relates-to.-> CMDB
  PRB -.links.-> INC
  CAT[Service Catalog] --> REQ
  KB[Knowledge] -.attach-to.-> INC
  WF[Workflow] -.orchestrates.-> INC & CHG & REQ
  NOTIFY[Notification] -.alerts.-> INC & CHG
```
Source: `docs/architecture/bounded-contexts.md` (Phase 5).

## 4. Microservices architecture (what's actually extracted)

```mermaid
flowchart LR
  subgraph Monolith["Host.Api — everything NOT listed below"]
  end
  subgraph Extracted["Extracted satellite services"]
    NOTIFY[Notification]
    SEARCH[Search indexer]
    AI[AI inference]
    DISCOVERY["Discovery (extracted from day one — runs at network edge)"]
  end
  Monolith <-->|events + REST/gRPC| Extracted
```
Source: `docs/architecture/microservice-boundary-rules.md` (Phase 6)
decision matrix.

---

*Next: `database-and-cmdb-architecture.md`.*
