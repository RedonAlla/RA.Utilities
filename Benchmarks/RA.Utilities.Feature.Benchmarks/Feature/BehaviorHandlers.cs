using System;
using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;

namespace RA.Utilities.Feature.Benchmarks.Feature;

// No-op notification handlers for the notification-behavior scenarios. They are non-generic
// classes, so the source generator auto-registers them — only the behaviors are registered
// explicitly, exactly as an application would wire them. The request scenarios reuse the shared
// generic ping handlers (registered explicitly per scenario, since generic handlers are not
// auto-registered).

/// <summary>
/// No-op handler for <see cref="LoggedNotification"/>.
/// </summary>
internal sealed class LoggedNotificationHandler : INotificationHandler<LoggedNotification>
{
    public Task HandleAsync(LoggedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// No-op handler for <see cref="MeasuredNotification"/>.
/// </summary>
internal sealed class MeasuredNotificationHandler : INotificationHandler<MeasuredNotification>
{
    public Task HandleAsync(MeasuredNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// Handler for <see cref="RetryNotification"/> that throws on its first invocation per instance
/// and succeeds afterwards, exercising the retry behavior's failure-and-recovery path. Each
/// benchmark iteration resolves a fresh transient handler from a fresh scope, so every publish
/// fails exactly once before the retry behavior recovers it.
/// </summary>
internal sealed class FlakyRetryNotificationHandler : INotificationHandler<RetryNotification>
{
    private int _attempts;

    public Task HandleAsync(RetryNotification notification, CancellationToken cancellationToken)
    {
        if (_attempts++ == 0)
        {
            throw new InvalidOperationException("Simulated transient failure.");
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// No-op handler for <see cref="RetryImmediateNotification"/>.
/// </summary>
internal sealed class RetryImmediateNotificationHandler : INotificationHandler<RetryImmediateNotification>
{
    public Task HandleAsync(RetryImmediateNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

/// <summary>
/// No-op handler for <see cref="StackedNotification"/>.
/// </summary>
internal sealed class StackedNotificationHandler : INotificationHandler<StackedNotification>
{
    public Task HandleAsync(StackedNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
