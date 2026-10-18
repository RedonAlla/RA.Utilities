using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Mediatr;

/// <summary>
/// Shared request/response handler for all MediatR ping requests.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class MediatRPingResponseHandler<TRequest> : IRequestHandler<TRequest, PongResponse>
    where TRequest : IRequest<PongResponse>, IPingRequest
{
    public Task<PongResponse> Handle(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new PongResponse(request.Value));
}

/// <summary>
/// Shared void request handler for all MediatR void ping requests. MediatR models void requests
/// as requests with a <c>Unit</c> response.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class MediatRPingVoidHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest, IPingRequest
{
    public Task Handle(TRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Single handler for the notification fan-out scenario.
/// </summary>
internal sealed class MediatROrderPlacedHandler : INotificationHandler<MediatROrderPlacedNotification>
{
    public Task Handle(MediatROrderPlacedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// First of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class MediatROrderFannedHandlerA : INotificationHandler<MediatROrderFannedNotification>
{
    public Task Handle(MediatROrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Second of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class MediatROrderFannedHandlerB : INotificationHandler<MediatROrderFannedNotification>
{
    public Task Handle(MediatROrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Third of three handlers for the three-handler fan-out scenario.
/// </summary>
internal sealed class MediatROrderFannedHandlerC : INotificationHandler<MediatROrderFannedNotification>
{
    public Task Handle(MediatROrderFannedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
