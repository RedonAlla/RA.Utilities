using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Extensions;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Registers the generated mediator and the benchmark handlers. The generic ping handlers are
/// registered explicitly (the generator skips generic handler types with FEAG002); the closed
/// behaviors (GeneratedNoOpBehaviorA/B/C, ContextAwareBehavior) are auto-registered by the
/// source generator.
/// </summary>
internal static class GeneratedMediatorServices
{
    public static IServiceCollection ConfigureSend(IServiceCollection services)
    {
        services.AddSingleton<ILogger<Generated.MediatorImpl>>(NullLogger<Generated.MediatorImpl>.Instance);
        services.AddMediator();

        services.AddScoped<Abstractions.IRequestHandler<PingZeroRequest, PongResponse>, PingResponseHandler<PingZeroRequest>>();
        services.AddScoped<Abstractions.IRequestHandler<PingOneRequest, PongResponse>, PingResponseHandler<PingOneRequest>>();
        services.AddScoped<Abstractions.IRequestHandler<PingThreeRequest, PongResponse>, PingResponseHandler<PingThreeRequest>>();
        services.AddScoped<Abstractions.IRequestHandler<VoidPingRequest>, PingVoidHandler<VoidPingRequest>>();
        services.AddScoped<Abstractions.IRequestHandler<VoidPingThreeRequest>, PingVoidHandler<VoidPingThreeRequest>>();
        services.AddScoped<Abstractions.IRequestHandler<ContextPingRequest, PongResponse>, ContextReadingHandler>();

        return services;
    }
}
