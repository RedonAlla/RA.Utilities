using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Extensions;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Registers the RA.Utilities.Feature mediator, handlers, and pipeline behaviors
/// for the send and publish benchmark scenarios.
/// </summary>
internal static class FeatureServices
{
    /// <summary>
    /// Configures the request/response scenarios. Pipeline depth is modeled with distinct
    /// request types per depth, since depth is fixed by the DI registrations.
    /// </summary>
    public static IServiceCollection ConfigureSend(IServiceCollection services)
    {
        services.AddSingleton<ILogger<Mediator>>(NullLogger<Mediator>.Instance);
        services.AddMediator();

        services.AddFeature<PingZeroRequest, PongResponse, PingResponseHandler<PingZeroRequest>>();

        services.AddFeature<PingOneRequest, PongResponse, PingResponseHandler<PingOneRequest>>()
                .AddDecoration<NoOpBehaviorA<PingOneRequest, PongResponse>>();

        services.AddFeature<PingThreeRequest, PongResponse, PingResponseHandler<PingThreeRequest>>()
                .AddDecoration<NoOpBehaviorA<PingThreeRequest, PongResponse>>()
                .AddDecoration<NoOpBehaviorB<PingThreeRequest, PongResponse>>()
                .AddDecoration<NoOpBehaviorC<PingThreeRequest, PongResponse>>();

        services.AddFeature<VoidPingRequest, PingVoidHandler<VoidPingRequest>>();

        services.AddFeature<VoidPingThreeRequest, PingVoidHandler<VoidPingThreeRequest>>()
                .AddDecoration<VoidNoOpBehaviorA<VoidPingThreeRequest>>()
                .AddDecoration<VoidNoOpBehaviorB<VoidPingThreeRequest>>()
                .AddDecoration<VoidNoOpBehaviorC<VoidPingThreeRequest>>();

        services.AddFeature<ContextPingRequest, PongResponse, ContextReadingHandler>()
                .AddDecoration<ContextAwareBehavior>();

        return services;
    }

    /// <summary>
    /// Configures the notification fan-out scenarios.
    /// </summary>
    public static IServiceCollection ConfigurePublish(IServiceCollection services)
    {
        services.AddSingleton<ILogger<Mediator>>(NullLogger<Mediator>.Instance);
        services.AddMediator();

        services.AddNotification<OrderPlacedNotification>()
                .AddHandler<OrderPlacedHandler>();

        services.AddNotification<OrderFannedNotification>()
                .AddHandler<OrderFannedHandlerA>()
                .AddHandler<OrderFannedHandlerB>()
                .AddHandler<OrderFannedHandlerC>();

        return services;
    }
}
