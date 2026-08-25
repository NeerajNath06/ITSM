namespace ITSM.BuildingBlocks.Domain;

/// <summary>
/// Base for every domain entity. Domain events are appended by entity
/// methods that enforce invariants (e.g. incident.Resolve()) and are
/// dispatched by the SaveChanges interceptor after commit, per
/// docs/backend/dotnet-architecture.md and docs/integration/event-driven-architecture.md.
/// This type must never reference EF Core, ASP.NET Core, or any
/// infrastructure concern — enforced by Architecture.Tests.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected init; } = Guid.NewGuid();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
