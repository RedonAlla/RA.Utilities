using System;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a user is not authorized to perform an action (HTTP 401).
/// </summary>
public class UnauthorizedException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a default message.
    /// </summary>
    public UnauthorizedException() : base(BaseResponseCode.Unauthorized, BaseResponseMessages.Unauthorized)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedException(string message)
        : base(BaseResponseCode.Unauthorized, message)
    {
    }
}
