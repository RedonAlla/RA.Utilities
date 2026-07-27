using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception for a rate limit or too many requests error (HTTP 429).
/// </summary>
public class TooManyRequestsException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyRequestsException"/> class with a default message and error code.
    /// </summary>
    public TooManyRequestsException()
        : base(BaseResponseCode.TooManyRequests, ResponseType.TooManyRequests, BaseResponseMessages.TooManyRequests)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyRequestsException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TooManyRequestsException(string message)
        : base(BaseResponseCode.TooManyRequests, ResponseType.TooManyRequests, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyRequestsException"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public TooManyRequestsException(int errorCode, string message = BaseResponseMessages.TooManyRequests)
        : base(errorCode, ResponseType.TooManyRequests, message)
    {
    }
}
