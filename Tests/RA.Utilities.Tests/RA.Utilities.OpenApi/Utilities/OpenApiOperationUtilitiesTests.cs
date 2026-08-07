using System;
using System.Collections.Generic;
using System.Net.Mime;
using FluentAssertions;
using Microsoft.OpenApi;
using RA.Utilities.OpenApi.Models;
using RA.Utilities.OpenApi.Utilities;

namespace RA.Utilities.Tests.RA.Utilities.OpenApi.Utilities;

/// <summary>
/// Tests for the <see cref="OpenApiOperationUtilities"/> class.
/// </summary>
public class OpenApiOperationUtilitiesTests
{
    private static OpenApiOperation CreateOperation()
    {
        return new OpenApiOperation
        {
            Responses = []
        };
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddGeneralErrorResponse"/> adds a 500 response to the operation.
    /// </summary>
    [Fact]
    public void AddGeneralErrorResponse_ShouldAdd500Response()
    {
        OpenApiOperation operation = CreateOperation();

        OpenApiOperationUtilities.AddGeneralErrorResponse(operation);

        operation.Responses.Should().ContainKey("500");
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddGeneralErrorResponse"/> includes JSON content for the 500 response.
    /// </summary>
    [Fact]
    public void AddGeneralErrorResponse_ShouldIncludeJsonContent()
    {
        OpenApiOperation operation = CreateOperation();

        OpenApiOperationUtilities.AddGeneralErrorResponse(operation);

        IOpenApiResponse response = operation?.Responses!["500"];
        response?.Content.Should().ContainKey(MediaTypeNames.Application.Json);
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddGeneralErrorResponse"/> includes an example for the 500 response.
    /// </summary>
    [Fact]
    public void AddGeneralErrorResponse_ShouldIncludeExample()
    {
        OpenApiOperation operation = CreateOperation();

        OpenApiOperationUtilities.AddGeneralErrorResponse(operation);

        IOpenApiMediaType? content = operation?.Responses!["500"]?.Content![MediaTypeNames.Application.Json];
        content?.Examples.Should().ContainKey("InternalServerError");
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddResponseExample"/> does not throw an exception when the operation has missing responses.
    /// </summary>
    [Fact]
    public void AddResponseExample_WithMissingResponses_ShouldNotThrow()
    {
        var operation = new OpenApiOperation();

        Action act = () => OpenApiOperationUtilities.AddResponseExample(
            operation, 200, "Example", new OpenApiExample());

        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddResponseExamples"/> processes multiple examples without throwing an exception.
    /// </summary>
    [Fact]
    public void AddResponseExamples_ShouldProcessMultipleExamples()
    {
        OpenApiOperation operation = CreateOperation();
        // Add a 200 response so one example can be added
        operation.Responses!["200"] = new OpenApiResponse
        {
            Content = new Dictionary<string, IOpenApiMediaType>
            {
                [MediaTypeNames.Application.Json] = new OpenApiMediaType
                {
                    Examples = new Dictionary<string, IOpenApiExample>()
                }
            }
        };

        OpenApiResponseExample[] examples =
        [
            new OpenApiResponseExample
            {
                StatusCode = 200,
                ExampleKey = "Success",
                Summary = "A successful response",
                Value = new { id = 1 }
            }
        ];

        Action act = () => OpenApiOperationUtilities.AddResponseExamples(operation, examples);
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddRequestExample"/> does not throw an exception when the operation has a missing request body.
    /// </summary>
    [Fact]
    public void AddRequestExample_WithMissingRequestBody_ShouldNotThrow()
    {
        var operation = new OpenApiOperation();

        Action act = () => OpenApiOperationUtilities.AddRequestExample(
            operation, "Example", new OpenApiExample());

        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that <see cref="OpenApiOperationUtilities.AddRequestExamples"/> does not throw an exception when the operation has a missing request body.
    /// </summary>
    [Fact]
    public void AddRequestExamples_ShouldNotThrowOnEmptyOperation()
    {
        var operation = new OpenApiOperation();
        OpenApiRequestExample[] examples =
        [
            new OpenApiRequestExample
            {
                ExampleKey = "Pet",
                Summary = "A pet example",
                Value = new { name = "Fluffy" }
            }
        ];

        Action act = () => OpenApiOperationUtilities.AddRequestExamples(operation, examples);
        act.Should().NotThrow();
    }
}
