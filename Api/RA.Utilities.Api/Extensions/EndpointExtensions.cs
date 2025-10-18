using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RA.Utilities.Api.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for registering API endpoints.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Scans the specified assembly for types that implement the <see cref="IEndpoint"/> interface
    /// and registers them as transient services in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// <strong>
    /// This registers automatically all endpoints that implement the <see cref="IEndpoint"/> interface
    /// to use Features flag needs to rethink.
    /// </strong>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembly">The assembly to scan for endpoints.</param>
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
    /// Discovers and maps all registered <see cref="IEndpoint"/> services to the application's routes.
    /// </summary>
    /// <remarks>
    /// <strong>
    /// This registers automatically all endpoints that implement the <see cref="IEndpoint"/> interface
    /// to use Features flag needs to rethink.
    /// </strong>
    /// </remarks>
    /// <param name="app">The <see cref="WebApplication"/> to map the endpoints to.</param>
    /// <param name="routeGroupBuilder">An optional <see cref="RouteGroupBuilder"/> to group the endpoints under a common prefix.</param>
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
