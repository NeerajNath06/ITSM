# Data Governance

**Phase:** 27

## Data ownership

Every table has exactly one owning bounded context (per
`docs/architecture/bounded-contexts.md`); data governance policy is set and
enforced by that context, never bolted on generically across the whole
database. A cross-cutting governance *policy* (retention default,
classification taxonomy) is centrally defined; its *enforcement* happens
inside each owning context's data lifecycle.

## Data classification

| Classification | Examples | Handling |
|---|---|---|
| `Public` | Catalog item names, public knowledge articles | No restriction |
| `Internal` | Ticket short description, CI names | Tenant-scoped only |
| `Confidential` | Caller contact info, notification device numbers, contract cost data | Field-level ACL; masked in exports by default; excluded from AI grounding unless explicitly permitted |
| `Restricted` | Credentials/secrets accidentally captured in free-text fields, health/compensation data (future HR scope) | Never logged, never sent to AI providers; redaction scanning where feasible; strictest ACL tier |

Classification is a **metadata attribute on each field definition**
(consistent with the metadata-driven platform principle), read by: export
tooling (to mask/exclude), AI grounding (to filter eligible context), and
the ACL policy engine (to apply field-level restriction).

## PII and sensitive data

- PII fields are tagged `Confidential` or `Restricted` per the table above
  at the point they're defined — not retrofitted after a compliance
  incident.
- Data-subject rights (GDPR/CCPA-class): a documented export and delete
  path exists per tenant before that tenant may hold EU/CA personal data in
  production. Delete requests are implemented as governed erasure
  (overwrite/anonymize fields, preserve referential integrity of the
  historical record where legally permissible — e.g. an Incident's
  resolution history stays intact but the caller's PII is scrubbed) rather
  than a raw row delete that would break FK-referenced history.

## Retention, archival, deletion

| Data type | Default retention | Mechanism |
|---|---|---|
| Operational records (Task family) | Tenant-configurable, default 7 years (ITSM audit norm) | Partition-based archival to cold storage, then deletion |
| Audit log | Immutable, retention per compliance tier (never shorter than the operational record it covers) | Partition drop past retention (Phase 19) |
| Attachments | Matches parent record's retention | Blob lifecycle policy (Phase 23) |
| Notification delivery history | Shorter default (90 days) — operational, not a system of record | Scheduled purge job |

## Legal hold

- A `legal_hold` flag on a Task/Case (and cascading to its attachments)
  suspends normal retention/deletion processing for that record regardless
  of age — checked by every retention/deletion job before acting, never
  bypassable by a scheduled job "running anyway."

## Encryption and access reviews

- Encryption requirements are set here as policy, implemented per
  `docs/security/security-architecture.md`.
- **Access reviews**: quarterly review of who holds `security_admin`,
  `tenant_admin`, and `cmdb_admin` roles, plus who has accessed
  `Restricted`-classified data in the review period (from the audit log) —
  documented as a governance process, tracked to completion, not merely
  "available if someone asks."

## Data lineage

- For the MVP, lineage is satisfied by: (a) the audit log (who changed
  what, when), and (b) the event stream's causation-ID chain
  (`event-driven-architecture.md`), which together reconstruct how a piece
  of data reached its current state. A dedicated lineage/catalog tool is
  **not** provisioned until reporting/analytics scope (post-MVP) creates
  real demand for cross-system lineage visualization — named here as a
  deferred, not forgotten, capability.

---

*Next: `docs/platform/configuration-management.md` (Phase 28).*
