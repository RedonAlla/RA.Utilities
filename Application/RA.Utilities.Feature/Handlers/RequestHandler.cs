using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Handlers;

/// <summary>
/// Provides a base class for request handlers that return a response.
/// Derived classes implement the business logic in
/// <see cref="HandleAsync(TRequest, CancellationToken)"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc/>
    Task<TResponse> IRequestHandler<TRequest, TResponse>.HandleAsync(
        TRequest request, CancellationToken cancellationToken) =>
            HandleAsync(request, cancellationToken);

    /// <inheritdoc/>
    Task<TResponse> IRequestHandler<TRequest, TResponse>.HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class =>
            HandleAsync(request, context, cancellationToken);

    /// <summary>
    /// Handles the request. Override this method in derived classes to implement business logic.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the response.</returns>
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the request with a typed pipeline context. The default delegates to
    /// <see cref="HandleAsync(TRequest, CancellationToken)"/>. Override to consume context data.
    /// </summary>
    protected virtual Task<TResponse> HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken);
}

/// <summary>
/// Provides a base class for request handlers that do not return a value.
/// Derived classes implement the business logic in
/// <see cref="HandleAsync(TRequest, CancellationToken)"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    /// <inheritdoc/>
    Task IRequestHandler<TRequest>.HandleAsync(
        TRequest request, CancellationToken cancellationToken) =>
            HandleAsync(request, cancellationToken);

    /// <inheritdoc/>
    Task IRequestHandler<TRequest>.HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class =>
            HandleAsync(request, context, cancellationToken);

    /// <summary>
    /// Handles the request. Override this method in derived classes to implement business logic.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the request with a typed pipeline context. The default delegates to
    /// <see cref="HandleAsync(TRequest, CancellationToken)"/>. Override to consume context data.
    /// </summary>
    protected virtual Task HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken);
}
