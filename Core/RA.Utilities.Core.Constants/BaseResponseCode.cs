namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default response codes used by all microservices.
/// </summary>
public static class BaseResponseCode
{
    /// <summary>
    /// Represents the HTTP status code 200 (OK).
    /// </summary>
    public const int Success = 200;

    /// <summary>
    /// Represents the HTTP status code 201 (Created).
    /// </summary>
    public const int Created = 201;

    /// <summary>
    /// Represents the HTTP status code 202 (Accepted).
    /// </summary>
    public const int Accepted = 202;

    /// <summary>
    /// Represents the HTTP status code 204 (No Content).
    /// </summary>
    public const int NoContent = 204;

    /// <summary>
    /// Represents the HTTP status code 400 (Bad Request).
    /// </summary>
    public const int BadRequest = 400;

    /// <summary>
    /// Represents the HTTP status code 401 (Unauthorized).
    /// </summary>
    public const int Unauthorized = 401;

    /// <summary>
    /// Represents the HTTP status code 403 (Forbidden).
    /// </summary>
    public const int Forbidden = 403;

    /// <summary>
    /// Represents the HTTP status code 404 (Not Found).
    /// </summary>
    public const int NotFound = 404;

    /// <summary>
    /// Represents the HTTP status code 409 (Conflict).
    /// </summary>
    public const int Conflict = 409;

    /// <summary>
    /// Represents the HTTP status code 422 (Unprocessable Entity).
    /// </summary>
    public const int Unprocessable = 422;

    /// <summary>
    /// Represents the HTTP status code 429 (Too Many Requests).
    /// </summary>
    public const int TooManyRequests = 429;

    /// <summary>
    /// Represents the HTTP status code 500 (Internal Server Error).
    /// </summary>
    public const int InternalServerError = 500;

    /// <summary>
    /// Represents the HTTP status code 503 (Service Unavailable).
    /// </summary>
    public const int ServiceUnavailable = 503;

    /// <summary>
    /// Represents the HTTP status code 504 (Gateway Timeout).
    /// </summary>
    public const int GatewayTimeout = 504;
}
