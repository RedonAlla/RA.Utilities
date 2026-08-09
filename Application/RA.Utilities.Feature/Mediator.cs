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
/// Each <see cref="Send{TRequest, TResponse, TContext}"/> and <see cref="Publish{TNotification, TContext}"/> call
/// creates an isolated <see cref="PipelineContext{T}"/> that flows through the entire pipeline.
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

    // ------------------- SEND (no context) -------------------

    /// <inheritdoc/>
    public Task<Result> Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendCore<TRequest, TContextMarker>(request, null, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendCore<TRequest, TResponse, TContextMarker>(request, null, cancellationToken);
    }

    // ------------------- SEND (with context) -------------------

    /// <inheritdoc/>
    public Task<Result> Send<TRequest, TContext>(TRequest request, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TRequest : IRequest
        where TContext : class, new()
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendCore<TRequest, TContext>(request, context, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TRequest, TResponse, TContext>(TRequest request, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
        where TContext : class, new()
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendCore<TRequest, TResponse, TContext>(request, context, cancellationToken);
    }

    private async Task<Result> SendCore<TRequest, TContext>(TRequest request, PipelineContext<TContext>? context, CancellationToken cancellationToken)
        where TRequest : IRequest
        where TContext : class, new()
    {
        PipelineContext<TContext> ctx = context ?? new PipelineContext<TContext>();
        IRequestHandler<TRequest> handler = _provider.GetRequiredService<IRequestHandler<TRequest>>();
        IEnumerable<IPipelineBehavior<TRequest>> behaviors = _provider.GetServices<IPipelineBehavior<TRequest>>();

        RequestHandlerContextDelegate<TContext> handlerDelegate =
            c => handler.HandleAsync(request, c, cancellationToken);

        RequestHandlerContextDelegate<TContext> next = behaviors
            .Reverse()
            .Aggregate(handlerDelegate,
                (nextDelegate, behavior) =>
                    c => behavior.HandleAsync(request, nextDelegate, c, cancellationToken));

        return await next(ctx);
    }

    private async Task<Result<TResponse>> SendCore<TRequest, TResponse, TContext>(TRequest request, PipelineContext<TContext>? context, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
        where TContext : class, new()
    {
        PipelineContext<TContext> ctx = context ?? new PipelineContext<TContext>();
        IRequestHandler<TRequest, TResponse> handler = _provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors = _provider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

        RequestHandlerContextDelegate<TResponse, TContext> handlerDelegate =
            c => handler.HandleAsync(request, c, cancellationToken);

        RequestHandlerContextDelegate<TResponse, TContext> next = behaviors
            .Reverse()
            .Aggregate(handlerDelegate,
                (nextDelegate, behavior) =>
                    c => behavior.HandleAsync(request, nextDelegate, c, cancellationToken));

        return await next(ctx);
    }

    // ------------------- PUBLISH (no context) -------------------

    /// <inheritdoc/>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        return PublishCore<TNotification, TContextMarker>(notification, null, cancellationToken);
    }

    // ------------------- PUBLISH (with context) -------------------

    /// <inheritdoc/>
    public Task Publish<TNotification, TContext>(TNotification notification, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TNotification : INotification
        where TContext : class, new()
    {
        ArgumentNullException.ThrowIfNull(notification);
        return PublishCore<TNotification, TContext>(notification, context, cancellationToken);
    }

    private async Task PublishCore<TNotification, TContext>(TNotification notification, PipelineContext<TContext>? context, CancellationToken cancellationToken)
        where TNotification : INotification
        where TContext : class, new()
    {
        PipelineContext<TContext> ctx = context ?? new PipelineContext<TContext>();
        var handlers = _provider.GetServices<INotificationHandler<TNotification>>().ToList();

        if (handlers.Count == 0)
        {
            return;
        }

        var behaviors = _provider.GetServices<INotificationBehavior<TNotification>>().ToList();

        foreach (INotificationHandler<TNotification>? handler in handlers)
        {
            NotificationHandlerContextDelegate<TContext> handlerDelegate =
                c => handler.HandleAsync(notification, c, cancellationToken);

            foreach (INotificationBehavior<TNotification> behavior in behaviors.Reverse<INotificationBehavior<TNotification>>())
            {
                NotificationHandlerContextDelegate<TContext> next = handlerDelegate;
                handlerDelegate = c => behavior.HandleAsync(notification, next, c, cancellationToken);
            }

            try
            {
                await handlerDelegate(ctx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[Publish] Handler {HandlerType} failed for notification {NotificationType}. Notification: {@Notification}",
                    handler.GetType().Name, typeof(TNotification).Name, notification);
            }
        }
    }

    /// <summary>
    /// Internal marker type used when no user-defined context is provided.
    /// </summary>
    private sealed class TContextMarker
    {
    }
}
