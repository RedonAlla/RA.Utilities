using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RA.Utilities.Api.Middlewares.Json;
using RA.Utilities.Api.Middlewares.Options;
using RA.Utilities.Logging.Shared.Models.HttpLog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using RA.Utilities.Api.Middlewares.Utilities;

namespace RA.Utilities.Api.Middlewares;

/// <summary>
/// Factory-based middleware for logging HTTP requests and responses.
/// It implements IMiddleware, allowing it to be activated by DI and
/// have dependencies injected via its constructor.
/// </summary>
public class HttpLoggingMiddleware : IMiddleware
{
    private readonly ILogger<HttpLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private readonly HttpLoggingOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="recyclableMemoryStreamManager">The recyclable memory stream manager.</param>
    /// <param name="options">The middleware options.</param>
    public HttpLoggingMiddleware(
        ILogger<HttpLoggingMiddleware> logger,
        RecyclableMemoryStreamManager recyclableMemoryStreamManager,
        IOptions<HttpLoggingOptions> options
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recyclableMemoryStreamManager = recyclableMemoryStreamManager ?? throw new ArgumentNullException(nameof(recyclableMemoryStreamManager));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Processes a request to log HTTP request and response.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The next delegate in the middleware pipeline.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (PathUtilities.ShouldIgnorePath(context.Request.Path, _options.PathsToIgnore))
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        // 1. Log Request
        await LogRequestAsync(context);

        // 2. Capture and Log Response
        Stream originalBodyStream = context.Response.Body;
        await using RecyclableMemoryStream responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        await next(context);
        stopwatch.Stop();

        await LogResponseAsync(context, responseBody, stopwatch.Elapsed);

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequestAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestLog = new HttpRequestLogTemplate
        {
            TraceIdentifier = context.TraceIdentifier,
            Schema = context.Request.Scheme,
            Host = context.Request.Host.ToString(),
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            RemoteAddress = context.Connection.RemoteIpAddress?.ToString(),
            RequestHeaders = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            RequestBody = await ReadBodyAsync(context.Request.Body)
        };

        _logger.LogInformation("HTTP Request: {@RequestLog}", requestLog);

        context.Request.Body.Position = 0;
    }

    private async Task LogResponseAsync(HttpContext context, MemoryStream responseBody, TimeSpan duration)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseLog = new HttpResponseLogTemplate
        {
            TraceIdentifier = context.TraceIdentifier,
            Path = context.Request.Path,
            RemoteAddress = context.Connection.RemoteIpAddress?.ToString(),
            StatusCode = context.Response.StatusCode,
            Duration = $"{duration.TotalMilliseconds:0.00} ms",
            ResponseHeaders = context.Response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            ResponseBody = await ReadBodyAsync(responseBody)
        };

        _logger.LogInformation("HTTP Response: {@ResponseLog}", responseLog);
    }

    private async Task<object> ReadBodyAsync(Stream stream)
    {
        if (stream.Length == 0)
        {
            return null;
        }

        if (stream.Length > _options.MaxBodyLogLength)
        {
            return $"[Body larger than {_options.MaxBodyLogLength} bytes. Truncated.]";
        }

        stream.Position = 0;
        using var reader = new StreamReader(stream, leaveOpen: true);
        string bodyAsString = await reader.ReadToEndAsync();
        stream.Position = 0;

        // Try to parse as JSON for structured logging
        try
        {
            return JsonSerializer.Deserialize<object>(bodyAsString, HttpLoggingJsonContext.Default.Object);
        }
        catch
        {
            return bodyAsString; // Fallback to string if not valid JSON
        }
    }
}
