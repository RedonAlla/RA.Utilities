using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using RA.Utilities.Api.Results;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// An <see cref="IOpenApiOperationTransformer"/> implementation that adds a default error response (500) to OpenAPI operations.
/// </summary>
public sealed class DefaultResponsesOperationTransformer : IOpenApiOperationTransformer
{
    /// <summary>
    /// Transforms the specified <see cref="OpenApiOperation"/> by adding a default error response if not already present.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to transform.</param>
    /// <param name="context">The context for the operation transformation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        operation.Responses ??= [];
        string defaultErrorName = nameof(Response<>);

        if (!operation.Responses.ContainsKey(defaultErrorName))
        {
            var paramDesc = new ApiParameterDescription
            {
                Type = typeof(Response<ErrorResponse>),
                Source = BindingSource.Body,
                Name = defaultErrorName,
            };

            operation.Responses["500"] = new OpenApiResponse
            {
                Description = "Response designated \"catch-all\" for any unexpected or unhandled exceptions that occur within your application",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType
                    {
                        Schema = await context.GetOrCreateSchemaAsync(paramDesc.Type, paramDesc, cancellationToken),
                        Examples = new Dictionary<string, IOpenApiExample>
                        {
                            ["500"] = new OpenApiExample
                            {
                                Summary = "Default error response",
                                Description = "This is an example of a default error response.",
                                Value = System.Text.Json.JsonSerializer.SerializeToNode(new ErrorResponse())
                            }
                        }
                    }
                }
            };
        }
    }
}
