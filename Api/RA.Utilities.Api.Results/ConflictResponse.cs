using System.Text.Json.Serialization;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{ConflictResult}"/> object that produces an <see cref="ResponseType.Conflict"/> response.
/// </summary>
public sealed class ConflictResponse : Response<ConflictResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictResponse"/> class.
    /// </summary>
    /// <param name="model">The conflict result data.</param>
    /// <param name="responseCode">The response code, defaulting to <see cref="BaseResponseCode.Conflict"/>.</param>
    /// <param name="responseMessage">The response message. If null, a default message is generated.</param>
    public ConflictResponse(
        ConflictResult model,
        int responseCode = BaseResponseCode.Conflict,
        string responseMessage = BaseResponseMessages.Conflict
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.Conflict;
        Result = model;
    }
}

/// <summary>
/// Represents the result of a conflict, typically indicating that an entity with the given name and value already exists.
/// </summary>
public class ConflictResult : ErrorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictResult"/> class.
    /// </summary>
    /// <param name="entity">The type of resource causing the conflict.</param>
    /// <param name="value">The specific value that caused the conflict.</param>
    /// <param name="errorCode">The machine-readable error code associated with this specific error.</param>
    /// <param name="errorMessage">The human-readable description of the error.</param>
    public ConflictResult(
        string entity,
        object value,
        string? errorCode = null,
        string errorMessage = BaseResponseMessages.Conflict
    )
    {
        Entity = entity;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// The type of resource causing the conflict (e.g., "User").
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Entity { get; set; }

    /// <summary>
    /// The specific value that caused the conflict (e.g., "test@example.com").
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Value { get; set; }
}
