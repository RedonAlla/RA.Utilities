using System;

namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default response messages used by all microservices.
/// </summary>
public static class BaseResponseMessages
{
    /// <summary>
    /// Default message for a successful response.
    /// </summary>
    public const string Success = "OK";

    /// <summary>
    /// Default message for a bad request response.
    /// </summary>
    public const string BadRequest = "One or more validation errors occurred.";

    /// <summary>
    /// Default message for a resource not found response.
    /// </summary>
    public const string NotFound = "Resource not found.";

    /// <summary>
    /// Default message for a general error response.
    /// </summary>
    public const string Error = "Something happened on our end.";

    /// <summary>
    /// Default message for an unauthorized response.
    /// </summary>
    public const string Unauthorized = "You are not authorized to perform this action.";
}
