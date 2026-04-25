using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Represents a standardized API response for an forbidden request (HTTP 403).
/// </summary>
public class ForbiddenResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code.</param>
    /// <param name="responseMessage">The response message.</param>
    public ForbiddenResponse(
        ErrorResult? result,
        int responseCode = BaseResponseCode.Forbidden,
        string responseMessage = BaseResponseMessages.Forbidden)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Forbidden;
        Result = result;
    }
}
