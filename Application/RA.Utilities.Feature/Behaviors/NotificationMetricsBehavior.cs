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
    private readonly Stopwatch _timer;
    private readonly ILogger<NotificationMetricsBehavior<TNotification>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationMetricsBehavior{TNotification}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public NotificationMetricsBehavior(ILogger<NotificationMetricsBehavior<TNotification>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        _logger.LogDebug("MetricsBehavior..");

        _timer.Start();
        await next();
        _timer.Stop();

        long elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning("Long running request: {RequestName} ({ElapsedMilliseconds}ms)",
                typeof(TNotification).Name, elapsedMilliseconds);
        }
    }
}
