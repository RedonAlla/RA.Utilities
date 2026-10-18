using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Benchmarks.Shared;
using RA.Utilities.Feature.Extensions;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Feature;

/// <summary>
/// Registers the Feature generated mediator, handlers, and no-op pipeline behaviors. The service
/// setup mirrors the Dispatcher and MediatR configuration scenario for scenario.
/// </summary>
internal static class FeatureBenchmarkServices
{
    public static IServiceCollection ConfigureSend(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediator();

        services.AddFeature<FeaturePingZeroRequest, PongResponse, FeaturePingResponseHandler<FeaturePingZeroRequest>>();

        services.AddFeature<FeaturePingOneRequest, PongResponse, FeaturePingResponseHandler<FeaturePingOneRequest>>()
            .AddDecoration<FeatureNoOpBehavior<FeaturePingOneRequest, PongResponse>>();

        services.AddFeature<FeaturePingThreeRequest, PongResponse, FeaturePingResponseHandler<FeaturePingThreeRequest>>()
            .AddDecoration<FeatureNoOpBehavior<FeaturePingThreeRequest, PongResponse>>()
            .AddDecoration<FeatureNoOpBehavior<FeaturePingThreeRequest, PongResponse>>()
            .AddDecoration<FeatureNoOpBehavior<FeaturePingThreeRequest, PongResponse>>();

        services.AddFeature<FeatureVoidPingRequest, FeaturePingVoidHandler<FeatureVoidPingRequest>>();

        services.AddFeature<FeatureVoidPingThreeRequest, FeaturePingVoidHandler<FeatureVoidPingThreeRequest>>()
            .AddDecoration<FeatureNoOpVoidBehavior<FeatureVoidPingThreeRequest>>()
            .AddDecoration<FeatureNoOpVoidBehavior<FeatureVoidPingThreeRequest>>()
            .AddDecoration<FeatureNoOpVoidBehavior<FeatureVoidPingThreeRequest>>();

        return services;
    }

    public static IServiceCollection ConfigurePublish(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediator();
        return services;
    }
}
