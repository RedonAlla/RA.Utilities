using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;

namespace RA.Utilities.Feature.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to add Mediator services.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Adds Mediator services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        return services;
    }
}
