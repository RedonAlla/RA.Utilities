using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.OpenApi.OperationTransformers;

/// <summary>
/// An <see cref="IOpenApiOperationTransformer"/> implementation that adds a default error response (500) to OpenAPI operations.
/// </summary>
internal sealed class DefaultResponsesOperationTransformer : IOpenApiOperationTransformer
{
    private static readonly string InternalServerError = BaseResponseCode.InternalServerError.ToString(CultureInfo.InvariantCulture);

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

            operation.Responses[InternalServerError] = new OpenApiResponse
            {
                Description = "Response designated \"catch-all\" for any unexpected or unhandled exceptions that occur within your application",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType
                    {
                        Schema = await context.GetOrCreateSchemaAsync(paramDesc.Type, paramDesc, cancellationToken),
                        Examples = new Dictionary<string, IOpenApiExample>
                        {
                            [InternalServerError] = new OpenApiExample
                            {
                                Summary = "Default error response",
                                Description = "This is an example of a default error response.",
                                Value = JsonSerializer.SerializeToNode(new ErrorResponse())
                            }
                        }
                    }
                }
            };
        }
    }
}
