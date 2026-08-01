using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Represents a standardized API response for a rate limit or too many requests error (HTTP 429).
/// </summary>
public sealed class TooManyRequestsResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyRequestsResponse"/> class with a custom response code and message.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="responseMessage">The response message.</param>
    public TooManyRequestsResponse(int responseCode, string responseMessage)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.TooManyRequests;
        Result = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyRequestsResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.TooManyRequests"/>.</param>
    /// <param name="responseMessage">The response message, defaulting to <see cref="BaseResponseMessages.TooManyRequests"/>.</param>
    public TooManyRequestsResponse(
        ErrorResult? result = null,
        int responseCode = BaseResponseCode.TooManyRequests,
        string responseMessage = BaseResponseMessages.TooManyRequests)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.TooManyRequests;
        Result = result;
    }
}
