using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

/// <summary>
/// Shared request/response handler for all MediatR ping requests. Returns the plain
/// <see cref="PongResponse"/> (no Result wrapper) — the wrapper's allocation cost is an
/// intentional part of the comparison against RA.Utilities.Feature.
/// </summary>
/// <typeparam name="TRequest">The MediatR ping request type.</typeparam>
internal sealed class MediatRPingResponseHandler<TRequest> : IRequestHandler<TRequest, PongResponse>
    where TRequest : IRequest<PongResponse>, IPingRequest
{
    public Task<PongResponse> Handle(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new PongResponse(request.Value));
}

/// <summary>
/// Shared void request handler for all MediatR void ping requests. MediatR 12 models
/// void commands as requests with a <see cref="Unit"/> response.
/// </summary>
/// <typeparam name="TRequest">The MediatR void ping request type.</typeparam>
internal sealed class MediatRPingVoidHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest, IPingRequest
{
    public Task Handle(TRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Handler for the context-carrying request. The context data is read by the behavior;
/// the handler only produces the response.
/// </summary>
internal sealed class MediatRContextReadingHandler : IRequestHandler<MediatRContextPingRequest, PongResponse>
{
    public Task<PongResponse> Handle(MediatRContextPingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new PongResponse(request.Value));
}

/// <summary>
/// Single handler for the <see cref="MediatROrderPlacedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class MediatROrderPlacedHandler : INotificationHandler<MediatROrderPlacedNotification>
{
    public Task Handle(MediatROrderPlacedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// First of three handlers for the <see cref="MediatROrderFannedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class MediatROrderFannedHandlerA : INotificationHandler<MediatROrderFannedNotification>
{
    public Task Handle(MediatROrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Second of three handlers for the <see cref="MediatROrderFannedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class MediatROrderFannedHandlerB : INotificationHandler<MediatROrderFannedNotification>
{
    public Task Handle(MediatROrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Third of three handlers for the <see cref="MediatROrderFannedNotification"/> fan-out benchmark.
/// </summary>
internal sealed class MediatROrderFannedHandlerC : INotificationHandler<MediatROrderFannedNotification>
{
    public Task Handle(MediatROrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
