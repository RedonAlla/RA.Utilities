namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default response messages used by all microservices.
/// </summary>
public abstract partial class BaseResponseMessages
{
    /// <summary>
    /// Default message for a successful response.
    /// </summary>
    public static readonly string Success = "Operation completed successfully.";

    /// <summary>
    /// Default message for a resource created response.
    /// </summary>
    public static readonly string Created = "Resource created successfully.";

    /// <summary>
    /// Default message for a resource updated response.
    /// </summary>
    public static readonly string Updated = "Resource updated successfully.";

    /// <summary>
    /// Default message for a resource deleted response.
    /// </summary>
    public static readonly string Deleted = "Resource deleted successfully.";

    /// <summary>
    /// Default message for an accepted response (HTTP 202).
    /// </summary>
    public static readonly string Accepted = "The request has been accepted for processing.";

    /// <summary>
    /// Default message for a no content response (HTTP 204).
    /// </summary>
    public static readonly string NoContent = "No content.";

    /// <summary>
    /// Default message for a bad request response.
    /// </summary>
    public static readonly string BadRequest = "The request is invalid.";

    /// <summary>
    /// Default message for a resource not found response.
    /// </summary>
    public static readonly string NotFound = "The requested resource was not found.";

    /// <summary>
    /// Default message for an unauthorized response.
    /// </summary>
    public static readonly string Unauthorized = "Authentication failed or is missing.";

    /// <summary>
    /// Default message for a forbidden response.
    /// </summary>
    public static readonly string Forbidden = "You do not have permission to access this resource.";

    /// <summary>
    /// Default message for a conflict response.
    /// </summary>
    public static readonly string Conflict = "A conflict occurred with the current state of the resource.";

    /// <summary>
    /// Default message for an internal server error response.
    /// </summary>
    public static readonly string InternalServerError = "An unexpected error occurred on the server.";

    /// <summary>
    /// Default message for a generic, unspecified error.
    /// </summary>
    public static readonly string Error = "Something happened on our end.";

    /// <summary>
    /// Default message for an unprocessable entity response (HTTP 422).
    /// </summary>
    public static readonly string Unprocessable = "Unprocessable entity.";

    /// <summary>
    /// Default message for a gateway timeout response (HTTP 504).
    /// </summary>
    public static readonly string GatewayTimeout = "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server.";

    /// <summary>
    /// Default message for a too many requests response (HTTP 429).
    /// </summary>
    public static readonly string TooManyRequests = "Too many requests. Please try again later.";

    /// <summary>
    /// Default message for a service unavailable response (HTTP 503).
    /// </summary>
    public static readonly string ServiceUnavailable = "The service is temporarily unavailable. Please try again later.";
}
