# CMDB Data Quality Rules

**Phase:** 11 (companion to `cmdb-architecture.md`). CMDB is only as useful
as it is trustworthy — a CMDB agents stop believing gets bypassed, which
defeats the entire impact-analysis investment. These rules keep it credible.

## Quality dimensions tracked

| Dimension | Definition | How measured |
|---|---|---|
| **Completeness** | Required attributes for a CI's class are populated | % of CIs per class missing a required attribute (from `cmdb_ci_class_schema`) |
| **Correctness** | Attribute values match reality | Sampled reconciliation against Discovery re-scan diffs |
| **Staleness** | Time since last confirmed (Discovery re-scan or manual review) | `now() - last_discovered_at`, alerted past a per-class threshold |
| **Duplication** | Two CI rows represent the same real-world asset | Identification-rule collision detection (Phase 11 main doc) run as a background job |
| **Orphan relationships** | A `cmdb_relationship` row pointing to a retired/missing CI | Referential + status check, run nightly |
| **Ownership coverage** | % of CIs with a non-null `owner_group_id` | Direct query |

## Rules

1. **Every CI class must declare required attributes** in
   `cmdb_ci_class_schema` before it can be used — a class with zero required
   attributes is a modeling smell, not a shortcut.
2. **Discovery-sourced CIs are re-validated on every scan cycle**; a CI not
   re-confirmed within `N` cycles (class-configurable, default 30 days for
   Prod) is flagged `Stale`, surfaced on a data-quality dashboard, not
   silently trusted forever.
3. **Manual CIs are reviewed on a scheduled cadence** (default quarterly)
   since nothing re-confirms them automatically — ownership group is
   notified to attest.
4. **Duplicate detection runs as a scheduled background job** (`Host.Workers`),
   using the class's identification rule; matches above a confidence
   threshold are queued for `cmdb_admin` review, never auto-merged silently
   (merging is destructive to relationship history and must be an audited,
   reversible action).
5. **Orphan relationships are auto-flagged, not auto-deleted** — a
   relationship pointing to a retired CI might indicate the retirement was
   wrong, not the relationship.
6. **A CI cannot be marked `Retired` while it has active `depends on`
   inbound relationships from a non-retired CI** without an explicit
   override + reason (prevents silently orphaning a dependency graph that
   Incident/Change impact analysis relies on).
7. **Quality score is per-tenant, per-class, and trended over time** — a
   snapshot number without a trend hides whether data quality is improving
   or decaying.

## Why this is a named, standalone document

Data quality rules are **operational and evolve independently** of the CI
schema itself (Phase 11 main doc) — a schema change is rare; a quality
threshold tuning happens continuously as real discovery data arrives. Mixing
the two documents would force every quality-threshold tweak to look like an
architecture change requiring the same review weight as a schema migration.

---

*Next: `docs/workflow/workflow-engine-architecture.md` (Phase 12).*
