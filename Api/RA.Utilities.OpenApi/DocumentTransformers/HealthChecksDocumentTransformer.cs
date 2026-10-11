using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// An <see cref="IOpenApiDocumentTransformer"/> that adds the health check endpoints
/// (<c>/health</c> and <c>/alive</c>) to the OpenAPI document.
/// </summary>
/// <remarks>
/// Endpoints mapped by <c>MapHealthChecks</c> carry no API metadata and are therefore not part of
/// the generated OpenAPI document. This transformer documents the readiness endpoint (<c>/health</c>)
/// with the JSON report schema written by <c>UIResponseWriter.WriteHealthCheckUIResponse</c> and the
/// liveness endpoint (<c>/alive</c>) with its plain text response.
/// <para>
/// Register it after the default document transformers so that the Bearer security scheme is not
/// applied to the health check operations, which must stay reachable without authentication.
/// </para>
/// </remarks>
internal sealed class HealthChecksDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string HealthPath = "/health";
    private const string AlivePath = "/alive";
    private const string TagName = "Health";
    private const string ReportSchemaName = "HealthCheckReport";
    private const string EntrySchemaName = "HealthCheckEntry";
    private const string OkDescription = "The service is healthy or degraded.";
    private const string ServiceUnavailableDescription = "The service is unhealthy.";

    /// <summary>
    /// Adds the health check paths, their component schemas and the health tag to the OpenAPI document.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The context for the document transformation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the transformation is finished.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(context);

        AddSchemas(document);
        AddPaths(document);
        AddTag(document);

        return Task.CompletedTask;
    }

    private static void AddSchemas(OpenApiDocument document)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();

        if (!document.Components.Schemas.ContainsKey(ReportSchemaName))
        {
            document.Components.Schemas[ReportSchemaName] = CreateReportSchema();
        }

        if (!document.Components.Schemas.ContainsKey(EntrySchemaName))
        {
            document.Components.Schemas[EntrySchemaName] = CreateEntrySchema();
        }
    }

    private static void AddPaths(OpenApiDocument document)
    {
        document.Paths ??= [];

        if (!document.Paths.ContainsKey(HealthPath))
        {
            document.Paths[HealthPath] = new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = CreateHealthOperation()
                }
            };
        }

        if (!document.Paths.ContainsKey(AlivePath))
        {
            document.Paths[AlivePath] = new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = CreateAliveOperation()
                }
            };
        }
    }

    private static void AddTag(OpenApiDocument document)
    {
        document.Tags ??= new HashSet<OpenApiTag>();

        if (document.Tags.All(tag => tag.Name != TagName))
        {
            document.Tags.Add(new OpenApiTag
            {
                Name = TagName,
                Description = """
                Service health check endpoints.
                * Get the service health report
                * Get the service liveness state​
                """
            });
        }
    }

    private static OpenApiOperation CreateHealthOperation()
    {
        return new OpenApiOperation
        {
            Tags = new HashSet<OpenApiTagReference> { new(TagName) },
            Summary = "Service health",
            Description = "Returns the aggregate health status of the service together with the result of each registered health check.",
            Responses = new OpenApiResponses
            {
                ["200"] = CreateJsonResponse(OkDescription),
                ["503"] = CreateJsonResponse(ServiceUnavailableDescription)
            }
        };
    }

    private static OpenApiOperation CreateAliveOperation()
    {
        return new OpenApiOperation
        {
            Tags = new HashSet<OpenApiTagReference> { new(TagName) },
            Summary = "Service liveness",
            Description = "Returns the liveness state of the service as plain text, based on the health checks tagged with 'live'.",
            Responses = new OpenApiResponses
            {
                ["200"] = CreateTextResponse(OkDescription),
                ["503"] = CreateTextResponse(ServiceUnavailableDescription)
            }
        };
    }

    private static OpenApiResponse CreateJsonResponse(string description)
    {
        return new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypeNames.Application.Json] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchemaReference(ReportSchemaName)
                }
            }
        };
    }

    private static OpenApiResponse CreateTextResponse(string description)
    {
        return new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypeNames.Text.Plain] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Description = "The health status of the service: 'Healthy', 'Degraded' or 'Unhealthy'."
                    }
                }
            }
        };
    }

    private static OpenApiSchema CreateReportSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Description = "The aggregate health report returned by the /health endpoint.",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["status"] = CreateStatusSchema("The aggregate health status of the service."),
                ["totalDuration"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The total time spent running all health checks, e.g. 00:00:00.0128087."
                },
                ["entries"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "The result of each registered health check, keyed by check name.",
                    AdditionalProperties = new OpenApiSchemaReference(EntrySchemaName)
                }
            }
        };
    }

    private static OpenApiSchema CreateEntrySchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Description = "The result of a single health check.",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["data"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "Arbitrary data returned by the health check."
                },
                ["description"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "An optional description of the health check result; omitted when not provided."
                },
                ["duration"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The time spent running this health check, e.g. 00:00:00.0015425."
                },
                ["exception"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The exception message when the health check fails; omitted otherwise."
                },
                ["status"] = CreateStatusSchema("The health status of this check."),
                ["tags"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema { Type = JsonSchemaType.String },
                    Description = "The tags associated with this health check."
                }
            }
        };
    }

    private static OpenApiSchema CreateStatusSchema(string description)
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Description = description,
            Enum = ["Healthy", "Degraded", "Unhealthy"]
        };
    }
}
