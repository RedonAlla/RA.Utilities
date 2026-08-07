using System.Net.Mime;

namespace RA.Utilities.OpenApi.Models;

/// <summary>
/// Base class for OpenAPI example models.
/// </summary>
public class OpenApiExampleModel
{
    /// <summary>
    /// The key to identify the example.
    /// </summary>
    public string ExampleKey { get; set; } = string.Empty;

    /// <summary>
    /// Short description for the example.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Long description for the example.
    /// CommonMark syntax MAY be used for rich text representation.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// JSON value for Request or Response example.
    /// </summary>
    public object Value { get; set; } = string.Empty;

    /// <summary>
    /// The media type of the example (default is application/json).
    /// </summary>
    public string MediaType { get; set; } = MediaTypeNames.Application.Json;
}
