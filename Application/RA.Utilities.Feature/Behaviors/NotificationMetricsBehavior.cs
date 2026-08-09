using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Behaviors;

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
    public async Task HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        _logger.LogDebug("MetricsBehavior..");
        var timer = Stopwatch.StartNew();
        await next();
        timer.Stop();

        if (timer.ElapsedMilliseconds > 500)
            _logger.LogWarning("Long running notification: {NotificationName} ({ElapsedMilliseconds}ms)", typeof(TNotification).Name, timer.ElapsedMilliseconds);
    }

    /// <inheritdoc/>
    public async Task HandleAsync<TContext>(TNotification notification, NotificationHandlerContextDelegate<TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        _logger.LogDebug("MetricsBehavior..");
        var timer = Stopwatch.StartNew();
        await next(context);
        timer.Stop();

        if (timer.ElapsedMilliseconds > 500)
            _logger.LogWarning("Long running notification: {NotificationName} ({ElapsedMilliseconds}ms)", typeof(TNotification).Name, timer.ElapsedMilliseconds);
    }
}
