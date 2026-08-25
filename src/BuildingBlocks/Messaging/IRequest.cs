namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Marks a command or query that returns <typeparamref name="TResponse"/>,
/// dispatched via <see cref="IDispatcher"/> to the single <see cref="IRequestHandler{TRequest,TResponse}"/>
/// registered for it. Deliberately shaped to match the handler signature
/// already documented in docs/backend/dotnet-architecture.md
/// (<c>IRequestHandler&lt;ResolveIncidentCommand, ResolveIncidentResult&gt;</c>)
/// so every module's command/query code reads the same regardless of which
/// dispatcher implementation resolves it.
/// </summary>
public interface IRequest<out TResponse>;
