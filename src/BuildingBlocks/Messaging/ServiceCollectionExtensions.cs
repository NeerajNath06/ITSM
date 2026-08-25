using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ITSM.BuildingBlocks.Messaging;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IDispatcher"/>, the global <see cref="ValidationBehavior{TRequest,TResponse}"/>,
    /// and every <see cref="IRequestHandler{TRequest,TResponse}"/>, <see cref="IPipelineBehavior{TRequest,TResponse}"/>,
    /// and FluentValidation <see cref="IValidator{T}"/> found in the given
    /// assemblies. Called once per module from that module's <c>AddModule</c>.
    /// </summary>
    public static IServiceCollection AddDispatcher(this IServiceCollection services, params Assembly[] handlerAssemblies)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        var candidateTypes = handlerAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsAbstract: false, IsInterface: false });

        foreach (var type in candidateTypes)
        {
            foreach (var closedInterface in type.GetInterfaces().Where(IsRegistrableInterface))
            {
                services.AddScoped(closedInterface, type);
            }
        }

        return services;

        static bool IsRegistrableInterface(Type i) =>
            i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
             i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) ||
             i.GetGenericTypeDefinition() == typeof(IValidator<>));
    }
}
