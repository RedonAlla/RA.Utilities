using System.Text.Json.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown for a bad request, typically due to invalid client-side input (HTTP 400).
/// </summary>
public class BadRequestException : RaBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a single validation error.
    /// </summary>
    /// <param name="error">The validation error that caused the exception.</param>
    public BadRequestException(
        ValidationError error
    ) : base(BaseResponseCode.BadRequest, ResponseType.BadRequest, BaseResponseMessages.BadRequest)
    {
        Errors = [error];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a single validation error.
    /// </summary>
    /// <param name="error">The validation error that caused the exception.</param>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The error message.</param>
    public BadRequestException(
        ValidationError error,
        int errorCode,
        string message = BaseResponseMessages.BadRequest
    ) : base(errorCode, ResponseType.BadRequest, message)
    {
        Errors = [error];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a collection of validation errors.
    /// </summary>
    /// <param name="errors">An array of validation errors that caused the exception.</param>
    public BadRequestException(
        ValidationError[] errors
    ) : base(BaseResponseCode.BadRequest, ResponseType.BadRequest, BaseResponseMessages.BadRequest)
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a collection of validation errors.
    /// </summary>
    /// <param name="errors">An array of validation errors that caused the exception.</param>
    /// <param name="errorCode">A specific error code associated with the error.</param>
    /// <param name="message">The error message.</param>
    public BadRequestException(
        ValidationError[] errors,
        int errorCode,
        string message = BaseResponseMessages.BadRequest
    ) : base(errorCode, ResponseType.BadRequest, message)
    {
        Errors = errors;
    }

    /// <summary>
    /// The list of validation errors.
    /// </summary>
    public ValidationError[] Errors { get; }
}

/// <summary>
/// Represents a single validation error, providing detailed context about a failure.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="errorMessage">The message describing the validation error.</param>
    public ValidationError(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// The name of the property that failed validation.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PropertyName { get; init; }

    /// <summary>
    /// The message describing the validation error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; init; }

    /// <summary>
    /// The value that was provided for the property, which caused the validation to fail.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? AttemptedValue { get; init; }

    /// <summary>
    /// Gets a custom error code associated with the validation failure.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the expected value, if applicable.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ExpectedValue { get; init; }
}
