namespace RA.Utilities.Api.Options;

/// <summary>
/// Defines a required HTTP header that must be present in incoming requests.
/// Used by <see cref="DefaultHeadersOptions"/> to configure header enforcement.
/// </summary>
public class RequiredHeaderDefinition
{
    /// <summary>
    /// Gets or sets the HTTP header name to enforce (e.g., "x-request-id").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a custom error message returned when the header is missing.
    /// When <c>null</c>, a default message is generated: "Header '{Name}' is required."
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically generate a value (a new <see cref="System.Guid"/>)
    /// when the header is missing, instead of returning a 400 Bad Request.
    /// </summary>
    public bool AutoGenerate { get; set; }

    /// <summary>
    /// Gets or sets whether to echo the header value back in the response.
    /// When <c>true</c> the header and its value are added to the response headers.
    /// </summary>
    public bool EchoInResponse { get; set; }
}
