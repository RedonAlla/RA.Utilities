using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Extensions;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the generated <c>Publish</c> fan-out: every handler runs, failures (returned or
/// thrown) are captured per handler, and the aggregated outcome is returned as a
/// <see cref="Result"/> — a single exception when one handler fails, an
/// <see cref="AggregateException"/> when several do.
/// </summary>
public class PublishTests
{
    [Fact]
    public async Task Publish_RunsEveryHandler_AndReturnsSuccess()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Publish(new FanOutNotification(42));

        result.IsSuccess.Should().BeTrue();
        FanOutHandlerA.ReceivedOrderIds.Should().Contain(42);
        FanOutHandlerB.ReceivedOrderIds.Should().Contain(42);
    }

    [Fact]
    public async Task Publish_WithOneFailingHandler_ReturnsItsException_AndRunsTheOthers()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Publish(new PartialFailureNotification(7));

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<InvalidOperationException>();
        PartialFailureSucceedingHandler.ReceivedOrderIds.Should().Contain(7);
    }

    [Fact]
    public async Task Publish_WithMultipleFailingHandlers_AggregatesFailures()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Publish(new MultiFailureNotification());

        result.IsFailure.Should().BeTrue();
        AggregateException aggregate = result.Exception.Should().BeOfType<AggregateException>().Which;
        aggregate.InnerExceptions.Should().HaveCount(2);
    }

    [Fact]
    public async Task Publish_CapturesThrownHandlerExceptions_IntoTheResult()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Func<Task<Result>> act = () => dispatcher.Publish(new ThrowingNotification()).AsTask();

        // The exception is captured into the result, not propagated.
        Result result = (await act.Should().NotThrowAsync()).Subject;
        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task Publish_WithNoHandlers_ReturnsSuccess()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Publish(new UnhandledNotification());

        result.IsSuccess.Should().BeTrue();
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        return services.BuildServiceProvider();
    }
}

public record FanOutNotification(int OrderId) : INotification;

public class FanOutHandlerA : INotificationHandler<FanOutNotification>
{
    public static System.Collections.Generic.List<int> ReceivedOrderIds { get; } = [];

    public ValueTask<Result> HandleAsync(FanOutNotification notification, CancellationToken cancellationToken)
    {
        ReceivedOrderIds.Add(notification.OrderId);
        return ValueTask.FromResult(Result.Success());
    }
}

public class FanOutHandlerB : INotificationHandler<FanOutNotification>
{
    public static System.Collections.Generic.List<int> ReceivedOrderIds { get; } = [];

    public ValueTask<Result> HandleAsync(FanOutNotification notification, CancellationToken cancellationToken)
    {
        ReceivedOrderIds.Add(notification.OrderId);
        return ValueTask.FromResult(Result.Success());
    }
}

public record PartialFailureNotification(int OrderId) : INotification;

public class PartialFailureSucceedingHandler : INotificationHandler<PartialFailureNotification>
{
    public static System.Collections.Generic.List<int> ReceivedOrderIds { get; } = [];

    public ValueTask<Result> HandleAsync(PartialFailureNotification notification, CancellationToken cancellationToken)
    {
        ReceivedOrderIds.Add(notification.OrderId);
        return ValueTask.FromResult(Result.Success());
    }
}

public class PartialFailureFailingHandler : INotificationHandler<PartialFailureNotification>
{
    public ValueTask<Result> HandleAsync(PartialFailureNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Failure(new InvalidOperationException("partial handler failure")));
}

public record MultiFailureNotification : INotification;

public class MultiFailureHandlerA : INotificationHandler<MultiFailureNotification>
{
    public ValueTask<Result> HandleAsync(MultiFailureNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Failure(new InvalidOperationException("failure A")));
}

public class MultiFailureHandlerB : INotificationHandler<MultiFailureNotification>
{
    public ValueTask<Result> HandleAsync(MultiFailureNotification notification, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Failure(new InvalidOperationException("failure B")));
}

public record ThrowingNotification : INotification;

public class ThrowingNotificationHandler : INotificationHandler<ThrowingNotification>
{
    public ValueTask<Result> HandleAsync(ThrowingNotification notification, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("handler threw");
}

public record UnhandledNotification : INotification;
