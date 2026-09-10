using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;

namespace RA.Utilities.Feature.Sample.Handlers;

/// <summary>
/// Sample handlers living in a class library separate from the entry assembly.
/// The generated module initializer in this assembly queues their registrations when the
/// assembly is loaded; the entry assembly touches it before calling <c>AddMediator</c>.
/// </summary>
public static class HandlersAssembly
{
    /// <summary>
    /// Forces this assembly to load (running its module initializer) so that its handler
    /// registrations are queued before <c>AddMediator</c> runs.
    /// </summary>
    public static void Touch()
    {
        // Intentionally empty: calling any member of this assembly loads it, which runs the
        // generated module initializer and queues the handler registrations.
    }
}

public record Ping(string Value) : IRequest<Pong>;

public record Pong(string Value);

public class PingHandler : IRequestHandler<Ping, Pong>
{
    public Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken) =>
        Task.FromResult(new Pong($"pong:{request.Value}"));
}

public record OrderPlacedNotification(string OrderId) : INotification;

public class EmailNotifier : INotificationHandler<OrderPlacedNotification>
{
    public Task HandleAsync(OrderPlacedNotification notification, CancellationToken cancellationToken)
    {
        System.Console.WriteLine($"Email sent for order {notification.OrderId}.");
        return Task.CompletedTask;
    }
}
