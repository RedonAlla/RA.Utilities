using System;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents the base class for custom exceptions in the RA domain.
/// </summary>
public class RaBaseException : Exception
{
    /// <summary>
    /// Error type associated with the exception (e.g., "NotFound").
    /// </summary>
    public ResponseType ResponseType { get; init; }

    /// <summary>
    /// Error code associated with the exception.
    /// </summary>
    public int ErrorCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a default message and error code.
    /// </summary>
    public RaBaseException() : base(BaseResponseMessages.Error)
    {
        ErrorCode = BaseResponseCode.InternalServerError;
        ResponseType = ResponseType.Error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a specified error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="errorType">The error type associated with the exception (e.g., "NotFound").</param>
    /// <param name="message">The message that describes the error.</param>
    public RaBaseException(
        int errorCode,
        ResponseType errorType,
        string message = BaseResponseMessages.Error
    ) : base(message)
    {
        ErrorCode = errorCode;
        ResponseType = errorType;
    }
}
