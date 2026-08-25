# ServiceNow Product Landscape

**Phase:** 1 · **Source discipline:** official documentation first
([servicenow.com/docs](https://www.servicenow.com/docs),
[servicenow.com/platform](https://www.servicenow.com/platform.html),
[servicenow.com/products](https://www.servicenow.com/products.html));
secondary sources used only where official docs did not surface a detail,
each marked. Items not directly confirmed against primary docs during this
pass are marked `REQUIRES_VALIDATION`.

**Purpose:** a capability inventory of what ServiceNow actually ships, so
later phases derive our platform from real architectural pressure ServiceNow
solved — not from guessing what a "ServiceNow-like" product should contain.

---

## 0. How ServiceNow organizes its catalog

ServiceNow's own current framing (per [servicenow.com/platform.html](https://www.servicenow.com/platform.html)
and the AI Platform overview) is **one platform; one architecture; one data
model** — every product below is an application built on the same Now
Platform kernel (table schema, workflow engine, ACL model, UI framework),
not a separately-architected product. This is the single most important
architectural fact to carry forward: **the product boundaries are business
boundaries, not deployment or database boundaries.**

Cloud infrastructure model: ServiceNow runs a **multi-instance, logically
single-tenant architecture** — each customer gets application- and
database-layer isolation, not a shared-schema multi-tenant model. `[SOURCE: servicenow.com/platform architecture overviews, secondary aggregation — REQUIRES_VALIDATION against primary docs.servicenow.com instance architecture page]`

Commercially, ServiceNow groups its catalog into workflow families
(technology, employee, customer, creator/platform, security & risk, finance
& supply chain) for licensing purposes. `[SOURCE: secondary — industry analysis, REQUIRES_VALIDATION]`. The architectural inventory below is
organized by the same axis the Master AI Command uses (Platform / IT /
Customer / Employee / Security / Enterprise) because that grouping maps more
directly to bounded contexts (Phase 5) than the commercial SKU grouping does.

---

## 1. Platform (Now Platform / AI Platform)

| Concept | What it is | Why it exists | Architectural lesson |
|---|---|---|---|
| **Now Platform** | The shared PaaS kernel: table schema, ACL engine, UI framework, workflow engine, scripting | Every product is an app on this kernel — avoids re-solving data model/security/UI per product | One data model + one security model, reused everywhere, is what makes cross-product joins (incident↔CI↔change) cheap |
| **AI Platform layer (2026)** | Generative assistance in workflows, autonomous AI agents, and an **AI Control Tower** governing usage/policy/risk | AI capability needed a governance layer, not just a feature | AI governance (who can call what, with what data, audited) is itself a platform capability, not per-app logic — see Phase 46/47 |
| **Context Engine** | Supplies enterprise context (org, CMDB, process state) to AI decisions | Grounding AI in real operational data instead of prompt-only context | Any AI feature we build needs a server-side context/grounding layer, never client-assembled context |
| **Application Engine / App Engine Studio** | Low-code app-building tools on top of the table schema | Lets customers extend the platform without forking code | Confirms: metadata-driven tables/forms are how the platform stays extensible without a release cycle |
| **Workflow Automation / Flow Designer** | Declarative workflow builder: triggers, actions, approvals | Business logic as data (versioned, no-code), not hardcoded server logic | Workflow-as-data is a platform capability all products consume — see Phase 12 |
| **Business Rules / scripted automation** | Server-side scripts triggered on table events (insert/update/query) | Escape hatch for logic that doesn't fit declarative workflow | We need an equivalent extensibility point, but must bound it (see anti-pattern: business logic scattered across layers) |
| **IntegrationHub / Automation Engine** | Low-code integration + RPA-style automation connectors | Standardizes how external systems are called from workflow | Integration is a platform capability (spoke/connector model), not per-product bespoke code |
| **APIs (Table API, Scripted REST, Import Set, Attachment API, Aggregate API)** | Generic + custom REST surfaces | Table API = generic CRUD w/ ACL; Scripted REST = business outcomes | Confirms the two-tier API pattern (generic table access + purpose-built domain endpoints) is proven at scale |
| **Data model (CMDB/CSDM tables, `task` base table, `sys_*` metadata tables)** | Shared base tables every app extends | Avoids duplicating "what is a record, who owns it, when was it changed" per app | `task` as a shared base for Incident/Problem/Change/Request is the pattern behind our Phase 9 Task model |
| **Security model** | ACLs (table/field/row condition) + roles + domain separation | Fine-grained, data-driven authorization instead of code-embedded checks | Directly informs Phase 17 |
| **Knowledge** | Article authoring, versioning, publishing, search | Self-service deflection depends on findable, trustworthy content | Confirms Knowledge is a platform capability consumed by every customer-facing product, not an ITSM-only feature |
| **Service Catalog** | Structured request items, variables, fulfillment workflow | One request/fulfillment model reused by IT, HR, Facilities, etc. | Catalog is platform, not ITSM-specific — see Phase 48 |
| **Employee Center** | Unified portal shell for all employee-facing services | One front door regardless of which backend product fulfills the request | Confirms one portal shell pattern across departments (echoes our earlier master guide's Employee Center) |
| **Mobile** | Native mobile apps for agents and employees | Field/on-call/away-from-desk workflows need push-capable clients | Push notification + offline-tolerant UX is a platform requirement, not an ITSM afterthought |
| **Reporting / Performance Analytics** | Ad hoc reports + time-series KPI tracking with baselines/targets | Operational reporting (today's numbers) vs. trend analytics (over time) are different capabilities | We should separate "reporting" (Phase-level dashboards) from "analytics" (trend/SLO tracking) architecturally |
| **Notifications** | Multi-channel (email, push, SMS) template-driven delivery | Every workflow needs to reach a human reliably | Confirms Notification as its own platform capability — aligns with our existing `09-notification-oncall-escalation-engine.md` |
| **Events** | Platform-level event bus for triggering flows/notifications on record changes | Decouples "something happened" from "who reacts to it" | Maps to our Phase 14 event-driven architecture |
| **Approvals** | Reusable approval construct (single/group/multi-stage) used across Catalog, Change, HR cases | One approval primitive avoids reinventing "wait for a yes/no from a human" per app | Approval should be a shared workflow activity type, not duplicated per domain (Phase 12) |

---

## 2. IT workflows

| Product | What it solves | Core entities | Depends on |
|---|---|---|---|
| **ITSM** | Incident, Problem, Change, Request fulfillment per ITIL practices | Incident, Problem, Change, `task` base, SLA | CMDB (impact), Knowledge, Catalog |
| **ITOM** | IT operations: event correlation, health, cloud observability | Alert, Event, Business Service map | CMDB, Discovery |
| **ITAM (IT Asset Management)**, split into **Hardware Asset Management** and **Software Asset Management** | Lifecycle, cost, license compliance for hardware/software assets | Asset, Model, License, Contract | CMDB (asset↔CI link), Procurement |
| **CMDB** | System of record for configuration items and their relationships | CI, CI Class, Relationship | Discovery/Service Mapping for population |
| **Discovery** | Automated CI discovery from the network/cloud | Discovered device/service records feeding CMDB | Requires on-prem/cloud collector agents (MID Server analog) |
| **Service Mapping** | Builds application service maps (which CIs support which business service) | Service, Application Service, dependency graph | CMDB, Discovery |
| **Event Management / AIOps** | Correlates monitoring events into actionable alerts, reduces noise | Event, Alert, Correlation rule | ITOM, CMDB |
| **Cloud Observability** | Metrics/logs/traces ingestion for cloud-native workloads | Trace, Metric, Log correlated to CI/service | ITOM, CMDB |
| **Service Operations Workspace** | Unified agent UX across ITSM/ITOM work | N/A (UX layer) | ITSM, ITOM |
| **DevOps (DevOps Change Velocity)** | Links CI/CD pipeline events to Change records | Change, Pipeline execution, deployment | ITSM Change, source integration |
| **Strategic Portfolio Management / Project Portfolio Management** | Portfolio, program, project, and demand management | Portfolio, Program, Project, Demand | Often integrates with ITSM/DevOps for delivery traceability |

---

## 3. Customer workflows

| Product | What it solves | Core entities |
|---|---|---|
| **Customer Service Management (CSM)** | External customer case management, service issue resolution | Case, Account, Contact, Entitlement |
| **Field Service Management (FSM)** | Dispatch and manage field technicians against work orders | Work Order, Task, Technician, Parts |
| **Sales and CRM (Order/Sales workflows)** | Quote-to-order and account workflows connected to fulfillment | Opportunity, Order, Account | `REQUIRES_VALIDATION` — evolving CRM push per 2026 announcements |
| **Industry workflows (Telecom, Banking, Healthcare, etc.)** | Vertical-specific case/process templates on top of CSM/FSM | Industry-specific case subtypes | Built on CSM/FSM/Task, not separate engines |

---

## 4. Employee workflows

| Product | What it solves | Core entities |
|---|---|---|
| **HR Service Delivery (HRSD)** | Employee case management (HR, payroll, benefits inquiries) | HR Case, Employee, Lifecycle Event |
| **Employee Experience / Employee Center** | Unified employee portal across all department services | Portal navigation, unified request/case list |
| **Workplace Service Delivery** | Facilities/workplace requests (desk booking, maintenance) | Workplace Case, Location, Asset |
| **Legal Service Delivery** | Legal intake and matter management | Legal Case, Matter |

**Lesson repeated across every "X Service Delivery" product:** they are all
**Case** (a `task` subtype) + a domain-specific data extension + the same
Employee Center front door. This is the strongest evidence in the entire
landscape for a shared `Task`/`Case` base model (Phase 9).

---

## 5. Security, Risk & Compliance

| Product | What it solves | Core entities |
|---|---|---|
| **Security Operations (SecOps) / Security Incident Response** | Security incident triage and response, distinct lifecycle from ITSM Incident | Security Incident, IOC, Playbook |
| **Vulnerability Response** | Tracks and remediates vulnerabilities against CMDB assets | Vulnerable Item, CVE, CI |
| **Security Exposure Management / Security Posture Control** | Aggregate risk/exposure visibility across the estate | Exposure finding, control status |
| **Risk / Integrated Risk Management** | Enterprise risk register, controls, assessments | Risk, Control, Assessment |
| **Compliance** | Maps controls/policies to regulatory frameworks | Policy, Control, Compliance task |
| **Privacy** | Data privacy impact assessments, subject rights requests | Privacy assessment, DSR |
| **Third-Party Risk** | Vendor/supplier risk assessment workflows | Vendor, Risk assessment |

**Architectural note:** Security Incident is a **separate lifecycle** from
ITSM Incident even though both extend `task` — access control (SecOps data
is far more sensitive) and process (containment/forensics vs. service
restoration) diverge enough to warrant a distinct domain, not a shared one.
This is a concrete counterexample to "just reuse Incident for everything."

---

## 6. Enterprise / other product families

| Family | Products (non-exhaustive, current taxonomy evolves) |
|---|---|
| Finance & Supply Chain | Source-to-Pay, Accounts Payable Operations `REQUIRES_VALIDATION` |
| Procurement | Procurement workflows tied to Asset/Contract |
| Legal | Legal Service Delivery (also under Employee) |
| Workplace | Workplace Service Delivery (also under Employee) |
| Manufacturing, Healthcare, Public Sector, Banking/Financial Services, Retail, Technology, Telecommunications | Industry-specific vertical workflow packages layered on CSM/FSM/ITSM primitives |

`UNKNOWN`: exact current SKU boundaries within Finance/Supply Chain — these
have shifted release-to-release and require validation against the live
`servicenow.com/products` catalog at implementation time rather than being
treated as fixed here.

---

## 7. What we explicitly do NOT need to replicate

Per the Master AI Command's Rule 1–4 (don't clone every product, don't copy
proprietary implementation): the vertical industry workflow packages (§6),
Sales/CRM, and most of Finance & Supply Chain are **out of scope** for our
platform. We replicate the **architectural pattern** (Task/Case base +
shared platform capabilities + portal) demonstrated across IT, Employee, and
Security families, and build our own product scope (ITSM/ITAM first) on it —
per the MVP definition in `docs/roadmap/mvp.md`.

---

## References

- [ServiceNow Docs](https://www.servicenow.com/docs)
- [What is the ServiceNow AI Platform?](https://www.servicenow.com/platform/what-is-servicenow-ai-platform.html)
- [ServiceNow AI Platform](https://www.servicenow.com/platform.html)
- [ServiceNow Products](https://www.servicenow.com/products.html)
- Secondary (marked inline where used): industry analyst summaries of 2026 portfolio structure — flagged `REQUIRES_VALIDATION` against primary docs before being treated as a locked fact.

---

*Next: `docs/servicenow-research/product-capability-matrix.md` (Phase 2).*
