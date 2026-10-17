using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Response;

/// <summary>
/// Represents a standardized API response for a gateway timeout error (HTTP 504).
/// </summary>
public sealed class GatewayTimeoutResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTimeoutResponse"/> class with a custom response code and message.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="responseMessage">The response message.</param>
    public GatewayTimeoutResponse(int responseCode, string responseMessage)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.GatewayTimeout;
        Result = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTimeoutResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    public GatewayTimeoutResponse(ErrorResult? result = null)
    {
        ResponseCode = BaseResponseCode.GatewayTimeout;
        ResponseMessage = BaseResponseMessages.GatewayTimeout;
        ResponseType = ResponseType.GatewayTimeout;
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTimeoutResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.GatewayTimeout"/>.</param>
    /// <param name="responseMessage">The response message, defaulting to <see cref="BaseResponseMessages.GatewayTimeout"/>.</param>
    public GatewayTimeoutResponse(
        ErrorResult result,
        int? responseCode,
        string responseMessage = "")
    {
        ResponseCode = responseCode ?? BaseResponseCode.GatewayTimeout;
        ResponseMessage = responseMessage ?? BaseResponseMessages.GatewayTimeout;
        ResponseType = ResponseType.GatewayTimeout;
        Result = result;
    }
}
