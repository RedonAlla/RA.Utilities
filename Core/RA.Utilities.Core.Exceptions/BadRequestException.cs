using System.Text.Json.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown for a bad request, typically due to invalid client-side input (HTTP 400).
/// </summary>
public class BadRequestException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a collection of validation errors.
    /// </summary>
    /// <param name="errors">An array of validation errors that caused the exception.</param>
    /// <param name="code">The error code. Defaults to 400 (Bad Request).</param>
    /// <param name="message">The error message. Defaults to a standard bad request message.</param>
    public BadRequestException(
        ValidationErrors[] errors,
        int code = BaseResponseCode.BadRequest,
        string message = BaseResponseMessages.BadRequest
    ) : base(code, message)
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets or sets the list of validation errors.
    /// </summary>
    public ValidationErrors[] Errors { get; }
}

/// <summary>
/// Represents a single validation error, providing detailed context about a failure.
/// </summary>
public class ValidationErrors
{
    /// <summary>
	/// Gets the name of the property that failed validation.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PropertyName { get; init; }

    /// <summary>
    /// Gets the message describing the validation error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; init; }

    /// <summary>
    /// Gets the value that was provided for the property, which caused the validation to fail.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object AttemptedValue { get; init; }

    /// <summary>
    /// Gets a custom error code associated with the validation failure.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorCode { get; init; }

    /// <summary>
    /// Gets or sets the expected value.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object ExpectedValue { get; set; }
}
