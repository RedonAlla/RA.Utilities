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
/// Calls that provide a typed context create an isolated <see cref="PipelineContext{T}"/> that flows
/// through the entire pipeline; calls without a context dispatch through the non-context overloads
/// and allocate no context.
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
        return SendCore<TRequest>(request, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendCore<TRequest, TResponse>(request, cancellationToken);
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

    /// <summary>
    /// No-context dispatch path: composes the pipeline from the non-context overloads and
    /// allocates no <see cref="PipelineContext{T}"/>.
    /// </summary>
    private Task<Result> SendCore<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
    {
        IRequestHandler<TRequest> handler = _provider.GetRequiredService<IRequestHandler<TRequest>>();
        IEnumerable<IPipelineBehavior<TRequest>> behaviors = _provider.GetServices<IPipelineBehavior<TRequest>>();

        RequestHandlerDelegate handlerDelegate =
            () => handler.HandleAsync(request, cancellationToken);

        RequestHandlerDelegate next = behaviors
            .Reverse()
            .Aggregate(handlerDelegate,
                (nextDelegate, behavior) =>
                    () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

        // The composed pipeline is invoked directly, avoiding an async state machine allocation.
        return next();
    }

    /// <summary>
    /// No-context dispatch path: composes the pipeline from the non-context overloads and
    /// allocates no <see cref="PipelineContext{T}"/>.
    /// </summary>
    private Task<Result<TResponse>> SendCore<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        IRequestHandler<TRequest, TResponse> handler = _provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors = _provider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

        RequestHandlerDelegate<TResponse> handlerDelegate =
            () => handler.HandleAsync(request, cancellationToken);

        RequestHandlerDelegate<TResponse> next = behaviors
            .Reverse()
            .Aggregate(handlerDelegate,
                (nextDelegate, behavior) =>
                    () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

        // The composed pipeline is invoked directly, avoiding an async state machine allocation.
        return next();
    }

    /// <summary>
    /// Context dispatch path: a context is created when the caller supplied none, and it flows
    /// through the context-aware overloads so behaviors and handlers can exchange typed data.
    /// </summary>
    private Task<Result> SendCore<TRequest, TContext>(TRequest request, PipelineContext<TContext>? context, CancellationToken cancellationToken)
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

        // The composed pipeline is invoked directly, avoiding an async state machine allocation.
        return next(ctx);
    }

    /// <summary>
    /// Context dispatch path: a context is created when the caller supplied none, and it flows
    /// through the context-aware overloads so behaviors and handlers can exchange typed data.
    /// </summary>
    private Task<Result<TResponse>> SendCore<TRequest, TResponse, TContext>(TRequest request, PipelineContext<TContext>? context, CancellationToken cancellationToken)
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

        // The composed pipeline is invoked directly, avoiding an async state machine allocation.
        return next(ctx);
    }

    // ------------------- PUBLISH (no context) -------------------

    /// <inheritdoc/>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        return PublishCore<TNotification>(notification, cancellationToken);
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

    /// <summary>
    /// No-context publish path: wraps each notification handler with the non-context overloads
    /// and allocates no <see cref="PipelineContext{T}"/>.
    /// </summary>
    private async Task PublishCore<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var handlers = _provider.GetServices<INotificationHandler<TNotification>>().ToList();

        if (handlers.Count == 0)
        {
            return;
        }

        var behaviors = _provider.GetServices<INotificationBehavior<TNotification>>().ToList();

        foreach (INotificationHandler<TNotification>? handler in handlers)
        {
            try
            {
                if (behaviors.Count == 0)
                {
                    // Fast path: no notification behaviors, so invoke the handler directly
                    // without building a wrapper delegate.
                    await handler.HandleAsync(notification, cancellationToken);
                    continue;
                }

                NotificationHandlerDelegate handlerDelegate =
                    () => handler.HandleAsync(notification, cancellationToken);

                foreach (INotificationBehavior<TNotification> behavior in behaviors.Reverse<INotificationBehavior<TNotification>>())
                {
                    NotificationHandlerDelegate next = handlerDelegate;
                    handlerDelegate = () => behavior.HandleAsync(notification, next, cancellationToken);
                }

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

    /// <summary>
    /// Context publish path: a context is created when the caller supplied none, and it flows
    /// through the context-aware overloads so behaviors and handlers can exchange typed data.
    /// </summary>
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
            try
            {
                if (behaviors.Count == 0)
                {
                    // Fast path: no notification behaviors, so invoke the context-aware
                    // handler overload directly without building a wrapper delegate.
                    await handler.HandleAsync(notification, ctx, cancellationToken);
                    continue;
                }

                NotificationHandlerContextDelegate<TContext> handlerDelegate =
                    c => handler.HandleAsync(notification, c, cancellationToken);

                foreach (INotificationBehavior<TNotification> behavior in behaviors.Reverse<INotificationBehavior<TNotification>>())
                {
                    NotificationHandlerContextDelegate<TContext> next = handlerDelegate;
                    handlerDelegate = c => behavior.HandleAsync(notification, next, c, cancellationToken);
                }

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
}
