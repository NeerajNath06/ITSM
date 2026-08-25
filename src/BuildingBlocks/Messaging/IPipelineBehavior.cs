namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Continuation of a pipeline behavior chain — either the next behavior, or
/// the request handler itself once all behaviors have run.
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Cross-cutting logic that wraps every request of the matching type
/// (validation, logging, etc.) — FluentValidation plugs in as a behavior
/// here per docs/backend/dotnet-architecture.md. Behaviors run in
/// registration order, outermost first; the last-registered behavior sits
/// closest to the handler.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
