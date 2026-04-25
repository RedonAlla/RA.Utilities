using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{ErrorResult}"/> object that produces an <see cref="ResponseType.Unprocessable"/> response.
/// </summary>
public sealed class UnprocessableResponse : Response<ErrorResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnprocessableResponse"/> class.
    /// </summary>
    /// <param name="result">The error result details.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.Unprocessable"/>.</param>
    /// <param name="responseMessage">The response message, defaulting to a generic unprocessable message.</param>
    public UnprocessableResponse(
        ErrorResult? result,
        int responseCode = BaseResponseCode.Unprocessable,
        string responseMessage = BaseResponseMessages.Unprocessable
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Unprocessable;
        Result = result ?? new ErrorResult
        {
            ErrorCode = nameof(BaseResponseCode.Unprocessable),
            ErrorMessage = BaseResponseMessages.Unprocessable
        };
    }
}
