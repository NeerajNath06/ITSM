# Definition of Done

**Phase:** 54 · A feature is **not complete** until every applicable item
below is satisfied. "Mostly works" or "works on my machine" does not
qualify — this mirrors the exit-criteria discipline the original ENTERPRISE_ITSM
guide established for its waves, now applied per-feature.

## Checklist

```
[ ] Architecture reviewed          — matches docs/architecture/, docs/adr/;
                                      no anti-pattern from docs/rules/anti-patterns.md
[ ] Database migration created      — EF Core migration, RLS policy if tenant-scoped
                                      table, reviewed per docs/rules/database-rules.md
[ ] API documented                  — OpenAPI/.proto generated and current,
                                      no undocumented public endpoint
[ ] Authorization implemented       — ACL policy exists for every new
                                      table/operation/field touched
[ ] Audit implemented               — mutation captured by the shared audit
                                      interceptor (or explicitly, justifiably exempt)
[ ] Logging implemented             — structured, scoped with TenantId/
                                      CorrelationId/TraceId, no Restricted data logged
[ ] Tracing implemented             — OpenTelemetry spans cover the new code path
[ ] Metrics implemented             — relevant RED/domain metrics emitted for
                                      anything with a latency/error/volume expectation
[ ] Unit tests implemented          — Domain/Application logic covered
[ ] Integration tests implemented   — including a tenant-isolation check if a
                                      new tenant-scoped table was added
[ ] Security reviewed               — for anything touching auth/ACL/tenancy/
                                      encryption/external integration
[ ] Performance considered          — no N+1 queries, pagination present on
                                      any new list endpoint, query plan reviewed
                                      for high-volume tables
[ ] Documentation updated           — relevant docs/domains/*.md, docs/api/,
                                      or other Phase document reflects the change
[ ] ADR created if architecture changed — per docs/governance/architecture-governance.md
```

## Applicability

Not every item applies to every change — a pure documentation fix doesn't
need a migration; a config-only tenant setting doesn't need a new ACL rule.
The rule is: **every item is considered explicitly, not silently skipped**.
A PR description states which items apply and confirms each, or states why
an item doesn't apply to this change.

## Enforcement

- Unit/integration tests, migration presence, and OpenAPI currency are
  CI-enforced (build fails without them where applicable).
- Security review, architecture review, and documentation currency are
  reviewer-enforced (a human checks these in PR review) —
  `docs/rules/git-rules.md`'s review requirement exists specifically to
  make this checklist real rather than aspirational.

## Relationship to `docs/testing/testing-strategy.md`'s CI gates

This checklist is the **per-change** gate; the testing strategy's "minimum
CI gates by program stage" table is the **per-milestone** aggregate gate —
a milestone is only truly done when every change within it individually
satisfied this checklist, not merely when the aggregate test suite happens
to be green.

---

*Next: `docs/MASTER-ARCHITECTURE.md` (Phase 55) — the final summary
document.*
