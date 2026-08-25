# Domain: Asset Management (ITAM)

**Phase:** 40 · MVP scope (light — full ITAM lifecycle depth is a
later-wave expansion per the roadmap; MVP covers hardware asset tracking
linked to CI).

| | |
|---|---|
| **Purpose** | Track lifecycle, ownership, and cost of hardware/software assets. |
| **Business Capability** | Asset Lifecycle & Cost Management |
| **Actors** | Asset Manager, Procurement, Finance (cost reporting consumer) |

**Use cases:** receive a new asset (procurement record → Asset record);
assign to a user/location; link to a CI (if it's IT infrastructure);
track warranty/contract coverage; retire/dispose with disposal record.

**Entities/Aggregates:** `Asset`, `Model` (make/model catalog),
`License` (software), `Contract` (coverage/warranty), `Entitlement`.

**Commands:** `ReceiveAsset`, `AssignAsset`, `LinkAssetToCI`, `RetireAsset`,
`RecordDisposal`.

**Queries:** `GetAssetById`, `ListAssetsByOwner`, `GetLicenseCompliance`.

**Events:** `AssetReceived`, `AssetAssigned`, `AssetRetired`.

**APIs:** `POST /api/v1/assets`, `POST /api/v1/assets/{id}/link-ci`.

**Database tables:** `asset`, `model`, `license`, `contract`,
`entitlement` — **never merged with `cmdb_ci`** (relates-to link only, per
`shared-enterprise-data-model.md`).

**Security:** Cost/contract fields are `Confidential`-classified
(commercially sensitive) — restricted to Asset Manager/Finance roles even
though basic asset assignment (who has this laptop) is broadly visible.

**Audit:** Assignment changes, disposal records (compliance-relevant for
regulated hardware disposal).

**Workflow:** approval for high-cost asset requests reuses the shared
`Approval` activity, triggered from Request Management when a catalog item
represents physical hardware.

**Notifications:** warranty-expiry reminders, license-compliance threshold
alerts.

**Dependencies:** CMDB (link, not ownership), Request Management (hardware
request fulfillment creates/assigns an Asset), Contract data.

**Failure scenarios:** an Asset is retired but its linked CI remains
`Installed` (or vice versa) — the two lifecycles are independent by design
(an Asset can be disposed while the CI record persists for historical
relationship integrity, per CMDB's never-hard-delete rule), so this is
expected, not a bug, and dashboards should show both states rather than
assuming they're always in lockstep.

**KPIs:** License compliance %, unused-license spend, cost per asset,
asset utilization rate.

---

*Next (out-of-MVP-scope domains, documented for pattern completeness):
`docs/domains/case-management.md`.*
