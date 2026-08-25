# Domain: Contract

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.
Referenced (not owned) by Asset Management in MVP for basic warranty
tracking; full Contract domain is post-MVP.

| | |
|---|---|
| **Purpose** | Represent commercial agreements (support contracts, licenses, warranties) and the Entitlements they grant. |
| **Business Capability** | Contract & Entitlement Management |
| **Actors** | Procurement, Vendor Manager, Finance |

**Entities/Aggregates:** `Contract`, `Entitlement` (shared with Customer
Management domain — one definition, two consumers).

**Commands/Queries/Events:** `CreateContract`, `GrantEntitlement`,
`RenewContract`; `GetContractById`, `ListExpiringContracts`;
`ContractExpiringSoon`, `EntitlementGranted`.

**APIs:** `/api/v1/contracts/*`.

**Database tables:** `contract`, `entitlement`.

**Security:** Commercial terms are `Confidential`-classified.

**Audit:** Renewal/expiry, entitlement grants (billing-adjacent).

**Workflow:** renewal reminder + approval as a Workflow Definition
triggered ahead of `expires_at`.

**Dependencies:** Vendor (supplies), Asset (covers, relates-to — MVP's
light Asset↔Contract link), Customer Management (entitlement consumer,
CSM context).

**Failure scenarios:** a Contract lapses without a renewal reminder firing
— mitigated by treating expiry-reminder scheduling the same as any other
time-based Workflow trigger (Phase 12), not a bespoke cron job outside the
platform's standard scheduling mechanism.

**KPIs:** Contract renewal rate, expiring-contract lead time, entitlement
utilization.

**Build trigger:** full Contract domain (beyond MVP's basic Asset↔Contract
link) ships alongside ITAM expansion or CSM, whichever lands first
post-MVP.

---

*This completes the Phase 40 domain documentation set (8 MVP-scope + 10
deferred-scope domains). Next: `docs/adr/` (Phase 41).*
