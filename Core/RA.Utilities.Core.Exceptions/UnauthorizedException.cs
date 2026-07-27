using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a user is not authorized to perform an action (HTTP 401).
/// </summary>
public class UnauthorizedException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a default message and error code.
    /// </summary>
    public UnauthorizedException()
        : base(BaseResponseCode.Unauthorized, ResponseType.Unauthorized, BaseResponseMessages.Unauthorized)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a default message and error code.
    /// </summary>
    public UnauthorizedException(string message)
        : base(BaseResponseCode.Unauthorized, ResponseType.Unauthorized, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a custom message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedException(
        int errorCode,
        string message = BaseResponseMessages.Unauthorized
    )
        : base(errorCode, ResponseType.Unauthorized, message)
    {
    }
}
