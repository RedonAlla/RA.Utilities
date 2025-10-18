using System.Text.Json.Serialization;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.Settings;

/// <summary>
/// Represents a single header to be added to the OpenAPI specification.
/// </summary>
public class HeaderDefinition
{
    /// <summary>
    /// The name of the header (e.g., "x-request-id").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A description of the header's purpose.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Specifies if the header is required. Defaults to true.
    /// </summary>
    public bool Required { get; set; } = true;

    /// <summary>
    /// The schema type for the header value (e.g., "string", "integer"). Defaults to String.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JsonSchemaType Type { get; set; } = JsonSchemaType.String;

    /// <summary>
    /// The format of the header value (e.g., "uuid", "date-time").
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// An example value for the header.
    /// </summary>
    public object? Value { get; set; }
}
