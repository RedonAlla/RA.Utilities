using System;
using RA.Utilities.Core.Constants;

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
        string? responseMessage = null
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage ?? $"{model.Entity} with value '{model.Value}' already exists.";
        ResponseType = ResponseType.Conflict;
        Result = model;
    }
}

/// <summary>
/// Represents the result of a conflict, typically indicating that an entity with the given name and value already exists.
/// </summary>
/// <param name="name">The name of the entity.</param>
/// <param name="value">The value of the entity.</param>
public class ConflictResult(string name, object value)
{
    /// <summary>
    /// Entity name.
    /// </summary>
    public string Entity { get; set; } = name;

    /// <summary>
    /// Entity value.
    /// </summary>
    public object Value { get; set; } = value;
}
