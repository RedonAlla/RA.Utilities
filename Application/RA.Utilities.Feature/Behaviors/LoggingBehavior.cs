using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
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
    public async Task<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        Result<TResponse> response = await next();
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
    public async Task<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Request Logging] Request: {@Request}", request);
        Result results = await next();
        _logger.LogInformation("[Request Logging] Finished");

        return results;
    }
}
