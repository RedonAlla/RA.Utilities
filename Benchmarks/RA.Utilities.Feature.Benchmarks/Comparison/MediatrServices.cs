using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

/// <summary>
/// Registers the MediatR mediator, handlers, and pipeline behaviors for the send and
/// publish benchmark scenarios, mirroring the RA.Utilities.Feature registrations.
/// The assembly scan registers the closed handlers (MediatR's scan includes non-public
/// types); the open generic handlers are registered explicitly per closing, since the
/// scan excludes open generics unless <c>RegisterGenericHandlers</c> is enabled.
/// The request handlers are registered scoped to match RA.Utilities.Feature's lifetime.
/// </summary>
internal static class MediatrServices
{
    /// <summary>
    /// Configures the request/response scenarios. Pipeline depth is modeled with distinct
    /// request types per depth, since depth is fixed by the DI registrations.
    /// </summary>
    public static IServiceCollection ConfigureSend(IServiceCollection services)
    {
        // MediatR 14 resolves an ILoggerFactory from the container (for its internal logging);
        // the null logging registration satisfies it and keeps the comparison sink-free.
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

            config.AddBehavior<MediatRContextAwareBehavior>();
        });

        // Scoped, matching RA.Utilities.Feature's handler lifetime (one handler per request/scope).
        services.AddScoped<IRequestHandler<MediatRPingZeroRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingZeroRequest>>();
        services.AddScoped<IRequestHandler<MediatRPingOneRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingOneRequest>>();
        services.AddScoped<IRequestHandler<MediatRPingThreeRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingThreeRequest>>();

        services.AddScoped<IRequestHandler<MediatRVoidPingRequest>, MediatRPingVoidHandler<MediatRVoidPingRequest>>();
        services.AddScoped<IRequestHandler<MediatRVoidPingThreeRequest>, MediatRPingVoidHandler<MediatRVoidPingThreeRequest>>();

        return services;
    }

    /// <summary>
    /// Configures the notification fan-out scenarios.
    /// </summary>
    public static IServiceCollection ConfigurePublish(IServiceCollection services)
    {
        // MediatR 14 resolves an ILoggerFactory from the container; see ConfigureSend.
        services.AddNullLogging();

        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<MediatRPingZeroRequest>());

        return services;
    }

    /// <summary>
    /// Configures the request-behavior scenarios mirroring
    /// <c>BehaviorServices.ConfigureRequestBehaviors</c>: a behavior-less baseline,
    /// logging-only, validation-only, logging + validation, and the void logging + validation
    /// pipeline. Behaviors are registered per closed construction through <c>AddBehavior</c> so
    /// each scenario's pipeline depth stays fixed by its request type.
    /// </summary>
    public static IServiceCollection ConfigureRequestBehaviors(IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<MediatRPingZeroRequest>();

            config.AddBehavior<MediatRLoggingBehavior<MediatRLoggingPingRequest, PongResponse>>();

            config.AddBehavior<MediatRValidationBehavior<MediatRValidatedPingRequest, PongResponse>>();

            config.AddBehavior<MediatRLoggingBehavior<MediatRLoggingValidatedPingRequest, PongResponse>>();
            config.AddBehavior<MediatRValidationBehavior<MediatRLoggingValidatedPingRequest, PongResponse>>();

            config.AddBehavior<MediatRUnitLoggingBehavior<MediatRVoidBehaviorCommand>>();
            config.AddBehavior<MediatRUnitValidationBehavior<MediatRVoidBehaviorCommand>>();
        });

        services.AddNullLogging();

        services.AddTransient<IValidator<MediatRValidatedPingRequest>, MediatRValidatedPingRequestValidator>();
        services.AddTransient<IValidator<MediatRLoggingValidatedPingRequest>, MediatRLoggingValidatedPingRequestValidator>();
        services.AddTransient<IValidator<MediatRVoidBehaviorCommand>, MediatRVoidBehaviorCommandValidator>();

        // Scoped, matching RA.Utilities.Feature's handler lifetime (one handler per request/scope).
        services.AddScoped<IRequestHandler<MediatRPingZeroRequest, PongResponse>, MediatRPingResponseHandler<MediatRPingZeroRequest>>();
        services.AddScoped<IRequestHandler<MediatRLoggingPingRequest, PongResponse>, MediatRPingResponseHandler<MediatRLoggingPingRequest>>();
        services.AddScoped<IRequestHandler<MediatRValidatedPingRequest, PongResponse>, MediatRPingResponseHandler<MediatRValidatedPingRequest>>();
        services.AddScoped<IRequestHandler<MediatRLoggingValidatedPingRequest, PongResponse>, MediatRPingResponseHandler<MediatRLoggingValidatedPingRequest>>();
        services.AddScoped<IRequestHandler<MediatRVoidBehaviorCommand>, MediatRPingVoidHandler<MediatRVoidBehaviorCommand>>();

        return services;
    }
}
