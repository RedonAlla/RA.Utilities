using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Handlers;

/// <summary>
/// Provides a base class for request handlers that return a response.
/// This base class provides built-in logging and automatic exception-to-<see cref="Result{T}"/> conversion,
/// allowing derived classes to focus solely on business logic.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestHandler{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger. Typically the derived handler's typed <see cref="ILogger{TCategoryName}"/>.</param>
    protected RequestHandler(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    async Task<Result<TResponse>> IRequestHandler<TRequest, TResponse>.HandleAsync(
        TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("[Handler] Start Handling {RequestType}", typeof(TRequest).Name);

        try
        {
            TResponse? result =
                await HandleAsync(request, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("[Handler] Finished Handling {RequestType}", typeof(TRequest).Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Handler] Failed Handling {RequestType}", typeof(TRequest).Name);
            return ex;
        }
    }

    /// <inheritdoc/>
    async Task<Result<TResponse>> IRequestHandler<TRequest, TResponse>.HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class
    {
        _logger.LogDebug("[Handler] Start Handling {RequestType}", typeof(TRequest).Name);
        try
        {
            TResponse? result =
                await HandleAsync(request, context, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("[Handler] Finished Handling {RequestType}", typeof(TRequest).Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Handler] Failed Handling {RequestType}", typeof(TRequest).Name);
            return ex;
        }
    }

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
/// This base class provides built-in logging and automatic exception-to-<see cref="Result"/> conversion,
/// allowing derived classes to focus solely on business logic.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestHandler{TRequest}"/> class.
    /// </summary>
    /// <param name="logger">The logger. Typically the derived handler's typed <see cref="ILogger{TCategoryName}"/>.</param>
    protected RequestHandler(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    async Task<Result> IRequestHandler<TRequest>.HandleAsync(
        TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("[Handler] Start Handling {RequestType}", typeof(TRequest).Name);

        try
        {
            Result result = await HandleAsync(request, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("[Handler] Finished Handling {RequestType}", typeof(TRequest).Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Handler] Failed Handling {RequestType}", typeof(TRequest).Name);
            return ex;
        }
    }

    /// <inheritdoc/>
    async Task<Result> IRequestHandler<TRequest>.HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class
    {
        _logger.LogDebug("[Handler] Start Handling {RequestType}", typeof(TRequest).Name);

        try
        {
            Result result =
                await HandleAsync(request, context, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("[Handler] Finished Handling {RequestType}", typeof(TRequest).Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Handler] Failed Handling {RequestType}", typeof(TRequest).Name);
            return ex;
        }
    }

    /// <summary>
    /// Handles the request. Override this method in derived classes to implement business logic.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the request with a typed pipeline context. The default delegates to
    /// <see cref="HandleAsync(TRequest, CancellationToken)"/>. Override to consume context data.
    /// </summary>
    protected virtual Task<Result> HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken);
}
