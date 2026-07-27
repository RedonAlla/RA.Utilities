using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RA.Utilities.Core.Constants;

/// <summary>
/// Represents a strongly-typed response type that can be extended by consuming projects.
/// Built-in values cover common HTTP response categories, and additional values
/// are created by inheriting from this record.
/// </summary>
/// <example>
/// // Using a built-in value:
/// throw new RaBaseException(400, ResponseType.Validation, "Invalid input.");
///
/// // Defining custom response types in a consuming project:
/// <code>
/// public record PaymentRequiredResponseType : ResponseType
/// {
///     private PaymentRequiredResponseType(string value) : base(value) { }
///     public static readonly PaymentRequiredResponseType Instance = new("PaymentRequired");
/// }
/// </code>
/// </example>
[JsonConverter(typeof(ResponseTypeJsonConverter))]
public record ResponseType
{
    /// <summary>
    /// Gets the string value of this response type.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseType"/> class.
    /// Accessible to derived types and types within the same assembly.
    /// </summary>
    /// <param name="value">The string value for the response type.</param>
    protected internal ResponseType(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    // ── Built-in values ──────────────────────────────────────────

    /// <summary>
    /// The operation was successful.
    /// </summary>
    public static readonly ResponseType Success = new("Success");

    /// <summary>
    /// A resource was created. Corresponds to HTTP 201.
    /// </summary>
    public static readonly ResponseType Created = new("Created");

    /// <summary>
    /// A resource was updated. Corresponds to HTTP 200.
    /// </summary>
    public static readonly ResponseType Updated = new("Updated");

    /// <summary>
    /// A resource was deleted. Corresponds to HTTP 200.
    /// </summary>
    public static readonly ResponseType Deleted = new("Deleted");

    /// <summary>
    /// The request succeeded with no content to return.
    /// Corresponds to HTTP 204.
    /// </summary>
    public static readonly ResponseType NoContent = new("NoContent");

    /// <summary>
    /// The request was accepted for processing but is not yet complete.
    /// Corresponds to HTTP 202.
    /// </summary>
    public static readonly ResponseType Accepted = new("Accepted");

    /// <summary>
    /// The request failed validation.
    /// Corresponds to HTTP 400.
    /// </summary>
    public static readonly ResponseType Validation = new("Validation");

    /// <summary>
    /// An unexpected problem occurred.
    /// Corresponds to HTTP 500.
    /// </summary>
    public static readonly ResponseType Problem = new("Problem");

    /// <summary>
    /// The requested resource was not found.
    /// Corresponds to HTTP 404.
    /// </summary>
    public static readonly ResponseType NotFound = new("NotFound");

    /// <summary>
    /// A conflict with the current state occurred.
    /// Corresponds to HTTP 409.
    /// </summary>
    public static readonly ResponseType Conflict = new("Conflict");

    /// <summary>
    /// Authentication is required.
    /// Corresponds to HTTP 401.
    /// </summary>
    public static readonly ResponseType Unauthorized = new("Unauthorized");

    /// <summary>
    /// A general, unspecified error occurred.
    /// </summary>
    public static readonly ResponseType Error = new("Error");

    /// <summary>
    /// The request was malformed.
    /// Corresponds to HTTP 400.
    /// </summary>
    public static readonly ResponseType BadRequest = new("BadRequest");

    /// <summary>
    /// The request was semantically incorrect.
    /// Corresponds to HTTP 422.
    /// </summary>
    public static readonly ResponseType Unprocessable = new("Unprocessable");

    /// <summary>
    /// Insufficient permissions.
    /// Corresponds to HTTP 403.
    /// </summary>
    public static readonly ResponseType Forbidden = new("Forbidden");

    /// <summary>
    /// Too many requests.
    /// Corresponds to HTTP 429.
    /// </summary>
    public static readonly ResponseType TooManyRequests = new("TooManyRequests");

    /// <summary>
    /// The service is temporarily unavailable.
    /// Corresponds to HTTP 503.
    /// </summary>
    public static readonly ResponseType ServiceUnavailable = new("ServiceUnavailable");

    /// <summary>
    /// Gateway timeout. Corresponds to HTTP 504.
    /// </summary>
    public static readonly ResponseType GatewayTimeout = new("GatewayTimeout");

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="ResponseType"/> to its string <see cref="Value"/>.
    /// </summary>
    /// <param name="type">The <see cref="ResponseType"/> to convert.</param>
    /// <returns>The string value of the response type.</returns>
    public static implicit operator string(ResponseType type) => type.Value;
}

/// <summary>
/// JSON converter for <see cref="ResponseType"/> that serializes it as a plain string.
/// </summary>
public sealed class ResponseTypeJsonConverter : JsonConverter<ResponseType>
{
    /// <inheritdoc />
    public override ResponseType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        return string.IsNullOrWhiteSpace(value)
            ? ResponseType.Error
            : new ResponseType(value);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ResponseType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
