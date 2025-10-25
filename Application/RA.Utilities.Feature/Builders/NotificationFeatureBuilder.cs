using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;

namespace RA.Utilities.Feature.Builders;

/// <summary>
/// A builder for configuring notification features.
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public class NotificationFeatureBuilder<TNotification> : IFeatureBuilder where TNotification : INotification
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationFeatureBuilder{TNotification}"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public NotificationFeatureBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Adds a notification handler to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the notification handler.</typeparam>
    /// <returns>The current <see cref="NotificationFeatureBuilder{TNotification}"/> instance.</returns>
    public NotificationFeatureBuilder<TNotification> AddHandler<THandler>() where THandler : class, INotificationHandler<TNotification>
    {
        Services.AddTransient<INotificationHandler<TNotification>, THandler>();
        return this;
    }

    /// <summary>
    /// Adds a notification decoration (behavior) to the service collection.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the notification behavior.</typeparam>
    /// <returns>The current <see cref="NotificationFeatureBuilder{TNotification}"/> instance.</returns>
    public NotificationFeatureBuilder<TNotification> AddDecoration<TBehavior>() where TBehavior : class, INotificationBehavior<TNotification>
    {
        Services.AddTransient<INotificationBehavior<TNotification>, TBehavior>();
        return this;
    }
}
