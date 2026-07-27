using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception for a request that is syntactically correct but semantically cannot be processed (HTTP 422).
/// </summary>
public class UnprocessableException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnprocessableException"/> class with a default message and error code.
    /// </summary>
    public UnprocessableException()
        : base(BaseResponseCode.Unprocessable, ResponseType.Unprocessable, BaseResponseMessages.Unprocessable)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnprocessableException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnprocessableException(string message)
        : base(BaseResponseCode.Unprocessable, ResponseType.Unprocessable, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnprocessableException"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public UnprocessableException(
        int errorCode,
        string message = BaseResponseMessages.Unprocessable
    )
        : base(errorCode, ResponseType.Unprocessable, message)
    {
    }
}
