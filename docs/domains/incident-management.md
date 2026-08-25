# Domain: Incident Management

**Phase:** 40 · MVP scope (per `docs/roadmap/mvp.md`).

| | |
|---|---|
| **Purpose** | Restore normal service operation as fast as possible after an unplanned disruption, minimizing business impact. |
| **Business Capability** | IT Service Restoration |
| **Actors** | Requester/Caller (reports the issue), Agent (triages/resolves), Assignment Group, Major Incident Commander, Approver (for related change if needed) |

## Use cases

1. Employee reports a service disruption via Employee Portal or email.
2. Monitoring tool auto-creates an Incident from a correlated alert.
3. Agent triages, classifies, assigns.
4. Agent diagnoses, applies a fix, resolves with resolution notes.
5. Major Incident Commander declares a major incident, initiates the war
   room, sends stakeholder communications.
6. Requester receives a CSAT survey post-resolution.

## Entities / Value Objects / Aggregates

- **Aggregate root**: `Incident` (extends `TaskBase`, Phase 9).
- **Value objects**: `Priority` (derived from Impact × Urgency matrix),
  `OutageWindow` (start/end).
- **Composed, not always present**: `MajorIncidentDetails` (war-room link,
  communication cadence, commander).
- **Related, not owned**: `ConfigurationItem` (affected CI, reference
  only), `KnowledgeArticle` (suggested, reference only).

## Commands

`CreateIncident`, `ClassifyIncident`, `AssignIncident`, `ResolveIncident`,
`ReopenIncident`, `DeclareMajorIncident`, `CloseIncident`.

## Queries

`GetIncidentById`, `ListMyIncidents`, `ListIncidentsByGroup`,
`SearchSimilarIncidents` (backed by Search platform, Phase 21).

## Events

`IncidentCreated`, `IncidentAssigned`, `IncidentResolved`,
`IncidentReopened`, `IncidentDeclaredMajor`, `IncidentClosed`.

## APIs

- Domain API: `POST /api/v1/incidents`, `POST /api/v1/incidents/{id}/resolve`,
  `POST /api/v1/incidents/{id}/declare-major`.
- Table API: `/api/v1/table/incident` for generic list/filter needs (Admin
  Studio, reporting).

## Database tables

`task` (base), `incident`, `major_incident_details`, `journal_entry`,
`attachment_metadata`, `task_relationship` (for "caused by" / "related to").

## Security

Standard tenant ACL + RLS. Caller contact info is `Confidential`-classified
(masked for viewers without the caller-visibility grant). Major incident
communication content may be broadcast to a wider audience than the base
ACL — handled via an explicit "communication" sub-permission, not by
loosening the record's base ACL.

## Audit

Every state/assignment/priority change, every major-incident declaration —
per `docs/audit/audit-architecture.md`.

## Workflow

Assignment routing, major-incident notification fan-out, and post-resolve
survey triggering all run as Workflow Definitions (Phase 12), not hardcoded
handler logic — tenants can adjust routing rules via Admin Studio without a
code change.

## Notifications

Assignment notification, major-incident escalation (via
`09-notification-oncall-escalation-engine.md`), resolution notification to
requester, CSAT survey send.

## Dependencies

CMDB (affected CI, impact context), Knowledge (suggested articles), SLA
Engine (clock), Notification (all alerts), Workflow (routing/approval where
applicable).

## Failure scenarios

- Monitoring tool floods duplicate alerts → Event Management/correlation
  (Phase 2 finding: ITOM's job) must de-duplicate before Incident creation,
  or Incident creation itself must be idempotent on a correlation key to
  avoid a storm of duplicate tickets.
- CMDB CI reference is stale/incorrect → impact analysis misleads
  responders; mitigated by CMDB data-quality rules (Phase 11), not by
  Incident-side validation alone.
- Major incident declared but Notification service degraded → the
  end-of-sequence fallback and self-monitoring platform alert
  (`notification-service.md`) are the safety net.

## KPIs

MTTR, First Contact Resolution rate, reopen rate, SLA attainment %, major
incident count/duration trend.

---

*Next: `docs/domains/problem-management.md`.*
