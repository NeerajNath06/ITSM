namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Resolves and invokes the single <see cref="IRequestHandler{TRequest,TResponse}"/>
/// registered for a given request, running its <see cref="IPipelineBehavior{TRequest,TResponse}"/>
/// chain first. The in-process CQRS entry point every module's Api/endpoint
/// layer calls instead of resolving handlers directly.
/// </summary>
public interface IDispatcher
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
