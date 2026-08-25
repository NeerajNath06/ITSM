# Diagrams: Database & CMDB / CSDM Architecture

**Phase:** 42 · Diagrams 5, 13, 14.

## 5. Database architecture (hybrid topology)

```mermaid
flowchart TD
  subgraph Core["Core PostgreSQL database — shared schema, RLS-partitioned"]
    T[task / incident / problem / change / requested_item]
    C[cmdb_ci / cmdb_relationship]
    I[identity_user / identity_group]
    A[audit_log - partitioned]
  end
  subgraph SatDBs["Satellite databases — one each"]
    NDB[(Notification DB)]
    SDB[(Search index store)]
    ADB[(AI inference DB)]
  end
  Core -.events only, never direct query.-> SatDBs
```
Source: `docs/database/database-architecture.md` (Phase 7).

## 13. CMDB architecture (CI graph)

```mermaid
flowchart LR
  CI1[CI: App Server] -->|runs on| CI2[CI: VM]
  CI2 -->|hosted by| CI3[CI: Hypervisor]
  CI1 -->|depends on| CI4[CI: Database]
  APP[Application: Order Mgmt] -->|supports| BS[Business Service: Order Processing]
  CI1 -.is a.-> APP
```
Source: `docs/cmdb/cmdb-architecture.md` (Phase 11).

## 14. CSDM-inspired architecture (service layering)

```mermaid
flowchart TD
  BS[Business Service] -->|offered as| SO[Service Offering]
  BS -->|supported by| APP[Business Application]
  APP -->|runs on| CI[Configuration Item]
  CI -->|depends on| CI
  SO -->|has| SLA[SLA Definition]
```
Source: `docs/cmdb/csdm-inspired-service-model.md` (Phase 10).

---

*Next: `security-and-auth-flows.md`.*
