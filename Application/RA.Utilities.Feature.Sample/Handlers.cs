using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Sample;

public record LocalGreetingRequest(string Name) : IRequest<string>;

public class LocalGreetingHandler : IRequestHandler<LocalGreetingRequest, string>
{
    public Task<string> HandleAsync(LocalGreetingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult($"Hello, {request.Name}!");
}

// ------------------- typed pipeline context -------------------

/// <summary>
/// The user-defined context data carried through a pipeline execution. The handler writes
/// <see cref="HandlerResult"/>; the behaviors read it after the handler has run.
/// </summary>
public class GreetingContext
{
    public string? CorrelationId { get; set; }

    public string? HandlerResult { get; set; }
}

public record ContextGreetingRequest(string Name) : IRequest<string>;

/// <summary>
/// The outermost of the two closed pipeline behaviors for the context demo. It stamps the
/// correlation id before the handler runs and logs the context data around the whole pipeline —
/// including the <see cref="GreetingContext.HandlerResult"/> the handler writes.
/// </summary>
public class FirstContextLoggingBehavior : IPipelineBehavior<ContextGreetingRequest, string>
{
    private readonly ILogger<FirstContextLoggingBehavior> _logger;

    public FirstContextLoggingBehavior(ILogger<FirstContextLoggingBehavior> logger)
    {
        _logger = logger;
    }

    public Task<string> HandleAsync(ContextGreetingRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
        next();

    public async Task<string> HandleAsync<TContext>(
        ContextGreetingRequest request,
        RequestHandlerContextDelegate<string, TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken)
        where TContext : class, new()
    {
        var data = (GreetingContext)(object)context.Data;
        data.CorrelationId = Guid.NewGuid().ToString("N");
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Before handler — CorrelationId: {CorrelationId}, HandlerResult: {HandlerResult}",
                data.CorrelationId, data.HandlerResult ?? "(not written yet)");
        }

        string result = await next(context);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("After handler — CorrelationId: {CorrelationId}, HandlerResult: {HandlerResult}",
                data.CorrelationId, data.HandlerResult);
        }
        return result;
    }
}

/// <summary>
/// The innermost of the two closed pipeline behaviors: it runs directly around the handler and
/// logs the context data, showing the handler-written data flowing back out to the behaviors.
/// </summary>
public class SecondContextLoggingBehavior : IPipelineBehavior<ContextGreetingRequest, string>
{
    private readonly ILogger<SecondContextLoggingBehavior> _logger;

    public SecondContextLoggingBehavior(ILogger<SecondContextLoggingBehavior> logger)
    {
        _logger = logger;
    }

    public Task<string> HandleAsync(ContextGreetingRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
        next();

    public async Task<string> HandleAsync<TContext>(
        ContextGreetingRequest request,
        RequestHandlerContextDelegate<string, TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken)
        where TContext : class, new()
    {
        var data = (GreetingContext)(object)context.Data;
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Before handler — CorrelationId: {CorrelationId}, HandlerResult: {HandlerResult}",
                data.CorrelationId, data.HandlerResult ?? "(not written yet)");
        }

        string result = await next(context);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("After handler — CorrelationId: {CorrelationId}, HandlerResult: {HandlerResult}",
                data.CorrelationId, data.HandlerResult);
        }
        return result;
    }
}

/// <summary>
/// The handler writes its result into the typed context (visible to the behaviors after it runs)
/// and reads the correlation id the first behavior stamped (visible to the handler before it runs).
/// </summary>
public class ContextGreetingHandler : IRequestHandler<ContextGreetingRequest, string>
{
    public Task<string> HandleAsync(ContextGreetingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult($"Hello, {request.Name}!");

    public Task<string> HandleAsync<TContext>(
        ContextGreetingRequest request,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken)
        where TContext : class, new()
    {
        var data = (GreetingContext)(object)context.Data;
        data.HandlerResult = $"processed '{request.Name}'";
        return Task.FromResult($"Hello, {request.Name}! (correlation: {data.CorrelationId})");
    }
}
