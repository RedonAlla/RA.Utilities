using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// Defines a mediator interface for sending requests and publishing notifications.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request without a response.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> Send<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest;

    /// <summary>
    /// Sends a request with a response.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the response.</returns>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    Task<Result<TResponse>> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest<TResponse>;

    /// <summary>
    /// Sends a request without a response, with a typed pipeline context.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    Task<Result> Send<TRequest, TContext>(
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
    Task<Result<TResponse>> Send<TRequest, TResponse, TContext>(TRequest request, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
        where TContext : class, new();

    /// <summary>
    /// Publishes a notification.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;

    /// <summary>
    /// Publishes a notification with a typed pipeline context.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    Task Publish<TNotification, TContext>(TNotification notification, PipelineContext<TContext>? context = null, CancellationToken cancellationToken = default)
        where TNotification : INotification
        where TContext : class, new();
}
