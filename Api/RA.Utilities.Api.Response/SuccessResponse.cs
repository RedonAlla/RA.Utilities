using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Response;

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
    public SuccessResponse(T result)
    {
        ResponseCode = BaseResponseCode.Success;
        ResponseMessage = BaseResponseMessages.Success;
        ResponseType = ResponseType.Success;
        Result = result;
    }

    /// <summary>
    /// Create a new Instance of <see cref="Response{T}"/> with success status.
    /// </summary>
    /// <param name="result">The result data.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.Success"/>.</param>
    /// <param name="responseMessage">The response message.</param>
    public SuccessResponse(
        T result,
        int? responseCode,
        string responseMessage = "")
    {
        ResponseCode = responseCode ?? BaseResponseCode.Success;
        ResponseMessage = responseMessage ?? BaseResponseMessages.Success;
        ResponseType = ResponseType.Success;
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResponse{T}"/> class with a custom message and default result.
    /// </summary>
    public SuccessResponse()
    {
        ResponseCode = BaseResponseCode.Success;
        ResponseMessage = BaseResponseMessages.Success;
        ResponseType = ResponseType.Success;
        Result = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResponse{T}"/> class with a custom message and default result.
    /// </summary>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.Success"/>.</param>
    public SuccessResponse(int responseCode)
    {
        ResponseCode = responseCode;
        ResponseMessage = BaseResponseMessages.Success;
        ResponseType = ResponseType.Success;
        Result = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResponse{T}"/> class with a custom message and default result.
    /// </summary>
    /// <param name="responseMessage">The response message.</param>
    public SuccessResponse(string responseMessage)
    {
        ResponseCode = BaseResponseCode.Success;
        ResponseMessage = responseMessage ?? BaseResponseMessages.Success;
        ResponseType = ResponseType.Success;
        Result = default;
    }
}
