using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Abstractions;

/// <summary>
/// Defines a dispatcher for sending requests and publishing notifications.
/// Dispatch operations return a <see cref="Result"/> or <see cref="Result{TResult}"/> instead of
/// throwing for expected failures.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Sends a request without a response.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with a result.</returns>
    ValueTask<Result> Send<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest;

    /// <summary>
    /// Sends a request with a response.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with the result.</returns>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    ValueTask<Result<TResponse>> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest<TResponse>;

    /// <summary>
    /// Sends a request without a response, with a typed pipeline context.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="context">The pipeline context for this execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with a result.</returns>
    ValueTask<Result> Send<TRequest, TContext>(
        TRequest request,
        PipelineContext<TContext>? context = null,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest
        where TContext : class, new();

    /// <summary>
    /// Sends a request with a response, with a typed pipeline context.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="context">The pipeline context for this execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with the result.</returns>
    ValueTask<Result<TResponse>> Send<TRequest, TResponse, TContext>(TRequest request, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
        where TContext : class, new();

    /// <summary>
    /// Publishes a notification. All handlers run; failures are aggregated into the returned result.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with a result.</returns>
    ValueTask<Result> Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;

    /// <summary>
    /// Publishes a notification with a typed pipeline context. All handlers run; failures are
    /// aggregated into the returned result.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="context">The pipeline context for this execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with a result.</returns>
    ValueTask<Result> Publish<TNotification, TContext>(TNotification notification, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TNotification : INotification
        where TContext : class, new();
}
