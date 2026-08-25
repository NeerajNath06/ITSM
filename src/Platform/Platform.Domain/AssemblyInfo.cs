// Platform.Domain — the platform kernel's Domain layer (Identity, Tenancy,
// ACL, Audit, Metadata entities and value objects; see
// docs/architecture/bounded-contexts.md "Load-bearing contexts").
//
// Rule (enforced by tests/Architecture.Tests): this project may reference
// only BuildingBlocks. No EF Core, no ASP.NET Core, no infrastructure
// concern of any kind. Populated starting Wave 1 / Phase 2
// (Identity and Organization) per docs/roadmap/implementation-roadmap.md.
