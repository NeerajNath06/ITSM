using Microsoft.Extensions.DependencyInjection;

namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Default <see cref="IDispatcher"/>. The request's concrete runtime type is
/// only known as <c>IRequest&lt;TResponse&gt;</c> at the call site, so the
/// closed handler/behavior types are built via reflection and invoked
/// through <see langword="dynamic"/> — the same shape MediatR itself
/// resolves internally, kept deliberately simple here since this dispatcher
/// only needs to support the one call pattern (docs/backend/dotnet-architecture.md's
/// <c>IRequestHandler&lt;TCommand,TResult&gt;</c> shape), not a general
/// notification/streaming pipeline.
/// </summary>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        var behaviors = serviceProvider.GetServices(behaviorType).Cast<dynamic>().Reverse();

        RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle((dynamic)request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = pipeline;
            pipeline = () => behavior.Handle((dynamic)request, next, cancellationToken);
        }

        return pipeline();
    }
}
