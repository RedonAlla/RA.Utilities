namespace RA.Utilities.Core.Constants;

/// <summary>
/// Represents the semantic type of an API response, used to categorize the outcome.
/// Built-in values cover common HTTP response categories.
/// </summary>
/// <example>
/// // Using a built-in value:
/// throw new RaBaseException(400, ResponseType.Validation, "Invalid input.");
/// </example>
public enum ResponseType
{
    /// <summary>
    /// The operation was successful.
    /// </summary>
    Success,

    /// <summary>
    /// A resource was created. Corresponds to HTTP 201.
    /// </summary>
    Created,

    /// <summary>
    /// A resource was updated. Corresponds to HTTP 200.
    /// </summary>
    Updated,

    /// <summary>
    /// A resource was deleted. Corresponds to HTTP 200.
    /// </summary>
    Deleted,

    /// <summary>
    /// The request succeeded with no content to return.
    /// Corresponds to HTTP 204.
    /// </summary>
    NoContent,

    /// <summary>
    /// The request was accepted for processing but is not yet complete.
    /// Corresponds to HTTP 202.
    /// </summary>
    Accepted,

    /// <summary>
    /// The request failed validation.
    /// Corresponds to HTTP 400.
    /// </summary>
    Validation,

    /// <summary>
    /// An unexpected problem occurred.
    /// Corresponds to HTTP 500.
    /// </summary>
    Problem,

    /// <summary>
    /// The requested resource was not found.
    /// Corresponds to HTTP 404.
    /// </summary>
    NotFound,

    /// <summary>
    /// A conflict with the current state occurred.
    /// Corresponds to HTTP 409.
    /// </summary>
    Conflict,

    /// <summary>
    /// Authentication is required.
    /// Corresponds to HTTP 401.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// A general, unspecified error occurred.
    /// </summary>
    Error,

    /// <summary>
    /// The request was malformed.
    /// Corresponds to HTTP 400.
    /// </summary>
    BadRequest,

    /// <summary>
    /// The request was semantically incorrect.
    /// Corresponds to HTTP 422.
    /// </summary>
    Unprocessable,

    /// <summary>
    /// Insufficient permissions.
    /// Corresponds to HTTP 403.
    /// </summary>
    Forbidden,

    /// <summary>
    /// Too many requests.
    /// Corresponds to HTTP 429.
    /// </summary>
    TooManyRequests,

    /// <summary>
    /// The service is temporarily unavailable.
    /// Corresponds to HTTP 503.
    /// </summary>
    ServiceUnavailable,

    /// <summary>
    /// Gateway timeout. Corresponds to HTTP 504.
    /// </summary>
    GatewayTimeout,
}
