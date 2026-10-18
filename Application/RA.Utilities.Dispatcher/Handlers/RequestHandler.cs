using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Handlers;

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
    ValueTask<Result<TResponse>> IRequestHandler<TRequest, TResponse>.HandleAsync(
        TRequest request, CancellationToken cancellationToken) =>
            HandleAsync(request, cancellationToken);

    /// <inheritdoc/>
    ValueTask<Result<TResponse>> IRequestHandler<TRequest, TResponse>.HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class =>
            HandleAsync(request, context, cancellationToken);

    /// <summary>
    /// Handles the request. Override this method in derived classes to implement business logic.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with the result.</returns>
    public abstract ValueTask<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the request with a typed pipeline context. The default delegates to
    /// <see cref="HandleAsync(TRequest, CancellationToken)"/>. Override to consume context data.
    /// </summary>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    /// <param name="request">The request to handle.</param>
    /// <param name="context">The pipeline context for this execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with the result.</returns>
    protected virtual ValueTask<Result<TResponse>> HandleAsync<TContext>(
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
    ValueTask<Result> IRequestHandler<TRequest>.HandleAsync(
        TRequest request, CancellationToken cancellationToken) =>
            HandleAsync(request, cancellationToken);

    /// <inheritdoc/>
    ValueTask<Result> IRequestHandler<TRequest>.HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class =>
            HandleAsync(request, context, cancellationToken);

    /// <summary>
    /// Handles the request. Override this method in derived classes to implement business logic.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with a result.</returns>
    public abstract ValueTask<Result> HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the request with a typed pipeline context. The default delegates to
    /// <see cref="HandleAsync(TRequest, CancellationToken)"/>. Override to consume context data.
    /// </summary>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    /// <param name="request">The request to handle.</param>
    /// <param name="context">The pipeline context for this execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation, with a result.</returns>
    protected virtual ValueTask<Result> HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken);
}
