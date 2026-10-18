using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Behaviors;

/// <summary>
/// Represents a behavior that logs metrics for notification processing.
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public class NotificationMetricsBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationMetricsBehavior<TNotification>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationMetricsBehavior{TNotification}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public NotificationMetricsBehavior(ILogger<NotificationMetricsBehavior<TNotification>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ValueTask<Result> HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
        => MeasuredAsync(next);

    /// <inheritdoc/>
    public ValueTask<Result> HandleAsync<TContext>(TNotification notification, NotificationHandlerContextDelegate<TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => MeasuredAsync(() => next(context));

    private async ValueTask<Result> MeasuredAsync(NotificationHandlerDelegate next)
    {
        _logger.LogDebug("MetricsBehavior..");
        long start = Stopwatch.GetTimestamp();
        Result result = await next().ConfigureAwait(false);
        TimeSpan elapsed = Stopwatch.GetElapsedTime(start);

        if (elapsed.TotalMilliseconds > 500)
            _logger.LogWarning("Long running notification: {NotificationName} ({ElapsedMilliseconds}ms)", typeof(TNotification).Name, elapsed.TotalMilliseconds);

        return result;
    }
}
