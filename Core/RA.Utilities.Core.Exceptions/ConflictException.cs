using System;
using System.Runtime.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a request conflicts with the current state of the target resource (HTTP 409).
/// </summary>
[Serializable]
public class ConflictException : RaBaseException
{
    /// <summary>
    /// Gets the name of the entity that caused the conflict (e.g., "User").
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Gets the value or identifier of the entity that caused the conflict.
    /// </summary>
    public object EntityValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class for a resource that already exists.
    /// </summary>
    /// <param name="entityName">The name of the entity type that caused the conflict.</param>
    /// <param name="entityValue">The value or identifier of the conflicting entity.</param>
    /// <param name="code">The HTTP status code. Defaults to 409 (Conflict).</param>
    public ConflictException(string entityName, object entityValue, int code = BaseResponseCode.Conflict)
        : base(code, $"{entityName} with value '{entityValue}' already exists.")
    {
        EntityName = entityName;
        EntityValue = entityValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="entityName">The name of the entity type that caused the conflict.</param>
    /// <param name="entityValue">The value or identifier of the conflicting entity.</param>
    public ConflictException(string message, string entityName, object entityValue)
        : base(BaseResponseCode.Conflict, message)
    {
        EntityName = entityName;
        EntityValue = entityValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        EntityName = info.GetString(nameof(EntityName))!;
        EntityValue = info.GetValue(nameof(EntityValue), typeof(object))!;
    }
}
