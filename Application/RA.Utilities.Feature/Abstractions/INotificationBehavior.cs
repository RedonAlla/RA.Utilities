using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// Represents a behavior that can be applied to a notification.
/// </summary>
public interface INotificationBehavior<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the notification.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken);
}
