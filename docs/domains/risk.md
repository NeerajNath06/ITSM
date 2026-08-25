# Domain: Risk

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.

| | |
|---|---|
| **Purpose** | Maintain an enterprise risk register: identified risks, their controls, and assessment status. |
| **Business Capability** | Enterprise Risk Management |
| **Actors** | Risk Manager, Control Owner |

**Entities/Aggregates:** `Risk`, `Control`, `Assessment`.

**Commands/Queries/Events:** `RegisterRisk`, `LinkControl`,
`RecordAssessment`; `GetRiskRegister`; `RiskRegistered`,
`AssessmentCompleted`.

**APIs:** `/api/v1/risk/*`.

**Database tables:** `risk`, `control`, `risk_control_link`, `assessment`.

**Security:** Risk register visibility typically restricted to
governance/leadership roles — not broadly agent-visible like ITSM data.

**Audit:** Every assessment and control-linkage change (this data is
frequently audited externally, e.g. by regulators).

**Workflow:** periodic reassessment scheduling as a Workflow Definition
triggered on a cron.

**Dependencies:** Compliance (shares control data), CMDB (risk may
reference affected services/CIs), Security (security-originated risks).

**Failure scenarios:** a control marked "effective" with no recent
assessment — mitigated by staleness alerts analogous to CMDB's data-quality
staleness rule (Phase 11), reused as a pattern, not reimplemented.

**KPIs:** Risk exposure trend, control effectiveness %, overdue
assessments.

**Build trigger:** only alongside a future GRC/IRM product line
(`implementation-roadmap.md` Phase 8, Security, broadly construed).

---

*Next: `docs/domains/compliance.md`.*
