using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a user is not authorized to perform an action (HTTP 401).
/// </summary>
public class UnauthorizedException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a custom message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="responseCode">The HTTP status code for the error.</param>
    public UnauthorizedException(
        string errorCode = nameof(BaseResponseMessages.Unauthorized),
        string message = BaseResponseMessages.Unauthorized,
        int responseCode = BaseResponseCode.Unauthorized
    )
        : base(errorCode, message, responseCode)
    {
    }
}
