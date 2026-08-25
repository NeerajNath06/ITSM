# Domain: Security (Security Incident Response)

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.
Deliberately **not** sharing the general Incident ACL domain even though it
extends the same `task` root (Phase 2/5 finding).

| | |
|---|---|
| **Purpose** | Detect, contain, and remediate security incidents — a distinct lifecycle and access model from IT service Incidents. |
| **Business Capability** | Security Incident Response |
| **Actors** | SOC Analyst, Incident Commander (security), Legal/Compliance (for breach-adjacent cases) |

**Entities/Aggregates:** `SecurityIncident` (extends `task`, isolated ACL
domain), `IndicatorOfCompromise` (IOC), `Playbook`.

**Commands/Queries/Events:** `CreateSecurityIncident`, `ContainThreat`,
`Eradicate`, `Recover`; `GetSecurityIncidentById`;
`SecurityIncidentContained`.

**APIs:** `/api/v1/security-incidents/*` — a **separate ACL domain** from
`/api/v1/incidents/*`, not merely a different `task_type` value under a
shared endpoint.

**Database tables:** `task` (shared base), `security_incident`, `ioc`,
`playbook` — same TPT pattern as Incident, different ACL enforcement point.

**Security:** Highest-isolation Task subtype in the platform — `task_type =
'SecurityIncident'` rows are excluded from general Incident ACL grants by
construction (a `security_admin`/`soc_analyst`-scoped role required), not
merely by convention.

**Audit:** Full audit, plus integration with legal-hold (a security
incident under investigation may trigger a legal hold on related records
across other domains).

**Workflow:** Playbook execution as a specialized Workflow Definition
family (detect → contain → eradicate → recover → lessons-learned).

**Dependencies:** Vulnerability data, CMDB, threat intel feeds (future
integrations), Compliance (breach notification obligations).

**Failure scenarios:** the exact scenario this isolation prevents — a
general ITSM agent querying `/api/v1/table/task` and incidentally seeing a
Security Incident row because it shares the base table. Mitigated by the
ACL engine's table-family-aware policy checking `task_type` before applying
"can read task" grants — general Task ACL grants must never implicitly
include `SecurityIncident` rows.

**KPIs:** MTTD, MTTC (mean time to contain).

**Build trigger:** only alongside a future SecOps product line
(`implementation-roadmap.md` Phase 8, Security) — and only with a dedicated
security architecture review given the isolation requirements above.

---

*Next: `docs/domains/risk.md`.*
