// Platform.Infrastructure — EF Core DbContext(s), the tenant-aware
// connection interceptor, ACL policy evaluators, the audit SaveChanges
// interceptor, and external service clients (Keycloak, OpenBao) for the
// platform kernel.
//
// Rule (enforced by tests/Architecture.Tests): only Host.Api and
// Host.Workers may reference this project — no Module may reference
// another module's Infrastructure (docs/backend/dotnet-architecture.md).
// Populated starting Wave 1 / Phase 2.
