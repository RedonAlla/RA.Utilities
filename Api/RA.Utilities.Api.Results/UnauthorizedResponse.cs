using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Represents a standardized API response for an unauthorized request (HTTP 401).
/// </summary>
public class UnauthorizedResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedResponse"/> class.
    /// </summary>
    public UnauthorizedResponse()
    {
        ResponseCode = BaseResponseCode.Unauthorized;
        ResponseMessage = BaseResponseMessages.Unauthorized;
        ResponseType = ResponseType.Unauthorized;
        Result = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedResponse"/> class with custom response code and message.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code.</param>
    /// <param name="responseMessage">The response message.</param>
    public UnauthorizedResponse(
        ErrorResult? result,
        int responseCode = BaseResponseCode.Unauthorized,
        string responseMessage = BaseResponseMessages.Unauthorized
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Unauthorized;
        Result = result;
    }
}
