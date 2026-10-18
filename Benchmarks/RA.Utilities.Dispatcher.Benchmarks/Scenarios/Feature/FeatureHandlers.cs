using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Feature;

/// <summary>
/// Shared request/response handler for all Feature ping requests, registered explicitly through
/// <c>AddFeature</c> to mirror the Dispatcher and MediatR setups.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class FeaturePingResponseHandler<TRequest> : IRequestHandler<TRequest, PongResponse>
    where TRequest : IRequest<PongResponse>, IPingRequest
{
    public Task<PongResponse> HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new PongResponse(request.Value));
}

/// <summary>
/// Shared void request handler for all Feature void ping requests.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class FeaturePingVoidHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest, IPingRequest
{
    public Task HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Single handler for the notification fan-out scenario.
/// </summary>
internal sealed class FeatureOrderPlacedHandler : INotificationHandler<FeatureOrderPlacedNotification>
{
    public Task HandleAsync(FeatureOrderPlacedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// First of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class FeatureOrderFannedHandlerA : INotificationHandler<FeatureOrderFannedNotification>
{
    public Task HandleAsync(FeatureOrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Second of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class FeatureOrderFannedHandlerB : INotificationHandler<FeatureOrderFannedNotification>
{
    public Task HandleAsync(FeatureOrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Third of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class FeatureOrderFannedHandlerC : INotificationHandler<FeatureOrderFannedNotification>
{
    public Task HandleAsync(FeatureOrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
