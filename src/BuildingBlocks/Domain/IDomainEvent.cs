namespace ITSM.BuildingBlocks.Domain;

/// <summary>
/// Marker for a fact that already happened inside a bounded context.
/// See docs/integration/event-driven-architecture.md for the domain
/// event vs. integration event distinction.
/// </summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
