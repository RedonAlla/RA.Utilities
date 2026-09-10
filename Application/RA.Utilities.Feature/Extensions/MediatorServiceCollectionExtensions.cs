using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Generated;

namespace RA.Utilities.Feature.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to add Mediator services.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Adds Mediator services to the specified <see cref="IServiceCollection"/> and applies
    /// the compile-time handler registrations produced by the
    /// <c>RA.Utilities.Feature.Generators</c> source generator.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        HandlerRegistrations.ApplyAll(services);
        return services;
    }
}
