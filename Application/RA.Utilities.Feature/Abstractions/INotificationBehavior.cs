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
    Task HandleAsync(
        TNotification notification,
        NotificationHandlerDelegate next,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Handles the notification with a typed pipeline context and invokes the next context-aware delegate.
    /// The default implementation adapts to the non-context method.
    /// Override this method to read or write <see cref="PipelineContext{T}"/> data.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="context"> Extra/arbitrary data passing through the pipeline execution.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    Task HandleAsync<TContext>(
        TNotification notification,
        NotificationHandlerContextDelegate<TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken
    ) where TContext : class, new()
        => HandleAsync(notification, () => next(context), cancellationToken);
}
