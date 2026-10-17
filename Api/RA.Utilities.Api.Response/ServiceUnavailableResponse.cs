using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Response;

/// <summary>
/// Represents a standardized API response for a service unavailable error (HTTP 503).
/// </summary>
public sealed class ServiceUnavailableResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableResponse"/> class with a custom response code and message.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="responseMessage">The response message.</param>
    public ServiceUnavailableResponse(int responseCode, string responseMessage)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.ServiceUnavailable;
        Result = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    public ServiceUnavailableResponse(ErrorResult? result = null)
    {
        ResponseCode = BaseResponseCode.ServiceUnavailable;
        ResponseMessage = BaseResponseMessages.ServiceUnavailable;
        ResponseType = ResponseType.ServiceUnavailable;
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.ServiceUnavailable"/>.</param>
    /// <param name="responseMessage">The response message, defaulting to <see cref="BaseResponseMessages.ServiceUnavailable"/>.</param>
    public ServiceUnavailableResponse(
        ErrorResult result,
        int? responseCode,
        string responseMessage = "")
    {
        ResponseCode = responseCode ?? BaseResponseCode.ServiceUnavailable;
        ResponseMessage = responseMessage ?? BaseResponseMessages.ServiceUnavailable;
        ResponseType = ResponseType.ServiceUnavailable;
        Result = result;
    }
}
