using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception for a payment required error (HTTP 402).
/// </summary>
public class PaymentRequiredException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRequiredException"/> class with a default message and error code.
    /// </summary>
    public PaymentRequiredException()
        : base(BaseResponseCode.PaymentRequired, ResponseType.PaymentRequired, BaseResponseMessages.PaymentRequired)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRequiredException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PaymentRequiredException(string message)
        : base(BaseResponseCode.PaymentRequired, ResponseType.PaymentRequired, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRequiredException"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">
    /// The message that describes the error. When <see langword="null"/> or omitted,
    /// <see cref="BaseResponseMessages.PaymentRequired"/> is used.
    /// </param>
    public PaymentRequiredException(int errorCode, string? message = null)
        : base(errorCode, ResponseType.PaymentRequired, message ?? BaseResponseMessages.PaymentRequired)
    {
    }
}
