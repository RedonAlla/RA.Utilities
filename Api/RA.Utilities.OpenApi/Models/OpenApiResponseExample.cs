namespace RA.Utilities.OpenApi.Models;

/// <summary>
/// Represents an example for an OpenAPI response body.
/// </summary>
public class OpenApiResponseExample : OpenApiExampleModel
{
    /// <summary>
    /// The HTTP status code for which the example is provided.
    /// </summary>
    public int StatusCodes { get; set; }
}
