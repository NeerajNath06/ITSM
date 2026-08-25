using FluentValidation;

namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Runs every registered <see cref="IValidator{T}"/> for the request before
/// the handler executes — the FluentValidation-as-pipeline-behavior pattern
/// from docs/backend/dotnet-architecture.md, adapted to this project's
/// hand-rolled dispatcher instead of MediatR. Registered once, globally, as
/// an open generic by <see cref="ServiceCollectionExtensions.AddDispatcher"/>;
/// requests with no registered validator pass straight through.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(result => result.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
