using System.Text.Json.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{BadRequestResult}"/> object that produces an <see cref="ResponseType.BadRequest"/> response.
/// </summary>
public sealed class BadRequestResponse : Response<BadRequestResult[]>
{
    /// <summary>
    /// Create a new Instance of <see cref="Response{BadRequestResult}"/> with success status.
    /// </summary>
    /// <param name="errors">Errors type of <see cref="BadRequestResponse"/></param>
    /// <param name="responseCode">Response code.</param>
    /// <param name="responseMessage">Response message.</param>
    public BadRequestResponse(
        BadRequestResult[] errors,
        int responseCode = BaseResponseCode.BadRequest,
        string responseMessage = BaseResponseMessages.BadRequest
    )
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        ResponseType = ResponseType.BadRequest;
        Result = errors;
    }
}

/// <summary>
/// Error results for bad response.
/// </summary>
public class BadRequestResult
{
    /// <summary>
	/// The name of the property.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PropertyName { get; set; }

    /// <summary>
    /// The error message
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// The property value that caused the failure.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object AttemptedValue { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the expected value.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object ExpectedValue { get; set; }
}
