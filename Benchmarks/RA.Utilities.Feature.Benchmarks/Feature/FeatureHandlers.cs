using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Shared request/response handler for all ping requests. Handlers implement the
/// <see cref="IRequestHandler{TRequest, TResponse}"/> interface directly (not the abstract
/// <see cref="RA.Utilities.Feature.Handlers.RequestHandler{TRequest, TResponse}"/> base class,
/// whose extra async wrapper is out of scope for these benchmarks).
/// </summary>
/// <typeparam name="TRequest">The ping request type.</typeparam>
internal sealed class PingResponseHandler<TRequest> : IRequestHandler<TRequest, PongResponse>
    where TRequest : IRequest<PongResponse>, IPingRequest
{
    public Task<Result<PongResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success(new PongResponse(request.Value)));
}

/// <summary>
/// Shared void request handler for all void ping requests.
/// </summary>
/// <typeparam name="TRequest">The void ping request type.</typeparam>
internal sealed class PingVoidHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest, IPingRequest
{
    public Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success());
}

/// <summary>
/// Handler for the typed-context request. The context data itself is read by the behavior;
/// the handler only produces the response.
/// </summary>
internal sealed class ContextReadingHandler : IRequestHandler<ContextPingRequest, PongResponse>
{
    public Task<Result<PongResponse>> HandleAsync(ContextPingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success(new PongResponse(request.Value)));
}

/// <summary>
/// Single handler for the <see cref="OrderPlacedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class OrderPlacedHandler : INotificationHandler<OrderPlacedNotification>
{
    public Task HandleAsync(OrderPlacedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// First of three handlers for the <see cref="OrderFannedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class OrderFannedHandlerA : INotificationHandler<OrderFannedNotification>
{
    public Task HandleAsync(OrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Second of three handlers for the <see cref="OrderFannedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class OrderFannedHandlerB : INotificationHandler<OrderFannedNotification>
{
    public Task HandleAsync(OrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Third of three handlers for the <see cref="OrderFannedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class OrderFannedHandlerC : INotificationHandler<OrderFannedNotification>
{
    public Task HandleAsync(OrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
