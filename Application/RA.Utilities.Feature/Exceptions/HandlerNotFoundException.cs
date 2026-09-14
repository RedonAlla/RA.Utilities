using System;

namespace RA.Utilities.Feature.Exceptions;

/// <summary>
/// The exception that is thrown when an object-based <c>Send</c> or <c>Publish</c> call
/// cannot be dispatched because the runtime type of the message has no handler contract.
/// </summary>
public class HandlerNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerNotFoundException"/> class.
    /// </summary>
    /// <param name="messageType">The runtime type of the message that could not be dispatched.</param>
    public HandlerNotFoundException(Type messageType)
        : base(BuildMessage(messageType, null))
    {
        MessageType = messageType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerNotFoundException"/> class
    /// with a reason describing why the message could not be dispatched.
    /// </summary>
    /// <param name="messageType">The runtime type of the message that could not be dispatched.</param>
    /// <param name="reason">The reason the message could not be dispatched.</param>
    public HandlerNotFoundException(Type messageType, string reason)
        : base(BuildMessage(messageType, reason))
    {
        MessageType = messageType;
    }

    /// <summary>
    /// Gets the runtime type of the message that could not be dispatched.
    /// </summary>
    public Type MessageType { get; }

    private static string BuildMessage(Type messageType, string? reason)
    {
        ArgumentNullException.ThrowIfNull(messageType);

        string message =
            $"Handler for message of type '{messageType.Name}' was not found. " +
            "Make sure the message implements IRequest<TResponse>, IRequest, or INotification, " +
            "and that a matching handler is registered in the DI container.";

        return reason is null ? message : message + $" {reason}";
    }
}
