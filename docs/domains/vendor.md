# Domain: Vendor

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.

| | |
|---|---|
| **Purpose** | Represent external suppliers/vendors and their relationship to Contracts and Assets. |
| **Business Capability** | Vendor Master Data |
| **Actors** | Procurement, Vendor Manager |

**Entities/Aggregates:** `Vendor`, `VendorContact`.

**Commands/Queries/Events:** `RegisterVendor`, `UpdateVendorStatus`;
`GetVendorById`; `VendorRegistered`.

**APIs:** `/api/v1/vendors/*`.

**Database tables:** `vendor`, `vendor_contact`.

**Security:** Standard tenant ACL/RLS; commercial terms visible only to
Procurement/Finance roles (field-level, not a separate isolation domain —
lower sensitivity than Employee/Security).

**Audit:** Status changes (e.g. vendor deactivation affecting active
contracts).

**Workflow:** vendor onboarding approval reuses shared `Approval` activity.

**Dependencies:** Contract (a Vendor supplies Contracts, relates-to, not
owns), Asset Management (assets may reference a supplying Vendor).

**Failure scenarios:** deactivating a Vendor while active Contracts
reference it — should warn/block similarly to CMDB's rule against retiring
a CI with active dependent relationships (same referential-integrity
discipline, applied here).

**KPIs:** Vendor performance/SLA compliance (if tracked), active vendor
count.

**Build trigger:** only alongside ITAM's contract-management expansion,
post-MVP.

---

*Next: `docs/domains/contract.md`.*
