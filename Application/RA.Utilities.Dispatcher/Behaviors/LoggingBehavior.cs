#pragma warning disable CA1873

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Attributes;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Behaviors;

/// <summary>
/// A logging behavior for dispatcher requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
[PipelineBehaviorOrder(0)]
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ValueTask<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => LoggedAsync(request, next);

    /// <inheritdoc/>
    public ValueTask<Result<TResponse>> HandleAsync<TContext>(TRequest request, RequestHandlerContextDelegate<TResponse, TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => LoggedAsync(request, () => next(context));

    private async ValueTask<Result<TResponse>> LoggedAsync(TRequest request, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        Result<TResponse> result = await next().ConfigureAwait(false);

        if (result.IsFailure)
            _logger.LogWarning(result.Exception, "[Request Logging] Failed.");
        else
            _logger.LogInformation("[Request Logging] Finished. Response: {@Response}", result.Value);

        return result;
    }
}

/// <summary>
/// A logging behavior for dispatcher requests without a response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<LoggingBehavior<TRequest>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ValueTask<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
        => LoggedAsync(request, next);

    /// <inheritdoc/>
    public ValueTask<Result> HandleAsync<TContext>(TRequest request, RequestHandlerContextDelegate<TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => LoggedAsync(request, () => next(context));

    private async ValueTask<Result> LoggedAsync(TRequest request, RequestHandlerDelegate next)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        Result result = await next().ConfigureAwait(false);

        if (result.IsFailure)
            _logger.LogWarning(result.Exception, "[Request Logging] Failed.");
        else
            _logger.LogInformation("[Request Logging] Finished.");

        return result;
    }
}
