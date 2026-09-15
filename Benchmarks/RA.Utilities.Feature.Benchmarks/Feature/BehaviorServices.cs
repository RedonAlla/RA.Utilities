using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Behaviors;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Extensions;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Registers the generated mediator plus the package's real pipeline behaviors for the behavior
/// benchmark scenarios. Behaviors are wired through the public feature API
/// (<c>AddFeature</c>/<c>AddNotification</c> with <c>AddDecoration</c>/<c>AddValidator</c>)
/// exactly as an application would; the shared ping handlers are registered explicitly because
/// they are generic (the generator skips generic handler types), and the notification handlers
/// are auto-registered because they are not.
/// </summary>
internal static class BehaviorServices
{
    /// <summary>
    /// Configures the request-behavior scenarios: a behavior-less baseline, logging-only,
    /// validation-only, logging + validation, and the void logging + validation pipeline.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureRequestBehaviors(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediator();

        services.AddFeature<PingZeroRequest, PongResponse, PingResponseHandler<PingZeroRequest>>();

        services.AddFeature<LoggingPingRequest, PongResponse, PingResponseHandler<LoggingPingRequest>>()
            .AddDecoration<LoggingBehavior<LoggingPingRequest, PongResponse>>();

        services.AddFeature<ValidatedPingRequest, PongResponse, PingResponseHandler<ValidatedPingRequest>>()
            .AddValidator<ValidatedPingRequestValidator>();

        services.AddFeature<LoggingValidatedPingRequest, PongResponse, PingResponseHandler<LoggingValidatedPingRequest>>()
            .AddDecoration<LoggingBehavior<LoggingValidatedPingRequest, PongResponse>>()
            .AddValidator<LoggingValidatedPingRequestValidator>();

        services.AddFeature<VoidBehaviorCommand, PingVoidHandler<VoidBehaviorCommand>>()
            .AddDecoration<LoggingBehavior<VoidBehaviorCommand>>()
            .AddValidator<VoidBehaviorCommandValidator>();

        return services;
    }

    /// <summary>
    /// Configures the notification-behavior scenarios: a behavior-less baseline, each of the
    /// three package notification behaviors alone, the retry behavior's failure-and-recovery
    /// path, and the three behaviors stacked.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureNotificationBehaviors(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediator();

        services.AddNotification<LoggedNotification>()
            .AddDecoration<NotificationLoggingBehavior<LoggedNotification>>();

        services.AddNotification<MeasuredNotification>()
            .AddDecoration<NotificationMetricsBehavior<MeasuredNotification>>();

        services.AddNotification<RetryImmediateNotification>()
            .AddDecoration<NotificationRetryBehavior<RetryImmediateNotification>>();

        // The failure-path scenario resolves the retry behavior through a factory with a zero
        // backoff delay: a realistic 200 ms delay would swamp the microbenchmark — what is
        // measured here is the retry machinery (catch, warn, loop), not the backoff timer.
        services.AddTransient<INotificationBehavior<RetryNotification>>(static provider =>
            new NotificationRetryBehavior<RetryNotification>(
                provider.GetRequiredService<ILogger<NotificationRetryBehavior<RetryNotification>>>(),
                maxRetries: 2,
                baseDelayMilliseconds: 0));

        services.AddNotification<StackedNotification>()
            .AddDecoration<NotificationLoggingBehavior<StackedNotification>>()
            .AddDecoration<NotificationMetricsBehavior<StackedNotification>>()
            .AddDecoration<NotificationRetryBehavior<StackedNotification>>();

        return services;
    }
}
