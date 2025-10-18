using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Behaviors;

/// <summary>
/// Represents a behavior that logs the handling of notifications.
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public class NotificationLoggingBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationLoggingBehavior<TNotification>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationLoggingBehavior{TNotification}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public NotificationLoggingBehavior(ILogger<NotificationLoggingBehavior<TNotification>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Notification Logging] Handling {NotificationType}", typeof(TNotification).Name);
        await next();
        _logger.LogInformation("[Notification Logging] Finished {NotificationType}", typeof(TNotification).Name);
    }
}
