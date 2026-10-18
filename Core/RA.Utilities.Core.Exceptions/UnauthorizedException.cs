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
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedException(string message)
        : base(BaseResponseCode.Unauthorized, ResponseType.Unauthorized, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">
    /// The message that describes the error. When <see langword="null"/> or omitted,
    /// <see cref="BaseResponseMessages.Unauthorized"/> is used.
    /// </param>
    public UnauthorizedException(
        int errorCode,
        string? message = null
    )
        : base(errorCode, ResponseType.Unauthorized, message ?? BaseResponseMessages.Unauthorized)
    {
    }
}
