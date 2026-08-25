# Test Projects

Per `docs/testing/testing-strategy.md`. Populated as each layer has real
code to test — empty folders are placeholders, not yet buildable projects,
to avoid empty test projects cluttering the solution before there's
anything meaningful to assert on.

| Folder | Lands when | Tooling |
|---|---|---|
| `Architecture.Tests/` | Wave 0 (exists now) | xUnit + NetArchTest.Rules |
| `Unit/` | Wave 1 (Phase 2 — Identity/Tenancy/ACL logic) | xUnit + FluentAssertions |
| `Integration/` | Wave 1 (Phase 2 — RLS/ACL against a real PostgreSQL instance) | xUnit + Testcontainers |
| `Isolation/` | Wave 1 (Phase 2) — **highest-priority suite**, gates every wave after it | xUnit + Testcontainers |
| `Contract/` | Wave 3+ (Phase 5, once a public API surface exists) | OpenAPI diff gate, Pact-style |
| `E2E/` | Wave 3+ (Phase 5, once a golden path exists to test) | Playwright |
