using System;

namespace RA.Utilities.Logging.Shared.Exceptions;

/// <summary>
/// Represents an exception thrown when a logger of type <typeparamref name="T"/> is disabled.
/// </summary>
/// <typeparam name="T">The type of the logger that is disabled.</typeparam>
public class LoggerDisabledException<T> : LoggerDisabledException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerDisabledException{T}"/> class.
    /// </summary>
    public LoggerDisabledException()
        : base($"The logger of type '{typeof(T)}' is disabled. Enable logging, or remove the registration for this logger type.")
    {
    }
}

/// <summary>
/// Represents an exception thrown when logging is disabled.
/// </summary>
public class LoggerDisabledException : Exception
{
    private const string DefaultMessage = "Logging is not enabled. Loggers will not write any messages.";

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerDisabledException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoggerDisabledException(string? message = null)
        : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
    {
    }
}
