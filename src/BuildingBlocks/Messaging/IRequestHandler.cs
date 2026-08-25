namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Handles exactly one <see cref="IRequest{TResponse}"/> type. Application
/// layers implement this per use case (one vertical slice per handler, per
/// docs/backend/dotnet-architecture.md) — never a generic handler covering
/// multiple request types.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
