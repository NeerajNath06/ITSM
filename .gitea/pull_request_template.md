<!--
Per docs/rules/git-rules.md and docs/governance/definition-of-done.md.
-->

## What / Why

<!-- What changed, and why — the diff already shows what, explain the reasoning. -->

## Definition of Done (docs/governance/definition-of-done.md)

Check each applicable item, or state why it doesn't apply to this change:

- [ ] Architecture reviewed — matches `docs/architecture/`, `docs/adr/`; no anti-pattern from `docs/rules/anti-patterns.md`
- [ ] Database migration created (if schema changed) — RLS policy included if a tenant-scoped table was added
- [ ] API documented (if a public endpoint changed) — OpenAPI/`.proto` current
- [ ] Authorization implemented (if new table/operation/field)
- [ ] Audit implemented (if a mutation was added)
- [ ] Logging / tracing / metrics implemented
- [ ] Unit tests implemented
- [ ] Integration tests implemented (incl. tenant isolation, if a new tenant-scoped table)
- [ ] Security reviewed (if touching auth/ACL/tenancy/encryption/external integration)
- [ ] Performance considered (pagination, no N+1)
- [ ] Documentation updated (`docs/domains/*.md`, `docs/api/`, etc.)
- [ ] ADR created if an architectural decision changed

## Architecture reference

<!-- Cite the relevant docs/ section(s) this change follows. -->
