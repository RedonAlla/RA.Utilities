using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Behaviors;

/// <summary>
/// Represents a behavior that retries notification handling in case of failures.
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public class NotificationRetryBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    private readonly int _maxRetries = 3;
    private readonly ILogger<NotificationRetryBehavior<TNotification>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRetryBehavior{TNotification}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public NotificationRetryBehavior(ILogger<NotificationRetryBehavior<TNotification>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                attempt++;
                await next();
                break;
            }
            catch (Exception ex)
            {
                if (attempt < _maxRetries)
                {
                    _logger.LogWarning(ex, "[Notification Retry] Attempt {Attempt} failed for {NotificationType}. Retrying... Notification: {@Notification}", 
                        attempt, typeof(TNotification).Name, notification);
                    await Task.Delay(200 * attempt, cancellationToken);
                }
                else
                {
                    _logger.LogError(ex, "[Notification Retry] All {MaxRetries} attempts failed for {NotificationType}. Notification: {@Notification}", 
                        _maxRetries, typeof(TNotification).Name, notification);
                    throw; // Re-throw the last exception after logging
                }
            }
        }
    }
}
