using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{T}"/> object that
/// </summary>
/// <typeparam name="T"></typeparam> produces an <see cref="ResponseType.Success"/> response.
public sealed class SuccessResponse<T> : Response<T> where T : new()
{
    /// <summary>
    /// Create a new Instance of <see cref="Response{T}"/> with success status.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="responseMessage"></param>
    public SuccessResponse(T result, string responseMessage = BaseResponseMessages.Success)
    {
        ResponseCode = BaseResponseCode.Success;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Success;
        Result = result;
    }
}
