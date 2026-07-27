using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a request conflicts with the current state of the target resource (HTTP 409).
/// </summary>
public class ConflictException : RaBaseException
{
    /// <summary>
    /// Gets the name of the entity that caused the conflict (e.g., "User").
    /// </summary>
    public string EntityName { get; init; }

    /// <summary>
    /// Gets the value or identifier of the entity that caused the conflict.
    /// </summary>
    public object EntityValue { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class for a generic conflict.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public ConflictException(
        int errorCode = BaseResponseCode.Conflict,
        string message = BaseResponseMessages.Conflict
    )
        : base(errorCode, ResponseType.Conflict, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class for a resource that already exists.
    /// </summary>
    /// <param name="entity">The name of the entity type that caused the conflict.</param>
    /// <param name="value">The value or identifier of the conflicting entity.</param>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public ConflictException(
        string entity,
        object value,
        int errorCode = BaseResponseCode.Conflict,
        string? message = null
    )
        : base(errorCode, ResponseType.Conflict, message ?? $"{entity} with value '{value}' already exists.")
    {
        EntityName = entity;
        EntityValue = value;
    }
}
