using System;
using System.Collections.Generic;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.Settings;

/// <summary>
/// Configuration settings for the <see cref="DocumentTransformers.HeadersParameterTransformer"/>.
/// </summary>
public class HeadersParameterSettings
{
    /// <summary>
    /// Value key in the `appsettings.json` file.
    /// </summary>
    public const string AppSettingsKey = "OpenApiHeaders";

    /// <summary>
    /// A list of header definitions to be added to all API requests.
    /// </summary>
    public List<HeaderDefinition> RequestHeaders { get; set; } = [
        new()
        {
            Name = "x-request-id",
            Description = "ID of the request, unique to the call, as determined by the initiating party.",
            Required = true,
            Type = JsonSchemaType.String,
            Format = "uuid",
            Value = Guid.NewGuid().ToString()
        }
    ];

    /// <summary>
    /// A list of header definitions to be added to all API responses.
    /// </summary>
    public List<HeaderDefinition> ResponseHeaders { get; set; } =
    [
        new()
        {
            Name = "x-request-id",
            Type = JsonSchemaType.String,
            Format = "uuid",
            Description = "Returns a request ID that can be used to track the request. It is the same value as the `x-request-id` in the request.",
            Value = Guid.NewGuid().ToString()
        },
        new()
        {
            Name = "trace-id",
            Format = "uuid",
            Type = JsonSchemaType.String,
            Description = "Returns a trace ID that can be used to track the request internally.",
            Value = Guid.NewGuid().ToString()
        }
    ];
}
