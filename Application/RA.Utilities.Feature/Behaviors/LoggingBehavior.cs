#pragma warning disable CA1873

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Behaviors;

/// <summary>
/// A logging behavior for Mediator requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => LoggedAsync(request, next);

    /// <inheritdoc/>
    public Task<TResponse> HandleAsync<TContext>(TRequest request, RequestHandlerContextDelegate<TResponse, TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => LoggedAsync(request, () => next(context));

    private async Task<TResponse> LoggedAsync(TRequest request, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        TResponse response = await next().ConfigureAwait(false);
        _logger.LogInformation("[Request Logging] Finished. Response: {@Response}", response);
        return response;
    }
}

/// <summary>
/// A logging behavior for Mediator requests without a response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<LoggingBehavior<TRequest>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest}"/> class.
    /// </summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
        => LoggedAsync(request, next);

    /// <inheritdoc/>
    public Task HandleAsync<TContext>(TRequest request, RequestHandlerContextDelegate<TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => LoggedAsync(request, () => next(context));

    private async Task LoggedAsync(TRequest request, RequestHandlerDelegate next)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        await next().ConfigureAwait(false);
        _logger.LogInformation("[Request Logging] Finished.");
    }
}
