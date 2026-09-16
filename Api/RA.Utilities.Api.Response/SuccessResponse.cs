using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{T}"/> object that produces an <see cref="ResponseType.Success"/> response.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed class SuccessResponse<T> : Response<T>
{
    /// <summary>
    /// Create a new Instance of <see cref="Response{T}"/> with success status.
    /// </summary>
    /// <param name="result">The result data.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.Success"/>.</param>
    /// <param name="responseMessage">The response message.</param>
    public SuccessResponse(
        T result,
        int responseCode = BaseResponseCode.Success,
        string responseMessage = BaseResponseMessages.Success)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Success;
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResponse{T}"/> class with a custom message and default result.
    /// </summary>
    /// <param name="responseMessage">The response message.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.Success"/>.</param>
    public SuccessResponse(
        string responseMessage = BaseResponseMessages.Success,
        int responseCode = BaseResponseCode.Success)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Success;
        Result = default;
    }
}
