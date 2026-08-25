# AI Coding Agent Rules

**Phase:** 52 · The process discipline an AI coding agent (or any engineer)
follows for any non-trivial change in this repository.

## The loop

```
Understand
   ↓
Inspect
   ↓
Plan
   ↓
Validate architecture
   ↓
Implement
   ↓
Test
   ↓
Review
   ↓
Document
```

**The AI must NOT immediately start coding.** A request like "add a field
to Incident" or "build the notification escalation timer" starts with the
steps below, not with an editor open.

## Step detail

1. **Inspect existing code.** Read the relevant module(s) before writing
   anything — do not assume structure from the docs alone; the docs
   describe intended architecture, the code is the current reality, and per
   `CLAUDE.md`'s closing rule, a conflict between them gets documented, not
   silently resolved by guessing.
2. **Identify affected modules.** Use `docs/architecture/bounded-contexts.md`
   to determine which context(s) own the data/logic in question, and which
   contexts merely read it.
3. **Check architecture documentation.** The relevant Phase document(s)
   under `docs/` — cited throughout `CLAUDE.md`'s rule table — before
   assuming a pattern.
4. **Check ADRs.** `docs/adr/` — has this exact question already been
   decided? Does the proposed change require a new or superseding ADR?
5. **Check database ownership.** Per `docs/database/database-architecture.md`
   and `docs/rules/database-rules.md` — which context owns this table, does
   this change need RLS/migration work, is this a shared-kernel table
   requiring a second reviewer.
6. **Check API contracts.** Per `docs/api/api-design-rules.md` — does this
   change break an existing public contract (requires versioning), and
   which of the three API surfaces (Table/Domain/BFF) does a new endpoint
   belong to.
7. **Check security.** Per `docs/security/security-architecture.md` and
   `docs/rules/security-rules.md` — does this change need a new ACL rule,
   does it touch `Confidential`/`Restricted` data, does it affect tenant
   isolation.
8. **Check audit.** Does this mutation belong on the "always audited" list
   (`docs/audit/audit-architecture.md`)? Most Task-family/CMDB/ACL/Workflow
   mutations do, by default, via the shared interceptor — verify it's not
   bypassed.
9. **Check observability.** Does a new deployable/endpoint need explicit
   OpenTelemetry instrumentation beyond what the shared middleware already
   provides (`docs/observability/observability.md`)?
10. **Plan.** Write out the concrete plan — which files change, in what
    order, what tests prove it — before editing. For a large change, this
    plan is worth surfacing to the user/reviewer before implementation
    starts.
11. **Implement.** Follow `docs/rules/coding-rules.md` and the layering
    rules in `docs/backend/dotnet-architecture.md` /
    `docs/frontend/react-architecture.md`.
12. **Test.** Per `docs/testing/testing-strategy.md` — the right layer(s)
    for this change, plus a regression test if it's a bug fix.
13. **Review.** Self-review against `docs/rules/anti-patterns.md`
    (Phase 53) and `docs/governance/definition-of-done.md` (Phase 54)
    before considering the change ready.
14. **Update documentation.** If the change alters an architectural
    decision, add/update an ADR. If it changes a domain's shape, update the
    relevant `docs/domains/*.md`. Documentation drift is treated as a bug.

## Why this order, not "just start coding"

Every step above exists because skipping it has caused a real class of
problem researched into this program: skipping step 2/3 duplicates a shared
entity (Rule 3 in `CLAUDE.md`); skipping step 5/12 ships an RLS-less table
that later leaks cross-tenant; skipping step 7/8 ships an unaudited,
ACL-less mutation path. The loop is the accumulated lesson, not ceremony
for its own sake.

## Scale the loop to the change

A one-line bug fix in an already-understood module does not need a written
plan document — but it still gets inspected, tested, and reviewed against
the anti-pattern list. A new bounded context, a new public API version, or
a proposed microservice extraction gets the full loop, explicitly, with the
plan surfaced for review before implementation.

---

*Next: `docs/rules/anti-patterns.md` (Phase 53).*
