using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{NotFoundResult}"/> object that produces an <see cref="ResponseType.NotFound"/> response.
/// </summary>
public sealed class NotFoundResponse : Response<NotFoundResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundResponse"/> class.
    /// </summary>
    /// <param name="model">The not found result data.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.NotFound"/>.</param>
    /// <param name="responseMessage">The response message. If null, a default message is generated.</param>
    public NotFoundResponse(
        NotFoundResult model,
        int responseCode = BaseResponseCode.NotFound,
        string responseMessage = BaseResponseMessages.NotFound
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.NotFound;
        Result = model;
    }
}

/// <summary>
/// Represent an entity not found searched by given value.
/// </summary>
public class NotFoundResult : ErrorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundResult"/> class.
    /// </summary>
    /// <param name="entity">The type of resource that was being looked for.</param>
    /// <param name="value">The identifier that was used in the search.</param>
    /// <param name="errorCode">The specific error code.</param>
    /// <param name="message">The error message.</param>
    public NotFoundResult(
        string entity,
        object value,
        string errorCode = nameof(BaseResponseCode.NotFound),
        string message = BaseResponseMessages.NotFound
    )
    {
        Entity = entity;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = message;
    }

    /// <summary>
    /// The type of resource that was being looked for (e.g., "Product", "User")
    /// </summary>
    public string Entity { get; set; }

    /// <summary>
    /// The identifier that was used in the search (e.g., `123`, `"john.doe@example.com"`).
    /// </summary>
    public object Value { get; set; }
}
