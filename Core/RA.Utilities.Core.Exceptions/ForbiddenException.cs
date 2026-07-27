using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when an authenticated user lacks the necessary permissions to perform an action (HTTP 403 Forbidden).
/// </summary>
public class ForbiddenException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a default message and error code.
    /// </summary>
    public ForbiddenException()
        : base(BaseResponseCode.Forbidden, ResponseType.Forbidden, BaseResponseMessages.Forbidden)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a default message and error code.
    /// </summary>
    public ForbiddenException(string message)
        : base(BaseResponseCode.Forbidden, ResponseType.Forbidden, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a custom message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public ForbiddenException(
        int errorCode,
        string message = BaseResponseMessages.Forbidden
    )
        : base(errorCode, ResponseType.Forbidden, message)
    {
    }
}
