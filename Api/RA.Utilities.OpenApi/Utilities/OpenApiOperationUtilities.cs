using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;
using RA.Utilities.OpenApi.Models;

namespace RA.Utilities.OpenApi.Utilities;

/// <summary>
/// Provides utility methods for working with OpenApi operations.
/// </summary>
public static class OpenApiOperationUtilities
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

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
    /// Adds multiple request examples to the specified OpenApi operation.
    /// </summary>
    /// <param name="operation">The OpenApi operation to add the examples to.</param>
    /// <param name="examples">An array of <see cref="OpenApiRequestExample"/> to add.</param>
    public static void AddRequestExamples(OpenApiOperation operation, OpenApiRequestExample[] examples)
    {
        foreach (OpenApiRequestExample example in examples)
        {
            AddRequestExample(operation, example.ExampleKey, new OpenApiExample()
            {
                Summary = example.Summary,
                Description = example.Description,
                Value = JsonSerializer.SerializeToNode(example.Value, _jsonSerializerOptions),
            }, example.MediaType);
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
    /// Adds multiple response examples to the specified OpenApi operation.
    /// </summary>
    /// <param name="operation">The OpenApi operation to add the examples to.</param>
    /// <param name="examples">An array of <see cref="OpenApiResponseExample"/> to add.</param>
    public static void AddResponseExamples(OpenApiOperation operation, OpenApiResponseExample[] examples)
    {
        foreach (OpenApiResponseExample example in examples)
        {
            AddResponseExample(operation, example.StatusCodes, example.ExampleKey, new OpenApiExample
            {
                Summary = example.Summary,
                Description = example.Description,
                Value = JsonSerializer.SerializeToNode(example.Value, _jsonSerializerOptions)
            });
        }
    }

    /// <summary>
    /// Adds a general error response example (HTTP 500 Internal Server Error) to the specified OpenApi operation.
    /// </summary>
    /// <param name="operation">The OpenApi operation to add the general error response to.</param>
    public static void AddGeneralErrorResponse(OpenApiOperation operation)
    {
        AddResponseExample(operation, StatusCodes.Status500InternalServerError, nameof(BaseResponseCode.InternalServerError), new OpenApiExample
        {
            Summary = "Internal server error",
            Description = "This is an example of a general error.",
            Value = JsonSerializer.SerializeToNode(new ErrorResponse(), _jsonSerializerOptions)
        });
    }
}
