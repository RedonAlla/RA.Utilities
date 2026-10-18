using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Mediatr;

/// <summary>
/// Registers the MediatR mediator, handlers, and no-op pipeline behaviors. Pipeline depth is
/// modeled with a distinct request type per depth, since depth is fixed by the DI registrations.
/// Handlers are scoped, matching the Dispatcher and Feature handler lifetimes.
/// </summary>
internal static class MediatRBenchmarkServices
{
    public static IServiceCollection ConfigureSend(IServiceCollection services)
    {
        services.AddNullLogging();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<MediatRPingZeroRequest>();

            config.AddBehavior<MediatRNoOpBehaviorA<MediatRPingOneRequest, PongResponse>>();

            config.AddBehavior<MediatRNoOpBehaviorA<MediatRPingThreeRequest, PongResponse>>();
            config.AddBehavior<MediatRNoOpBehaviorB<MediatRPingThreeRequest, PongResponse>>();
            config.AddBehavior<MediatRNoOpBehaviorC<MediatRPingThreeRequest, PongResponse>>();

            config.AddBehavior<MediatRUnitNoOpBehaviorA<MediatRVoidPingThreeRequest>>();
            config.AddBehavior<MediatRUnitNoOpBehaviorB<MediatRVoidPingThreeRequest>>();
            config.AddBehavior<MediatRUnitNoOpBehaviorC<MediatRVoidPingThreeRequest>>();
        });

        services.AddScoped<IRequestHandler<MediatRPingZeroRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingZeroRequest>>();
        services.AddScoped<IRequestHandler<MediatRPingOneRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingOneRequest>>();
        services.AddScoped<IRequestHandler<MediatRPingThreeRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingThreeRequest>>();

        services.AddScoped<IRequestHandler<MediatRVoidPingRequest>, MediatRPingVoidHandler<MediatRVoidPingRequest>>();
        services.AddScoped<IRequestHandler<MediatRVoidPingThreeRequest>, MediatRPingVoidHandler<MediatRVoidPingThreeRequest>>();

        return services;
    }

    public static IServiceCollection ConfigurePublish(IServiceCollection services)
    {
        services.AddNullLogging();
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<MediatRPingZeroRequest>());
        return services;
    }
}
