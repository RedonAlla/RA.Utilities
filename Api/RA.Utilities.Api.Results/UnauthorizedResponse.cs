using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Represents a standardized API response for an unauthorized request (HTTP 401).
/// </summary>
public class UnauthorizedResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedResponse"/> class with a custom error code and message.
    /// </summary>
    /// <param name="errorCode">The response code.</param>
    /// <param name="errorMessage">The response message.</param>
    public UnauthorizedResponse(int errorCode, string errorMessage)
    {
        ResponseCode = errorCode;
        ResponseMessage = errorMessage;
        ResponseType = ResponseType.Unauthorized;
        Result = null;
    }

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
    public UnauthorizedResponse(
        ErrorResult? result
    )
    {
        ResponseCode = BaseResponseCode.Unauthorized;
        ResponseMessage = BaseResponseMessages.Unauthorized;
        ResponseType = ResponseType.Unauthorized;
        Result = result;
    }
}
