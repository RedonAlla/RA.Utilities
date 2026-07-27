using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception for a service unavailable error (HTTP 503).
/// </summary>
public class ServiceUnavailableException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with a default message and error code.
    /// </summary>
    public ServiceUnavailableException()
        : base(BaseResponseCode.ServiceUnavailable, ResponseType.ServiceUnavailable, BaseResponseMessages.ServiceUnavailable)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceUnavailableException(string message)
        : base(BaseResponseCode.ServiceUnavailable, ResponseType.ServiceUnavailable, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public ServiceUnavailableException(int errorCode, string message = BaseResponseMessages.ServiceUnavailable)
        : base(errorCode, ResponseType.ServiceUnavailable, message)
    {
    }
}
