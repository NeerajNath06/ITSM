# Domain: CMDB

**Phase:** 40 · MVP scope. Full architecture in `docs/cmdb/cmdb-architecture.md`
(Phase 11) and `docs/cmdb/csdm-inspired-service-model.md` (Phase 10); this
document is the domain profile.

| | |
|---|---|
| **Purpose** | Be the single, trustworthy source of truth for what technology exists and how it relates, enabling impact analysis. |
| **Business Capability** | Technology & Service Mapping |
| **Actors** | CMDB Admin, Discovery service (system actor), architects/consumers (read-only: Incident/Change agents) |

**Use cases:** discovery ingests and identifies CIs; manual CI entry for
undiscoverable assets; relationship mapping (manual or inferred); impact
query from an Incident/Change record; data-quality remediation review.

**Entities/Aggregates:** `ConfigurationItem`, `CIRelationship`,
`BusinessService`, `ServiceOffering`, `Application` (a CI subtype).

**Commands:** `CreateCI`, `UpdateCI`, `RetireCI`, `CreateRelationship`,
`ReconcileDuplicates`.

**Queries:** `GetCIById`, `GetImpactGraph(ciId)`, `SearchCIs`.

**Events:** `CICreated`, `CIRelationshipChanged`, `CIRetired`.

**APIs:** `GET /api/v1/cmdb/cis/{id}/impact-graph`, `POST /api/v1/cmdb/cis`
(restricted to `cmdb_admin`/Discovery identity).

**Database tables:** `cmdb_ci`, `cmdb_relationship`, `cmdb_ci_class_schema`.

**Security:** Writes restricted to `cmdb_admin`/Discovery service identity
— the single highest-blast-radius write path in the operational domain
(Phase 2/11 finding), broadly readable otherwise.

**Audit:** Every CI/relationship mutation.

**Workflow:** none owned directly — CMDB is queried by Incident/Change
workflows, does not itself run approval-style processes (a future
"CI change approval" process, if added, would be a Change-domain concern
referencing CMDB, not a CMDB-owned workflow).

**Notifications:** data-quality alerts (staleness, duplicates) to CI
owner groups (`cmdb-data-quality-rules.md`).

**Dependencies:** none upstream (root context) — Discovery feeds it, but
CMDB does not depend on Discovery's *availability* for its data to remain
valid.

**Failure scenarios:** see `cmdb-data-quality-rules.md` in full — orphaned
relationships, duplicate CIs, stale discovery data.

**KPIs:** Data completeness %, relationship accuracy, discovery coverage %,
mean staleness.

---

*Next: `docs/domains/asset-management.md`.*
