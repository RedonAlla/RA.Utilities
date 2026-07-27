using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception for a gateway timeout error (HTTP 504).
/// </summary>
public class GatewayTimeoutException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTimeoutException"/> class with a default message and error code.
    /// </summary>
    public GatewayTimeoutException()
        : base(BaseResponseCode.GatewayTimeout, ResponseType.GatewayTimeout, BaseResponseMessages.GatewayTimeout)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTimeoutException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GatewayTimeoutException(string message)
        : base(BaseResponseCode.GatewayTimeout, ResponseType.GatewayTimeout, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTimeoutException"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public GatewayTimeoutException(int errorCode, string message = BaseResponseMessages.GatewayTimeout)
        : base(errorCode, ResponseType.GatewayTimeout, message)
    {
    }
}
