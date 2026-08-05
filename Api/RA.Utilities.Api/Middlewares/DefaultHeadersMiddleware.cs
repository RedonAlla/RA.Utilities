using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RA.Utilities.Api.Options;
using RA.Utilities.Api.Results;
using RA.Utilities.Api.Utilities;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Middlewares;

/// <summary>
/// Middleware to enforce the presence of required HTTP headers on incoming requests.
/// Configured via <see cref="DefaultHeadersOptions"/>.
/// </summary>
public class DefaultHeadersMiddleware : IMiddleware
{
    private readonly DefaultHeadersOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHeadersMiddleware"/> class.
    /// </summary>
    /// <param name="options">The options for the default headers middleware.</param>
    public DefaultHeadersMiddleware(IOptions<DefaultHeadersOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (CommonUtilities.ShouldIgnorePath(context.Request.Path, _options.PathsToIgnore))
        {
            await next(context);
            return;
        }

        List<BadRequestResult> missingHeaders = [];
        Dictionary<string, string> resolvedHeaders = new(StringComparer.OrdinalIgnoreCase);

        foreach (RequiredHeaderDefinition header in _options.RequiredHeaders)
        {
            string? headerValue = context.Request.Headers[header.Name].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                // Header is present — use the provided value
                resolvedHeaders[header.Name] = headerValue;
            }
            else if (header.AutoGenerate)
            {
                // Header is missing but auto-generation is enabled
                resolvedHeaders[header.Name] = Guid.NewGuid().ToString();
            }
            else
            {
                // Header is missing and must be provided by the caller
                missingHeaders.Add(new BadRequestResult
                {
                    PropertyName = header.Name,
                    ErrorMessage = header.ErrorMessage ?? $"Header '{header.Name}' is required.",
                    ErrorCode = "NotNullValidator",
                });
            }
        }

        if (missingHeaders.Count > 0)
        {
            context.Response.Headers.TryAdd(HeaderParameters.Location, context.Request.Path!.ToString());
            context.Response.Headers.TryAdd(HeaderParameters.XRequestId, Guid.NewGuid().ToString());

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json; charset=utf-8";
            string payload = JsonSerializer.Serialize(new BadRequestResponse(missingHeaders.ToArray()));
            await context.Response.WriteAsync(payload);

            // Short-circuit the pipeline — do not call the next middleware
            return;
        }

        // Echo resolved headers into the response
        foreach (RequiredHeaderDefinition header in _options.RequiredHeaders)
        {
            if (header.EchoInResponse && resolvedHeaders.TryGetValue(header.Name, out string? value))
                context.Response.Headers.TryAdd(header.Name, value);
        }

        await next(context);
    }
}
