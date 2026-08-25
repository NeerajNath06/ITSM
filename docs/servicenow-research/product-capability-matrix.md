# Product Capability Matrix

**Phase:** 2 · Builds on `product-landscape.md` (Phase 1).

Scope: full matrix for products that are in-scope or dependency-adjacent to
our platform (ITSM, ITAM, CMDB, and their direct neighbors). Products
outside our scope (CSM, FSM, HRSD, SecOps) get a lighter entry to preserve
the architectural pattern evidence without over-investing in products we are
not building.

## Matrix

| Product | Business Problem | Primary Users | Core Entities | Core Workflows | Dependencies | Integrations | AI Usage | Security Concerns | KPIs |
|---|---|---|---|---|---|---|---|---|---|
| **Incident Management** | Restore service fast after unplanned disruption | Agents, requesters, major-incident commanders | Incident (`task` subtype), Journal, Related CI | Triage → assign → diagnose → resolve → close; major incident overlay | CMDB (impact/CI), Knowledge, SLA engine | Monitoring tools (alert→incident), Notification/on-call, ChatOps | Classification/routing suggestion, similar-incident search, resolution summarization | Field-level ACL on caller PII; audit on assignment/priority changes | MTTR, FCR, reopen rate, SLA attainment |
| **Problem Management** | Eliminate root cause of recurring incidents | Problem managers, SMEs | Problem, Known Error, linked Incidents | Investigate → root cause → known error → fix → closure | Incident (linkage), CMDB, Change (for permanent fix) | None typically external | RCA drafting assist | Access to known-error DB may be broader than incident detail | Problem backlog age, recurrence rate |
| **Change Enablement** | Control risk of intentional infrastructure/application changes | Change requesters, CAB, implementers | Change (`task` subtype: standard/normal/emergency), CAB record | Assess → authorize → schedule → implement → review | CMDB (impact graph), Release/DevOps pipeline, Calendar (blackout windows) | CI/CD pipelines, calendar systems | Risk scoring from CI criticality + history | Segregation of duties (requester ≠ sole approver); audit is compliance-critical | Change success rate, unauthorized-change count |
| **Request / Service Catalog** | Standardize how employees ask for things and how fulfillment happens | Requesters, fulfillers | Catalog Item, Cart, Requested Item, Catalog Task | Browse → configure variables → checkout → fulfill | Approval engine, Knowledge, Notification | Provisioning systems (identity, hardware) | Item recommendation, form pre-fill | Entitlement checks (who can order what) | Cycle time, catalog adoption |
| **Knowledge Management** | Make institutional knowledge findable and self-service-able | Authors, reviewers, all employees (readers) | Article, Category, Version | Draft → review → publish → retire; feedback loop | Search, Incident/Case (attach-to) | None typically external | RAG grounding source, AI-drafted articles pending human publish | Confidential articles need field/role scoping | Deflection rate, article feedback score |
| **CMDB** | Single source of truth for what technology exists and how it relates | Architects, ITOM/ITSM/ITAM consumers | CI, CI Class, Relationship | Populate (discovery/manual) → reconcile → maintain → consume | Discovery/Service Mapping for population | Cloud provider APIs, discovery agents | Anomaly detection on drift | Highest blast-radius data — a bad CI can mis-route every dependent workflow | Data completeness %, relationship accuracy |
| **Discovery** | Automatically populate/refresh CMDB from real infrastructure | Platform/infra teams | Discovered device/service payloads | Scan → identify → classify → write to CMDB | Network/cloud reachability, credentials store | On-prem collector agent, cloud provider APIs | Classification assist | Credential handling for scanning is highest-sensitivity secret in the platform | Discovery coverage %, staleness |
| **ITOM Event Management** | Reduce alert noise, correlate events into actionable signal | NOC/SRE | Event, Alert, Correlation rule | Ingest → correlate → group → (optionally) auto-open incident | CMDB, monitoring tool integrations | Prometheus/Datadog/cloud-native monitors (illustrative — verify per deployment) | Correlation/clustering, noise reduction | High ingestion volume — rate limiting and tenant isolation under load | Alert-to-incident ratio, noise reduction % |
| **IT Asset Management (Hardware + Software)** | Track lifecycle, cost, and license compliance of assets | Asset managers, procurement, finance | Asset, Model, License, Contract | Procure → receive → assign → maintain → retire/dispose | CMDB (asset↔CI link, not merged), Procurement, Contract | ERP/procurement systems | License optimization suggestions | Cost/contract data is commercially sensitive | License compliance %, cost per asset, unused-license spend |
| **SLA Engine** | Enforce and report on time-bound service commitments | Agents (visibility), managers (reporting) | SLA Definition, SLA Instance, Business Calendar | Attach to task → start/pause/stop per condition → breach event | Task engine, Notification | None typically external | Breach-risk prediction | SLA definitions themselves are configuration, not customer data, but breach data is sensitive to the customer | Attainment %, breach count, time-to-breach |
| **Notification Platform** | Reliable multi-channel delivery with on-call escalation | All users (recipients), on-call responders | Alert, Group, Shift, Device, Delivery Attempt | Trigger → target resolution → device fan-out → escalate if unacked | Task engine (source events), Workflow | SMS/voice/push/email providers, Teams/Slack | None typically (delivery infra), occasionally smart-routing | Device PII (phone numbers), delivery audit | Delivery latency, ack rate |
| **Customer Service Management** *(out of scope — pattern only)* | External customer issue resolution | Customer agents, customers | Case (`task` subtype), Account, Entitlement | Intake → triage → resolve → survey | Entitlement/contract data, Knowledge | Customer portals, telephony | Case summarization, deflection bots | Customer PII, contractual entitlement checks | CSAT, case resolution time |
| **HR Service Delivery** *(out of scope — pattern only)* | Employee HR case handling | HR agents, employees | HR Case (`task` subtype), Employee, Lifecycle Event | Intake → route to HR specialty → resolve | Employee master data, Knowledge (HR) | HRIS/payroll systems | HR-specific deflection | Highest PII sensitivity in the whole platform (compensation, health) — strictest field-level ACL needed | Case resolution time, employee satisfaction |
| **Security Incident Response** *(out of scope — pattern only)* | Contain/remediate security incidents, distinct from service incidents | SOC analysts | Security Incident (`task` subtype, isolated ACL domain), IOC | Detect → triage → contain → eradicate → recover → lessons learned | Vulnerability data, CMDB, threat intel feeds | SIEM, EDR, threat intel platforms | Playbook automation, IOC enrichment | Must NOT share the general Incident table's access model — separate ACL domain even though same `task` ancestor | MTTD, MTTC |

---

## What each product owns vs. consumes (summary)

| Product | Owns | Reads (does not own) |
|---|---|---|
| Incident | Incident record, its journal | CI (from CMDB), Knowledge, SLA definitions |
| Problem | Problem/Known Error record | Related Incidents (read+link, doesn't own) |
| Change | Change record, CAB decision | CI impact graph (CMDB), pipeline metadata (DevOps) |
| Catalog/Request | Catalog Item, Requested Item | Approval outcomes (Workflow), fulfiller assignment (Task engine) |
| CMDB | CI, CI Class, Relationship | Nothing upstream — CMDB is a root system of record |
| ITAM | Asset, License, Contract | CI (link only, not owned) |
| Knowledge | Article | Feedback/ratings from consuming products |
| Notification | Alert, Group, Shift | Source event data (owned by the triggering product) |
| SLA Engine | SLA Definition, SLA Instance | Task state transitions (owned by Task engine) |

**Key structural finding carried into Phase 5 (bounded contexts):** almost
every product either **extends `task`** or **reads CMDB without owning
it**. The two true "root" systems of record in the whole landscape are
**Task** (system of work) and **CMDB** (system of record for technology) —
this single fact drives most of the bounded-context and database decisions
in later phases.

---

*Next: `docs/architecture/servicenow-reference-architecture.md` and
`servicenow-layered-architecture.md` (Phase 3).*
