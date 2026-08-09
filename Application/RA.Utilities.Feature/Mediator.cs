using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature;

/// <summary>
/// Represents a mediator for sending requests, publishing notifications, and handling behaviors.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<Mediator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    public Mediator(IServiceProvider provider, ILogger<Mediator> logger)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ------------------- SEND (with response) -------------------
    /// <summary>
    /// Sends a request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the response.</returns>
    public async Task<Result<TResponse>> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);

        IRequestHandler<TRequest, TResponse> handler =
            _provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors = _provider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

        Task<Result<TResponse>> HandlerDelegate() => handler.HandleAsync(request, cancellationToken);

        RequestHandlerDelegate<TResponse> next = behaviors
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)HandlerDelegate,
                (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

        return await next();
    }

    // ------------------- SEND (no response) -------------------
    /// <summary>
    /// Sends a request without a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Result> Send<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        IRequestHandler<TRequest> handler = _provider.GetRequiredService<IRequestHandler<TRequest>>();

        IEnumerable<IPipelineBehavior<TRequest>> behaviors = _provider.GetServices<IPipelineBehavior<TRequest>>();

        Task<Result> HandlerDelegate() => handler.HandleAsync(request, cancellationToken);

        RequestHandlerDelegate next = behaviors
            .Reverse()
            .Aggregate((RequestHandlerDelegate)HandlerDelegate,
                (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

        return await next();
    }

    // ------------------- PUBLISH -------------------
    /// <inheritdoc/>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlers = _provider.GetServices<INotificationHandler<TNotification>>().ToList();

        if (handlers.Count == 0)
        {
            return;
        }

        var behaviors = _provider.GetServices<INotificationBehavior<TNotification>>().ToList();

        foreach (INotificationHandler<TNotification>? handler in handlers)
        {
            NotificationHandlerDelegate handlerDelegate = () => handler.HandleAsync(notification, cancellationToken);

            foreach (INotificationBehavior<TNotification> behavior in behaviors.Reverse<INotificationBehavior<TNotification>>())
            {
                NotificationHandlerDelegate next = handlerDelegate;
                handlerDelegate = () => behavior.HandleAsync(notification, next, cancellationToken);
            }

            try
            {
                await handlerDelegate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[Publish] Handler {HandlerType} failed for notification {NotificationType}. Notification: {@Notification}",
                    handler.GetType().Name, typeof(TNotification).Name, notification);
            }
        }
    }
}
