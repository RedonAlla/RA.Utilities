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
    private readonly int _maxRetries;
    private readonly int _baseDelayMilliseconds;
    private readonly ILogger<NotificationRetryBehavior<TNotification>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRetryBehavior{TNotification}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="maxRetries">The maximum number of retry attempts. Defaults to 3.</param>
    /// <param name="baseDelayMilliseconds">The base delay in milliseconds between retries. The actual delay is multiplied by the attempt number. Defaults to 200.</param>
    public NotificationRetryBehavior(
        ILogger<NotificationRetryBehavior<TNotification>> logger,
        int maxRetries = 3,
        int baseDelayMilliseconds = 200)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _maxRetries = maxRetries > 0 ? maxRetries : throw new ArgumentOutOfRangeException(nameof(maxRetries), maxRetries, "Max retries must be greater than 0.");
        _baseDelayMilliseconds = baseDelayMilliseconds >= 0 ? baseDelayMilliseconds : throw new ArgumentOutOfRangeException(nameof(baseDelayMilliseconds), baseDelayMilliseconds, "Base delay must be non-negative.");
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
                    _logger.LogWarning(
                        ex,
                        "[Notification Retry] Attempt {Attempt} failed for {NotificationType}. Retrying... Notification: {@Notification}",
                        attempt,
                        typeof(TNotification).Name,
                        notification);
                    await Task.Delay(_baseDelayMilliseconds * attempt, cancellationToken);
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
