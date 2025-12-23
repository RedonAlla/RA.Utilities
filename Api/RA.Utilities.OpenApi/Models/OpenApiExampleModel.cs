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
    public string ExampleKey { get; set; }

    /// <summary>
    /// Short description for the example.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Long description for the example.
    /// CommonMark syntax MAY be used for rich text representation.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// JSON value for Request or Response example.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// The media type of the example (default is application/json).
    /// </summary>
    public string MediaType { get; set; } = MediaTypeNames.Application.Json;
}
