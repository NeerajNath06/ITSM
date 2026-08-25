# Architecture & Design Document Set — Index

This folder expands the cross-cutting sections of
[`docs/ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md`](../ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md)
(the master build bible) into standalone, execution-ready specifications. The
master guide stays the single source of truth for **wave sequencing and exit
criteria**; these documents are the **detailed design** an engineer opens
while implementing a specific concern inside any wave.

Read order for a new engineer: master guide → `01` → `02` → the rest in any
order relevant to the module you're building.

## Document set

| # | File | Covers | Status |
|---|---|---|---|
| 01 | [`01-security-architecture.md`](01-security-architecture.md) | Threat model, AuthN/AuthZ, ACL engine, RLS, encryption, secrets, audit, compliance, secure SDLC | ✅ Done |
| 02 | `02-tenancy-and-data-isolation.md` | Hybrid tenancy model, tenant context propagation, connection routing, isolation test suite | ⏳ Next |
| 03 | `03-domain-data-model.md` | Task/CMDB/CSDM aggregates, state machines, entity relationship design | ⏳ Planned |
| 04 | `04-api-design.md` | Table API, Domain API, BFF, versioning, pagination/query language, OpenAPI contracts | ⏳ Planned |
| 05 | `05-workflow-engine.md` | Workflow definition schema, activity types, SLA engine, approval/CAB runtime | ⏳ Planned |
| 06 | `06-frontend-architecture.md` | Metadata-driven UI engine, design system, Employee Center / Agent Workspace / Admin Studio | ⏳ Planned |
| 07 | `07-devops-observability.md` | CI/CD, environments, HA/DR, OpenTelemetry, SLOs, runbooks | ⏳ Planned |
| 08 | `08-testing-qa-strategy.md` | Isolation/ACL/contract/load/chaos/a11y test design, CI gates per wave | ⏳ Planned |
| 09 | [`09-notification-oncall-escalation-engine.md`](09-notification-oncall-escalation-engine.md) | Groups/Shifts/Rotations/Escalation timelines, multi-channel delivery, closed-loop integrations (xMatters-aligned) | ✅ Done |

## How to use these documents

1. Each document is **normative** for its concern — code review should cite a
   section number when rejecting a PR that violates it.
2. Each document ends with a **checklist** that maps back to the wave(s) in
   the master guide where it must be satisfied.
3. Changing a decision marked **Locked** requires an ADR under `docs/adr/`,
   same rule as the master guide.
4. These are design specs, not code. No Wave 0 scaffolding happens from
   these alone — that remains a separate, explicit execution step.

---

*This index is updated as each document is delivered.*
