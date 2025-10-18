using System;

namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default response codes used by all microservices.
/// </summary>
public static class BaseResponseCode
{
    /// <summary>
    /// Default response code for success.
    /// </summary>
    public const int Success = 200;

    /// <summary>
    /// Default response code for a bad request.
    /// </summary>
    public const int BadRequest = 400;

    /// <summary>
    /// Default response code for a resource not found.
    /// </summary>
    public const int NotFound = 404;

    /// <summary>
    /// Default response code for an internal server error.
    /// </summary>
    public const int InternalServerError = 500;

    /// <summary>
    /// Default response code for an unauthorized request.
    /// </summary>
    public const int Unauthorized = 401;

    /// <summary>
    /// Default response code for a forbidden request.
    /// </summary>
    public const int Forbidden = 403;

    /// <summary>
    /// Default response code for a conflict request.
    /// </summary>
    public const int Conflict = 409;
}
