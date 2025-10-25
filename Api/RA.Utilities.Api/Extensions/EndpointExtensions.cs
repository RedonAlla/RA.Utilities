using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RA.Utilities.Api.Abstractions;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for discovering and registering API endpoints that implement the <see cref="IEndpoint"/> interface.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Scans the specified assembly for types that implement the <see cref="IEndpoint"/> interface
    /// and registers them as transient services in the <see cref="IServiceCollection"/>. This method is the first step
    /// in the endpoint registration process, making the endpoint definitions available for dependency injection.
    /// </summary>
    /// <remarks>
    /// <strong>
    /// This registers automatically all endpoints that implement the <see cref="IEndpoint"/> interface
    /// to use Features flag needs to rethink.
    /// </strong>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembly">The assembly to scan for endpoint definitions.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        // TODO Refactor the `IEndpoint` registration to allow for feature flagging.
        ServiceDescriptor[] serviceDescriptors = [.. assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))];

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    /// Discovers and maps all registered <see cref="IEndpoint"/> services to the application's routes. This method
    /// retrieves all services registered via <see cref="AddEndpoints(IServiceCollection, Assembly)"/> and executes
    /// their <see cref="IEndpoint.MapEndpoint(IEndpointRouteBuilder)"/> method.
    /// </summary>
    /// <remarks>
    /// <strong>
    /// This registers automatically all endpoints that implement the <see cref="IEndpoint"/> interface
    /// to use Features flag needs to rethink.
    /// </strong>
    /// </remarks>
    /// <param name="app">The <see cref="WebApplication"/> to map the endpoints to.</param>
    /// <param name="routeGroupBuilder">An optional <see cref="RouteGroupBuilder"/> to group the discovered endpoints under a common prefix and/or with shared configuration.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> to allow for fluent chaining.</returns>
    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}
