using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.Utilities;

/// <summary>
/// Provides utility methods for working with OpenApi operations.
/// </summary>
public static class OpenApiOperationUtilities
{
    /// <summary>
    /// Adds a request example to the specified OpenApi operation.
    /// </summary>
    /// <param name="operation">The OpenApi operation to add the example to.</param>
    /// <param name="exampleName">The name of the example.</param>
    /// <param name="example">The <see cref="IOpenApiExample"/> to add.</param>
    /// <param name="mediaType">The media type of the example (default is application/json).</param>
    public static void AddRequestExample(OpenApiOperation operation, string exampleName, IOpenApiExample example, string mediaType = MediaTypeNames.Application.Json)
    {
        if (operation.RequestBody?.Content?.TryGetValue(mediaType, out OpenApiMediaType? openApiMediaType) == true)
        {
            openApiMediaType.Examples ??= new Dictionary<string, IOpenApiExample>();
            openApiMediaType.Examples[exampleName] = example;
        }
    }

    /// <summary>
    /// Adds a response example to the specified OpenApi operation.
    /// </summary>
    /// <param name="operation">The OpenApi operation to add the example to.</param>
    /// <param name="statusCode">The HTTP status code for which the example is provided.</param>
    /// <param name="exampleName">The name of the example.</param>
    /// <param name="example">The <see cref="IOpenApiExample"/> to add.</param>
    /// <param name="mediaType">The media type of the example (default is application/json).</param>
    public static void AddResponseExample(OpenApiOperation operation, int statusCode, string exampleName, IOpenApiExample example, string mediaType = MediaTypeNames.Application.Json)
    {
        if (operation!.Responses!.TryGetValue(statusCode.ToString(CultureInfo.InvariantCulture), out IOpenApiResponse? response) &&
            response.Content!.TryGetValue(mediaType, out OpenApiMediaType? openApiMediaType))
        {
            openApiMediaType.Examples ??= new Dictionary<string, IOpenApiExample>();
            openApiMediaType.Examples[exampleName] = example;
        }
    }

    /// <summary>
    /// Adds a general error response example (HTTP 500 Internal Server Error) to the specified OpenApi operation.
    /// </summary>
    /// <param name="operation">The OpenApi operation to add the general error response to.</param>
    public static void AddGeneralErrorResponse(OpenApiOperation operation)
    {
        AddResponseExample(operation, StatusCodes.Status500InternalServerError, "InternalServerError", new OpenApiExample
        {
            Summary = "Internal server error",
            Description = "This is an example of a general error.",
            Value = JsonSerializer.SerializeToNode(new
            {
                ResponseCode = 500,
                ResponseType = "Error",
                ResponseMessage = "Something happened on our end.",
                Result = (object?)null
            })
        });
    }
}
