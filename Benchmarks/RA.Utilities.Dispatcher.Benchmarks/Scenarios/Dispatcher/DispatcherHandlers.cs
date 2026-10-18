using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Dispatcher;

/// <summary>
/// Shared request/response handler for all Dispatcher ping requests. Like every Dispatcher
/// handler, it returns a <see cref="Result{TResult}"/> wrapped in a <see cref="ValueTask"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class DispatcherPingResponseHandler<TRequest> : IRequestHandler<TRequest, PongResponse>
    where TRequest : IRequest<PongResponse>, IPingRequest
{
    public ValueTask<Result<PongResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success(new PongResponse(request.Value)));
}

/// <summary>
/// Shared void request handler for all Dispatcher void ping requests.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class DispatcherPingVoidHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest, IPingRequest
{
    public ValueTask<Result> HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success());
}

/// <summary>
/// Single handler for the notification fan-out scenario.
/// </summary>
internal sealed class DispatcherOrderPlacedHandler : INotificationHandler<DispatcherOrderPlacedNotification>
{
    public ValueTask<Result> HandleAsync(DispatcherOrderPlacedNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success());
}

/// <summary>
/// First of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class DispatcherOrderFannedHandlerA : INotificationHandler<DispatcherOrderFannedNotification>
{
    public ValueTask<Result> HandleAsync(DispatcherOrderFannedNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success());
}

/// <summary>
/// Second of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class DispatcherOrderFannedHandlerB : INotificationHandler<DispatcherOrderFannedNotification>
{
    public ValueTask<Result> HandleAsync(DispatcherOrderFannedNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success());
}

/// <summary>
/// Third of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class DispatcherOrderFannedHandlerC : INotificationHandler<DispatcherOrderFannedNotification>
{
    public ValueTask<Result> HandleAsync(DispatcherOrderFannedNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success());
}
