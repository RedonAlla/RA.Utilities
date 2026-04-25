using System.Text.Json.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Creates an <see cref="Response{BadRequestResult}"/> object that produces an <see cref="ResponseType.BadRequest"/> response.
/// </summary>
public sealed class BadRequestResponse : Response<BadRequestResult[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestResponse"/> class to represent a client-side validation failure.
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
public class BadRequestResult : ErrorResult
{
    /// <summary>
	/// The name of the request property that failed validation.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PropertyName { get; set; }

    /// <summary>
    /// The actual value that was provided in the request and caused the validation failure.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object AttemptedValue { get; set; }

    /// <summary>
    /// The value or format that was expected for this property.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object ExpectedValue { get; set; }
}
