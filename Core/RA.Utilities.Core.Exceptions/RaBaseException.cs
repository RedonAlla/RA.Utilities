using System;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents the base class for custom exceptions in the RA domain.
/// </summary>
public class RaBaseException : Exception
{
    /// <summary>
    /// The default error message used when no specific message is provided.
    /// </summary>
    private const string DefaultResponseMessage = BaseResponseMessages.Error;

    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int ResponseCode { get; set; }

    /// <summary>
    /// Gets or sets the specific error code associated with the exception.
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a default message and error code.
    /// </summary>
    public RaBaseException() : base(DefaultResponseMessage)
    {
        ResponseCode = BaseResponseCode.InternalServerError;
        ErrorCode = nameof(BaseResponseCode.InternalServerError);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a specified error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="responseCode">The HTTP status code for the error.</param>
    public RaBaseException(string errorCode, string message, int responseCode = BaseResponseCode.InternalServerError) : base(message)
    {
        ErrorCode = errorCode;
        ResponseCode = responseCode;
    }
}
