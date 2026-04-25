using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when an authenticated user lacks the necessary permissions to perform an action (HTTP 403 Forbidden).
/// </summary>
public class ForbiddenException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a custom message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="responseCode">The HTTP status code for the error.</param>
    public ForbiddenException(
        string errorCode = nameof(BaseResponseCode.Forbidden),
        string message = BaseResponseMessages.Forbidden,
        int responseCode = BaseResponseCode.Forbidden
    )
        : base(errorCode, message, responseCode)
    {
    }
}
