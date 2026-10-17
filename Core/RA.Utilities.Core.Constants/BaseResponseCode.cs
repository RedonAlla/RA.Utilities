namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default response codes used by all microservices.
/// </summary>
public partial class BaseResponseCode
{
    /// <summary>
    /// Represents the HTTP status code 200 (OK).
    /// </summary>
    public static readonly int Success = 200;

    /// <summary>
    /// Represents the HTTP status code 201 (Created).
    /// </summary>
    public static readonly int Created = 201;

    /// <summary>
    /// Represents the HTTP status code 202 (Accepted).
    /// </summary>
    public static readonly int Accepted = 202;

    /// <summary>
    /// Represents the HTTP status code 204 (No Content).
    /// </summary>
    public static readonly int NoContent = 204;

    /// <summary>
    /// Represents the HTTP status code 400 (Bad Request).
    /// </summary>
    public static readonly int BadRequest = 400;

    /// <summary>
    /// Represents the HTTP status code 401 (Unauthorized).
    /// </summary>
    public static readonly int Unauthorized = 401;

    /// <summary>
    /// Represents the HTTP status code 403 (Forbidden).
    /// </summary>
    public static readonly int Forbidden = 403;

    /// <summary>
    /// Represents the HTTP status code 404 (Not Found).
    /// </summary>
    public static readonly int NotFound = 404;

    /// <summary>
    /// Represents the HTTP status code 409 (Conflict).
    /// </summary>
    public static readonly int Conflict = 409;

    /// <summary>
    /// Represents the HTTP status code 422 (Unprocessable Entity).
    /// </summary>
    public static readonly int Unprocessable = 422;

    /// <summary>
    /// Represents the HTTP status code 429 (Too Many Requests).
    /// </summary>
    public static readonly int TooManyRequests = 429;

    /// <summary>
    /// Represents the HTTP status code 500 (Internal Server Error).
    /// </summary>
    public static readonly int InternalServerError = 500;

    /// <summary>
    /// Represents the HTTP status code 503 (Service Unavailable).
    /// </summary>
    public static readonly int ServiceUnavailable = 503;

    /// <summary>
    /// Represents the HTTP status code 504 (Gateway Timeout).
    /// </summary>
    public static readonly int GatewayTimeout = 504;
}
