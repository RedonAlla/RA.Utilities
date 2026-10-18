using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Behaviors;
using RA.Utilities.Dispatcher.Extensions;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the built-in notification behaviors: retry (happy path, recovery, give-up),
/// logging, and metrics — each wraps a single handler and passes its result through.
/// </summary>
public class NotificationBehaviorsTests
{
    [Fact]
    public async Task RetryBehavior_SuccessfulHandler_RunsOnce()
    {
        using ServiceProvider provider = CreateRetryProvider<RetryNotification>();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        FlakyNotificationHandler.Attempts = 0;
        FlakyNotificationHandler.FailuresBeforeSuccess = 0;
        Result result = await dispatcher.Publish(new RetryNotification());

        result.IsSuccess.Should().BeTrue();
        FlakyNotificationHandler.Attempts.Should().Be(1);
    }

    [Fact]
    public async Task RetryBehavior_FailingThenSucceedingHandler_RetriesAndRecovers()
    {
        using ServiceProvider provider = CreateRetryProvider<RetryNotification>();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        FlakyNotificationHandler.Attempts = 0;
        FlakyNotificationHandler.FailuresBeforeSuccess = 1;

        Result result = await dispatcher.Publish(new RetryNotification());

        result.IsSuccess.Should().BeTrue();
        FlakyNotificationHandler.Attempts.Should().Be(2);
    }

    [Fact]
    public async Task RetryBehavior_AlwaysFailingHandler_ReturnsLastFailure_AfterMaxRetries()
    {
        using ServiceProvider provider = CreateRetryProvider<GiveUpNotification>();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        GiveUpNotificationHandler.Attempts = 0;
        Result result = await dispatcher.Publish(new GiveUpNotification());

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<InvalidOperationException>();
        GiveUpNotificationHandler.Attempts.Should().Be(3); // maxRetries default
    }

    [Fact]
    public async Task LoggingAndMetricsBehaviors_PassTheResultThrough()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<INotificationBehavior<MeasuredNotification>, NotificationLoggingBehavior<MeasuredNotification>>();
        services.AddTransient<INotificationBehavior<MeasuredNotification>, NotificationMetricsBehavior<MeasuredNotification>>();
        using ServiceProvider provider = services.BuildServiceProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Publish(new MeasuredNotification("order-1"));

        result.IsSuccess.Should().BeTrue();
        MeasuredNotificationHandler.ReceivedPayloads.Should().Contain("order-1");
    }

    private static ServiceProvider CreateRetryProvider<TNotification>()
        where TNotification : INotification
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        // Zero backoff so the tests measure the retry machinery, not a delay loop.
        services.AddTransient<INotificationBehavior<TNotification>>(sp =>
            new NotificationRetryBehavior<TNotification>(
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<NotificationRetryBehavior<TNotification>>>(),
                maxRetries: 3,
                baseDelayMilliseconds: 0));
        return services.BuildServiceProvider();
    }
}

public record RetryNotification : INotification;

public class FlakyNotificationHandler : INotificationHandler<RetryNotification>
{
    public static int Attempts { get; set; }
    public static int FailuresBeforeSuccess { get; set; }

    public ValueTask<Result> HandleAsync(RetryNotification notification, CancellationToken cancellationToken)
    {
        Attempts++;
        if (Attempts <= FailuresBeforeSuccess)
            return ValueTask.FromResult(Result.Failure(new InvalidOperationException("transient failure")));

        return ValueTask.FromResult(Result.Success());
    }
}

public record GiveUpNotification : INotification;

public class GiveUpNotificationHandler : INotificationHandler<GiveUpNotification>
{
    public static int Attempts { get; set; }

    public ValueTask<Result> HandleAsync(GiveUpNotification notification, CancellationToken cancellationToken)
    {
        Attempts++;
        return ValueTask.FromResult(Result.Failure(new InvalidOperationException("permanent failure")));
    }
}

public record MeasuredNotification(string Payload) : INotification;

public class MeasuredNotificationHandler : INotificationHandler<MeasuredNotification>
{
    public static System.Collections.Generic.List<string> ReceivedPayloads { get; } = [];

    public ValueTask<Result> HandleAsync(MeasuredNotification notification, CancellationToken cancellationToken)
    {
        ReceivedPayloads.Add(notification.Payload);
        return ValueTask.FromResult(Result.Success());
    }
}
