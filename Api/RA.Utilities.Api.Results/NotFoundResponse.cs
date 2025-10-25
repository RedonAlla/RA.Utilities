using System;
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
        string? responseMessage = null
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage ?? $"{model.EntityName} with value '{model.EntityValue}' not found.";
        ResponseType = ResponseType.NotFound;
        Result = model;
    }
}

/// <summary>
/// Represent an entity not found searched by given value.
/// </summary>
/// <param name="name"></param>
/// <param name="value"></param>
public class NotFoundResult(string name, object value)
{
    /// <summary>
    /// The type of resource that was being looked for (e.g., "Product", "User")
    /// </summary>
    public string EntityName { get; set; } = name;

    /// <summary>
    /// The identifier that was used in the search (e.g., `123`, `"john.doe@example.com"`).
    /// </summary>
    public object EntityValue { get; set; } = value;
}
