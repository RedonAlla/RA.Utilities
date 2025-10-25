using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using RA.Utilities.OpenApi.Settings;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// An <see cref="IOpenApiDocumentTransformer"/> that adds configured headers to all API operations.
/// This transformer reads from <see cref="HeadersParameterSettings"/> to add headers to both requests and responses.
/// </summary>
public sealed class HeadersParameterTransformer : IOpenApiDocumentTransformer
{
    private readonly HeadersParameterSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadersParameterTransformer"/> class.
    /// </summary>
    /// <param name="settings">The configuration settings for headers, injected via IOptions.</param>
    public HeadersParameterTransformer(IOptions<HeadersParameterSettings> settings)
    {
        _settings = settings?.Value ?? new HeadersParameterSettings();
    }

    /// <summary>
    /// Adds configured request and response headers to all operations in the OpenAPI document based on the provided settings.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The context for the document transformation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the transformation is finished.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        foreach (OpenApiOperation? operation in document.Paths.Values.SelectMany(path => path?.Operations?.Values))
        {
            if (operation is null)
            {
                continue;
            }

            AddRequestHeaders(operation);
            AddResponseHeaders(operation);
        }

        return Task.CompletedTask;
    }

    private void AddRequestHeaders(OpenApiOperation operation)
    {
        operation.Parameters ??= [];

        foreach (HeaderDefinition headerDef in _settings.RequestHeaders)
        {
            if (headerDef is { Name.Length: > 0 } && !operation.Parameters.Any(p => p.Name == headerDef.Name && p.In == ParameterLocation.Header))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = headerDef.Name,
                    In = ParameterLocation.Header,
                    Required = headerDef.Required,
                    Description = headerDef.Description,
                    Schema = CreateSchemaFromDefinition(headerDef)
                });
            }
        }
    }

    private void AddResponseHeaders(OpenApiOperation operation)
    {
        foreach (IOpenApiResponse response in operation.Responses?.Values ?? Enumerable.Empty<IOpenApiResponse>())
        {
            // The IOpenApiResponse.Headers property is read-only.
            // We must work with the existing dictionary, which should be initialized by the object.
            // If it's null and we can't cast to a concrete type to set it, we skip.
            if (response.Headers is null && response is OpenApiResponse concreteResponse)
            {
                concreteResponse.Headers = new Dictionary<string, IOpenApiHeader>();
            }

            if (response.Headers is null)
            {
                continue;
            }

            foreach (HeaderDefinition headerDef in _settings.ResponseHeaders)
            {
                if (headerDef is { Name.Length: > 0 } && !response.Headers.ContainsKey(headerDef.Name))
                {
                    response.Headers.Add(headerDef.Name, new OpenApiHeader
                    {
                        Required = headerDef.Required,
                        Description = headerDef.Description,
                        Schema = CreateSchemaFromDefinition(headerDef)
                    });
                }
            }
        }
    }

    private static OpenApiSchema CreateSchemaFromDefinition(HeaderDefinition headerDef)
    {
        return new OpenApiSchema
        {
            Type = headerDef.Type,
            Format = headerDef.Format,
            Example = headerDef.Value is not null ? JsonValue.Create(headerDef.Value) : null
        };
    }
}
