namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default response messages used by all microservices.
/// </summary>
public static class BaseResponseMessages
{
    /// <summary>
    /// Default message for a successful response.
    /// </summary>
    public const string Success = "Operation completed successfully.";

    /// <summary>
    /// Default message for a resource created response.
    /// </summary>
    public const string Created = "Resource created successfully.";

    /// <summary>
    /// Default message for a resource updated response.
    /// </summary>
    public const string Updated = "Resource updated successfully.";

    /// <summary>
    /// Default message for a resource deleted response.
    /// </summary>
    public const string Deleted = "Resource deleted successfully.";

    /// <summary>
    /// Default message for a bad request response.
    /// </summary>
    public const string BadRequest = "The request is invalid.";

    /// <summary>
    /// Default message for a resource not found response.
    /// </summary>
    public const string NotFound = "The requested resource was not found.";

    /// <summary>
    /// Default message for an unauthorized response.
    /// </summary>
    public const string Unauthorized = "Authentication failed or is missing.";

    /// <summary>
    /// Default message for a forbidden response.
    /// </summary>
    public const string Forbidden = "You do not have permission to access this resource.";

    /// <summary>
    /// Default message for a conflict response.
    /// </summary>
    public const string Conflict = "A conflict occurred with the current state of the resource.";

    /// <summary>
    /// Default message for an internal server error response.
    /// </summary>
    public const string InternalServerError = "An unexpected error occurred on the server.";

    /// <summary>
    /// Default message for a general error response.
    /// </summary>
    public const string Error = "Something happened on our end.";
}
