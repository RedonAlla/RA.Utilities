using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// Defines a handler for a notification.
/// </summary>
/// <typeparam name="TNotification">The type of notification being handled.</typeparam>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles a notification.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken);

    /// <summary>
    /// Handles a notification with a typed pipeline context.
    /// The default implementation delegates to <see cref="HandleAsync(TNotification, CancellationToken)"/>.
    /// Override this method to consume <see cref="PipelineContext{T}"/> data.
    /// </summary>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    Task HandleAsync<TContext>(TNotification notification, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(notification, cancellationToken);
}
