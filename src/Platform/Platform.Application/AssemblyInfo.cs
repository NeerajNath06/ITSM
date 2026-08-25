// Platform.Application — the platform kernel's Application layer:
// commands/queries/handlers for Identity, Tenancy, ACL, Audit, Metadata
// (see docs/backend/dotnet-architecture.md, CQRS via MediatR).
//
// Rule (enforced by tests/Architecture.Tests): may reference Platform.Domain
// and BuildingBlocks only — never Platform.Infrastructure, never EF Core,
// never ASP.NET Core. Populated starting Wave 1 / Phase 2.
