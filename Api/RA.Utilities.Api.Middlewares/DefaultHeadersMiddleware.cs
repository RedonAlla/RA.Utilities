using System;
using System.Threading.Tasks;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using RA.Utilities.Api.Middlewares.Extensions;
using RA.Utilities.Api.Middlewares.Utilities;
using Microsoft.AspNetCore.HttpLogging;
using RA.Utilities.Api.Middlewares.Options;
using Microsoft.Extensions.Options; // Add this using directive for JsonSerializer.

namespace RA.Utilities.Api.Middlewares;

/// <summary>
/// Middleware to enforce the presence of the X-Request-Id header in incoming requests.
/// If the X-Request-Id header is missing, it returns a 400 Bad Request response.
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
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (PathUtilities.ShouldIgnorePath(context.Request.Path, _options.PathsToIgnore))
        {
            await next(context);
            return;
        }

        string xRequestId = context.Request.Headers.GetXRequestId();

        if (string.IsNullOrWhiteSpace(xRequestId))
        {
            var result = new BadRequestResult[] {
                new() {
                    PropertyName = HeaderParameters.XRequestId,
                    ErrorMessage = $"Header '{HeaderParameters.XRequestId}' is required.",
                    ErrorCode = "NotNullValidator",
                }
            };

            context.Response.Headers.AddSafe(HeaderParameters.Location, context.Request.Path!);
            context.Response.Headers.AddSafe(HeaderParameters.XRequestId, Guid.NewGuid().ToString());

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json; charset=utf-8";
            string payload = JsonSerializer.Serialize(new BadRequestResponse(result));
            await context.Response.WriteAsync(payload);

            // Short-circuit the pipeline — do not call the next middleware
            return;
        }

        context.Response.Headers.AddSafe(HeaderParameters.XRequestId, xRequestId);
        await next(context);
    }
}
