using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Benchmarks.Shared;
using RA.Utilities.Dispatcher.Extensions;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Dispatcher;

/// <summary>
/// Registers the Dispatcher generated mediator, handlers, and no-op pipeline behaviors. The
/// service setup mirrors the Feature and MediatR configuration scenario for scenario.
/// </summary>
internal static class DispatcherBenchmarkServices
{
    public static IServiceCollection ConfigureSend(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediator();

        services.AddDispatcher<DispatcherPingZeroRequest, PongResponse, DispatcherPingResponseHandler<DispatcherPingZeroRequest>>();

        services.AddDispatcher<DispatcherPingOneRequest, PongResponse, DispatcherPingResponseHandler<DispatcherPingOneRequest>>()
            .AddDecoration<DispatcherNoOpBehavior<DispatcherPingOneRequest, PongResponse>>();

        services.AddDispatcher<DispatcherPingThreeRequest, PongResponse, DispatcherPingResponseHandler<DispatcherPingThreeRequest>>()
            .AddDecoration<DispatcherNoOpBehavior<DispatcherPingThreeRequest, PongResponse>>()
            .AddDecoration<DispatcherNoOpBehavior<DispatcherPingThreeRequest, PongResponse>>()
            .AddDecoration<DispatcherNoOpBehavior<DispatcherPingThreeRequest, PongResponse>>();

        services.AddDispatcher<DispatcherVoidPingRequest, DispatcherPingVoidHandler<DispatcherVoidPingRequest>>();

        services.AddDispatcher<DispatcherVoidPingThreeRequest, DispatcherPingVoidHandler<DispatcherVoidPingThreeRequest>>()
            .AddDecoration<DispatcherNoOpVoidBehavior<DispatcherVoidPingThreeRequest>>()
            .AddDecoration<DispatcherNoOpVoidBehavior<DispatcherVoidPingThreeRequest>>()
            .AddDecoration<DispatcherNoOpVoidBehavior<DispatcherVoidPingThreeRequest>>();

        return services;
    }

    public static IServiceCollection ConfigurePublish(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediator();
        return services;
    }
}
