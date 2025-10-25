using System;
using System.Runtime.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents the base class for custom exceptions in the RA domain.
/// </summary>
[Serializable]
public class RaBaseException : Exception
{
    /// <summary>
    /// The default error message used when no specific message is provided.
    /// </summary>
    private const string DefaultResponseMessage = BaseResponseMessages.Error;

    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a default message and error code.
    /// </summary>
    public RaBaseException() : base(DefaultResponseMessage)
    {
        ErrorCode = BaseResponseCode.InternalServerError;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RaBaseException(string message) : base(message)
    {
        ErrorCode = BaseResponseCode.InternalServerError;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with a specified error code and message.
    /// </summary>
    /// <param name="errorCode">The HTTP status code for the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public RaBaseException(int errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaBaseException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected RaBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ErrorCode = info.GetInt32(nameof(ErrorCode));
    }
}
