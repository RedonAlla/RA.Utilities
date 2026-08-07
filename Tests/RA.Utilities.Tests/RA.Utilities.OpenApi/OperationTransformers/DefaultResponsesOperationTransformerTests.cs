using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using RA.Utilities.OpenApi.OperationTransformers;

namespace RA.Utilities.Tests.RA.Utilities.OpenApi.OperationTransformers;

/// <summary>
/// Tests for the <see cref="DefaultResponsesOperationTransformer"/> class.
/// </summary>
public class DefaultResponsesOperationTransformerTests
{
    /// <summary>
    /// Tests that <see cref="DefaultResponsesOperationTransformer.TransformAsync"/> adds a 500 response when no responses are present.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ShouldAdd500Response_WhenNoResponses()
    {
        var transformer = new DefaultResponsesOperationTransformer();
        var operation = new OpenApiOperation();
        ServiceProvider services = new ServiceCollection().BuildServiceProvider();

        var context = new OpenApiOperationTransformerContext
        {
            Description = null!,
            DocumentName = string.Empty,
            ApplicationServices = services
        };

        await transformer.TransformAsync(operation, context, CancellationToken.None);

        operation.Responses.Should().ContainKey("500");
    }

    /// <summary>
    /// Tests that <see cref="DefaultResponsesOperationTransformer.TransformAsync"/> does not duplicate a 500 response when it is already present.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ShouldNotDuplicate500Response_WhenAlreadyPresent()
    {
        var transformer = new DefaultResponsesOperationTransformer();
        var operation = new OpenApiOperation
        {
            Responses = []
        };
        // Pre-add a 500 response
        operation.Responses["500"] = new OpenApiResponse { Description = "Existing" };
        ServiceProvider services = new ServiceCollection().BuildServiceProvider();

        var context = new OpenApiOperationTransformerContext
        {
            Description = null!,
            DocumentName = string.Empty,
            ApplicationServices = services
        };

        await transformer.TransformAsync(operation, context, CancellationToken.None);

        // Should still only have one 500 entry
        operation.Responses.Should().ContainKey("500");
    }

    /// <summary>
    /// Tests that <see cref="DefaultResponsesOperationTransformer.TransformAsync"/> preserves existing responses while adding the default 500 response.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ShouldPreserveExistingResponses()
    {
        var transformer = new DefaultResponsesOperationTransformer();
        var operation = new OpenApiOperation
        {
            Responses = []
        };
        operation.Responses["200"] = new OpenApiResponse { Description = "OK" };
        ServiceProvider services = new ServiceCollection().BuildServiceProvider();

        var context = new OpenApiOperationTransformerContext
        {
            Description = null!,
            DocumentName = string.Empty,
            ApplicationServices = services
        };

        await transformer.TransformAsync(operation, context, CancellationToken.None);

        operation.Responses.Should().ContainKey("200");
        operation.Responses.Should().ContainKey("500");
    }

    /// <summary>
    /// Tests that <see cref="DefaultResponsesOperationTransformer.TransformAsync"/> throws an <see cref="ArgumentNullException"/> when the operation is null.
    /// </summary>
    [Fact]
    public async Task TransformAsync_WithNullOperation_ShouldThrowArgumentNullException()
    {
        var transformer = new DefaultResponsesOperationTransformer();
        ServiceProvider services = new ServiceCollection().BuildServiceProvider();

        var context = new OpenApiOperationTransformerContext
        {
            Description = null!,
            DocumentName = string.Empty,
            ApplicationServices = services
        };

        Func<Task> act = () =>
            transformer.TransformAsync(null!, context, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("operation");
    }
}
