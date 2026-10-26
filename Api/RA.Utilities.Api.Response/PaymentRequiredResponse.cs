using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Response;

/// <summary>
/// Represents a standardized API response for a payment required error (HTTP 402).
/// </summary>
public sealed class PaymentRequiredResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRequiredResponse"/> class with a custom response code and message.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="responseMessage">The response message.</param>
    public PaymentRequiredResponse(int responseCode, string responseMessage)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.PaymentRequired;
        Result = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRequiredResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    public PaymentRequiredResponse(ErrorResult? result = null)
    {
        ResponseCode = BaseResponseCode.PaymentRequired;
        ResponseMessage = BaseResponseMessages.PaymentRequired;
        ResponseType = ResponseType.PaymentRequired;
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRequiredResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.PaymentRequired"/>.</param>
    /// <param name="responseMessage">The response message, defaulting to <see cref="BaseResponseMessages.PaymentRequired"/>.</param>
    public PaymentRequiredResponse(
        ErrorResult result,
        int? responseCode,
        string responseMessage = "")
    {
        ResponseCode = responseCode ?? BaseResponseCode.PaymentRequired;
        ResponseMessage = responseMessage ?? BaseResponseMessages.PaymentRequired;
        ResponseType = ResponseType.PaymentRequired;
        Result = result;
    }
}
