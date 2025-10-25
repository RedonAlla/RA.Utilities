using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{T}"/> object that produces an <see cref="ResponseType.Error"/> response, typically for unhandled server errors.
/// </summary>
public sealed class ErrorResponse : Response<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
    /// </summary>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.InternalServerError"/>.</param>
    /// <param name="responseMessage">The response message, defaulting to a generic server error message.</param>
    public ErrorResponse(
        int responseCode = BaseResponseCode.InternalServerError,
        string responseMessage = BaseResponseMessages.Error
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Error;
        Result = null;
    }
}
