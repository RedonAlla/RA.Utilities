using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when an operation cannot be performed due to the current state of the resource (HTTP 409).
/// </summary>
public class UnprocessableException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnprocessableException"/> class with a custom message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="responseCode">The HTTP status code for the error.</param>
    public UnprocessableException(
        string errorCode = nameof(BaseResponseCode.Unprocessable),
        string message = BaseResponseMessages.Unprocessable,
        int responseCode = BaseResponseCode.Unprocessable)
        : base(errorCode, message, responseCode)
    {
    }
}
