# .NET Architecture

**Phase:** 31 · ASP.NET Core / .NET 10, applied to the modular monolith
(`Host.Api`) + satellite services topology from Phase 4/6.

## Layering per module (Clean Architecture, vertical slices within it)

```
Modules.Incident/
  Domain/            # entities, value objects, domain events — no EF, no HTTP, no external deps
  Application/        # commands, queries, handlers (MediatR), validators, interfaces
  Infrastructure/      # EF Core config, repositories, external service clients
  Api/                 # minimal API endpoint definitions, request/response DTOs
```

- **Clean Architecture** for the dependency direction (`Domain` has zero
  outward dependencies; `Application` depends only on `Domain`;
  `Infrastructure`/`Api` depend inward) — enforced by architecture tests
  (NetArchTest or equivalent), not just convention.
- **Vertical slices within Application**: one folder per use case
  (`ResolveIncident/ResolveIncidentCommand.cs`,
  `ResolveIncidentHandler.cs`, `ResolveIncidentValidator.cs`) rather than
  horizontal `Commands/`, `Handlers/`, `Validators/` folders spanning every
  use case — keeps a single feature's full logic colocated and easy to
  delete/modify without touching unrelated files.

## CQRS via MediatR

```csharp
public record ResolveIncidentCommand(Guid IncidentId, string ResolutionNotes) : IRequest<ResolveIncidentResult>;

public class ResolveIncidentHandler(
    IIncidentRepository repository,
    IUnitOfWork unitOfWork,
    IAuthorizationService authz)
    : IRequestHandler<ResolveIncidentCommand, ResolveIncidentResult>
{
    public async Task<ResolveIncidentResult> Handle(ResolveIncidentCommand cmd, CancellationToken ct)
    {
        var incident = await repository.GetAsync(cmd.IncidentId, ct)
            ?? throw new NotFoundException();
        await authz.AuthorizeAsync(incident, "Incident.Resolve"); // fail-closed if not authorized
        incident.Resolve(cmd.ResolutionNotes);                    // domain method enforces state machine
        await unitOfWork.SaveChangesAsync(ct);                    // audit interceptor fires here
        return new ResolveIncidentResult(incident.Id, incident.State);
    }
}
```

- **Commands** mutate, return the minimal result needed; **Queries** read,
  bypass the domain layer's write-side ceremony and query directly against
  read-optimized projections where useful (CQRS's read/write separation,
  not necessarily separate databases at our current scale).
- Validation via **FluentValidation**, run as a MediatR pipeline behavior —
  every command is validated before reaching its handler, uniformly, not
  per-handler boilerplate.

## EF Core, repositories, Unit of Work

- **Repository pattern used sparingly**: only where it genuinely
  encapsulates non-trivial query composition or is needed for testability
  boundaries — **not** a generic `IRepository<T>` applied uniformly to every
  entity (named anti-pattern, Phase 53). Simple CRUD entities are queried
  directly via `DbContext` from the Application layer's query handlers.
- **Unit of Work** = the `DbContext` itself (EF Core's `DbContext` already
  is a Unit of Work) — no redundant custom UoW abstraction wrapping it.
- **Domain events** raised by aggregate methods (`incident.Resolve()`
  appends an `IncidentResolvedDomainEvent` to an internal list), dispatched
  by a `SaveChanges` interceptor **after** the transaction commits (never
  before — a domain event must represent something that actually happened).
- **Integration events** are the versioned public shape published to the
  outbox (Phase 14) — mapped from domain events at the boundary, not the
  same CLR type reused directly (keeps internal refactors from breaking
  external contracts).

## Middleware and exception handling

- Global exception-handling middleware maps domain/application exceptions
  to `ProblemDetails` (per `docs/api/api-design-rules.md`): validation
  errors → 400, not-found → 404, authorization failures → 403, unhandled →
  500 with a correlation ID and no internal detail leaked.
- Middleware pipeline order (outer to inner): correlation ID → exception
  handling → authentication → tenant resolution → rate limiting (if not
  handled at APIM) → authorization → endpoint.

## Dependency injection

- Constructor injection throughout; no service locator pattern.
- Module registration via an `IModule` interface each module implements
  (`AddModule(IServiceCollection)`, `MapEndpoints(IEndpointRouteBuilder)`) —
  `Host.Api`'s `Program.cs` composes modules, it does not know their
  internals, keeping the composition root thin and the module boundary
  explicit in code, not just convention.

## Background workers

- `Host.Workers` hosts `BackgroundService` implementations for: outbox
  relay, SLA clock evaluation, and scheduled reindex triggers. Recurring/
  scheduled job execution specifically is implemented via **Hangfire** or
  **Quartz.NET** (per `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`)
  rather than hand-rolled `Timer`/polling loops, giving persistent job
  scheduling, retry, and a built-in dashboard for free. Workflow timers/
  scheduling specifically move to **Temporal** per
  `docs/adr/ADR-023-temporal-for-workflow-execution.md` — `Host.Workers`
  no longer owns that responsibility.
- Each worker is independently scalable (multiple replicas with
  competing-consumer semantics on its RabbitMQ queue) and independently
  observable (own health checks, own metrics).

## What we deliberately do NOT add

- No abstraction layer "in case we swap PostgreSQL later" — EF Core's
  provider model already gives reasonable portability; a speculative
  repository-of-repositories or ORM-agnostic layer is unjustified
  complexity (Master AI Command: don't add abstractions merely because
  they're popular).
- No custom mediator/CQRS framework — MediatR (or the .NET-native minimal
  equivalent, latest ecosystem-appropriate choice at implementation time)
  is used as-is.

## Nullability, async, cancellation

- Nullable reference types **enabled project-wide**, warnings as errors —
  no `#nullable disable` escape hatches in new code.
- `async`/`await` end-to-end; no `.Result`/`.Wait()` blocking calls.
- Every handler/repository method accepts and propagates a
  `CancellationToken` — a client disconnect or request timeout actually
  cancels the underlying database/HTTP work instead of running to
  completion wastefully.

---

*Next: `docs/rules/coding-rules.md` (Phase 32).*
