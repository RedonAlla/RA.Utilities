using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a requested resource could not be found (HTTP 404).
/// </summary>
public class NotFoundException : RaBaseException
{
    /// <summary>
    /// Gets the name of the entity that was not found (e.g., "User").
    /// </summary>
    public string EntityName { get; init; }

    /// <summary>
    /// Gets the value or identifier of the entity that was not found.
    /// </summary>
    public object EntityValue { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a generic not-found error.
    /// </summary>
    public NotFoundException()
        : base(BaseResponseCode.NotFound, ResponseType.NotFound, BaseResponseMessages.NotFound)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a generic not-found error.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">
    /// The message that describes the error. When <see langword="null"/> or omitted,
    /// <see cref="BaseResponseMessages.NotFound"/> is used.
    /// </param>
    public NotFoundException(int errorCode, string? message = null)
        : base(errorCode, ResponseType.NotFound, message ?? BaseResponseMessages.NotFound)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a resource that was not found.
    /// </summary>
    /// <param name="entity">The name of the entity type that was not found.</param>
    /// <param name="value">The value or identifier used to search for the entity.</param>
    public NotFoundException(string entity, object value)
        : base(
            BaseResponseCode.NotFound,
            ResponseType.NotFound,
            $"{entity} with value '{value}' was not found."
        )
    {
        EntityName = entity;
        EntityValue = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a resource that was not found,
    /// using a custom message.
    /// </summary>
    /// <param name="entity">The name of the entity type that was not found.</param>
    /// <param name="value">The value or identifier used to search for the entity.</param>
    /// <param name="message">
    /// The message that describes the error. When <see langword="null"/>, the default
    /// "&lt;entity&gt; with value '&lt;value&gt;' was not found." message is used.
    /// </param>
    public NotFoundException(string entity, object value, string? message)
        : base(
            BaseResponseCode.NotFound,
            ResponseType.NotFound,
            message ?? $"{entity} with value '{value}' was not found."
        )
    {
        EntityName = entity;
        EntityValue = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a resource that was not found.
    /// </summary>
    /// <param name="entity">The name of the entity type that was not found.</param>
    /// <param name="value">The value or identifier used to search for the entity.</param>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">
    /// The message that describes the error. When <see langword="null"/> or omitted,
    /// "&lt;entity&gt; with value '&lt;value&gt;' was not found." is used.
    /// </param>
    public NotFoundException(
        string entity,
        object value,
        int errorCode,
        string? message = null
    )
        : base(
            errorCode,
            ResponseType.NotFound,
            message ?? $"{entity} with value '{value}' was not found."
        )
    {
        EntityName = entity;
        EntityValue = value;
    }
}
