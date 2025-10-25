using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Builders;

namespace RA.Utilities.Feature.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to add mediator features.
/// </summary>
public static class FeatureBuilderExtensions
{
    /// <summary>
    /// Adds a request-response feature to the service collection.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <typeparam name="THandler">The type of the handler for the request.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>A <see cref="FeatureBuilder{TRequest, TResponse}"/> to configure the feature.</returns>
    public static FeatureBuilder<TRequest, TResponse> AddFeature<TRequest, TResponse, THandler>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
        return new FeatureBuilder<TRequest, TResponse>(services);
    }

    /// <summary>
    /// Adds a request-only feature to the service collection.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="THandler">The type of the handler for the request.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>A <see cref="FeatureBuilder{TRequest}"/> to configure the feature.</returns>
    public static FeatureBuilder<TRequest> AddFeature<TRequest, THandler>(
        this IServiceCollection services)
        where TRequest : IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        services.AddScoped<IRequestHandler<TRequest>, THandler>();
        return new FeatureBuilder<TRequest>(services);
    }

    /// <summary>
    /// Adds a notification feature to the service collection.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>A <see cref="NotificationFeatureBuilder{TNotification}"/> to configure the notification.</returns>
    public static NotificationFeatureBuilder<TNotification> AddNotification<TNotification>(
        this IServiceCollection services)
        where TNotification : INotification
    {
        return new NotificationFeatureBuilder<TNotification>(services);
    }
}
