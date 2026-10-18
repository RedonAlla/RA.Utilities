using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Behaviors;
using RA.Utilities.Dispatcher.Extensions;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies that <see cref="LoggingBehavior{TRequest, TResponse}"/> logs start/finish/failed
/// events and passes the inner result through unchanged.
/// </summary>
public class LoggingBehaviorTests
{
    [Fact]
    public async Task SuccessfulRequest_LogsStartAndFinished_AndPassesResultThrough()
    {
        var logger = new ListLogger<LoggingBehavior<LoggingRequest, string>>();
        using ServiceProvider provider = CreateProvider(logger);
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<LoggingRequest, string>(new LoggingRequest("ping"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
        logger.Entries.Should().Contain(entry => entry.Level == LogLevel.Information && entry.Message.Contains("[Request Logging] Start.", StringComparison.Ordinal));
        logger.Entries.Should().Contain(entry => entry.Level == LogLevel.Information && entry.Message.Contains("[Request Logging] Finished.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task FailedRequest_LogsWarning_AndPassesResultThrough()
    {
        var logger = new ListLogger<LoggingBehavior<FailingLoggingRequest, string>>();
        using ServiceProvider provider = CreateProvider(logger);
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<FailingLoggingRequest, string>(new FailingLoggingRequest());

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<NotFoundException>();
        logger.Entries.Should().Contain(entry => entry.Level == LogLevel.Warning && entry.Message.Contains("[Request Logging] Failed.", StringComparison.Ordinal));
    }

    private static ServiceProvider CreateProvider(ListLogger<LoggingBehavior<LoggingRequest, string>> logger)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddSingleton<ILogger<LoggingBehavior<LoggingRequest, string>>>(logger);
        services.AddTransient<IPipelineBehavior<LoggingRequest, string>, LoggingBehavior<LoggingRequest, string>>();
        return services.BuildServiceProvider();
    }

    private static ServiceProvider CreateProvider(ListLogger<LoggingBehavior<FailingLoggingRequest, string>> logger)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddSingleton<ILogger<LoggingBehavior<FailingLoggingRequest, string>>>(logger);
        services.AddTransient<IPipelineBehavior<FailingLoggingRequest, string>, LoggingBehavior<FailingLoggingRequest, string>>();
        return services.BuildServiceProvider();
    }
}

public record LoggingRequest(string Value) : IRequest<string>;

public class LoggingHandler : IRequestHandler<LoggingRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(LoggingRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success($"pong:{request.Value}"));
}

public record FailingLoggingRequest : IRequest<string>;

public class FailingLoggingHandler : IRequestHandler<FailingLoggingRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(FailingLoggingRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Failure<string>(new NotFoundException("FailingLoggingRequest", "none")));
}

/// <summary>
/// A minimal <see cref="ILogger{T}"/> that records (level, message) pairs for assertions.
/// </summary>
public class ListLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, string Message)> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Entries.Add((logLevel, formatter(state, exception)));
    }
}
