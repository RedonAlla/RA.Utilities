using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using RA.Utilities.Api.Json;
using RA.Utilities.Api.Options;
using RA.Utilities.Api.Utilities;
using RA.Utilities.Core.Constants;
using RA.Utilities.Logging.Shared.Constants;
using RA.Utilities.Logging.Shared.Models.HttpLog;

namespace RA.Utilities.Api.Middlewares;

/// <summary>
/// Factory-based middleware for logging HTTP requests and responses.
/// It implements IMiddleware, allowing it to be activated by DI and
/// have dependencies injected via its constructor.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
/// <param name="recyclableMemoryStreamManager">The recyclable memory stream manager.</param>
/// <param name="options">The middleware options.</param>
public class LoggingMiddleware(
    ILogger<LoggingMiddleware> logger,
    RecyclableMemoryStreamManager recyclableMemoryStreamManager,
    IOptions<HttpLoggingOptions> options
    ) : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = recyclableMemoryStreamManager ?? throw new ArgumentNullException(nameof(recyclableMemoryStreamManager));
    private readonly HttpLoggingOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Processes a request to log HTTP request and response.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The next delegate in the middleware pipeline.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var loggerScope = new Dictionary<string, object>
        {
            [LoggingConstants.XRequestId] = CommonUtilities.GetRequestId(context),
        };

        using (logger.BeginScope(loggerScope))
        {
            if (CommonUtilities.ShouldIgnorePath(context.Request.Path, _options.PathsToIgnore))
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
    }

    private async Task LogRequestAsync(HttpContext context)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        context.Request.EnableBuffering();

        var requestLog = new HttpRequestLogTemplate
        {
            RequestId = context.Request.Headers[HeaderParameters.XRequestId].FirstOrDefault(),
            TraceIdentifier = context.TraceIdentifier,
            Scheme = context.Request.Scheme,
            Host = context.Request.Host.ToString(),
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            RemoteAddress = context.Connection.RemoteIpAddress?.ToString(),
            RequestHeaders = FilterHeaders(context.Request.Headers, _options.ExcludedHeaders),
            RequestBody = await ReadBodyAsync(context.Request.Body)
        };

        _logger.LogInformation("HTTP Request: {@RequestLog}", requestLog);
    }

    private async Task LogResponseAsync(HttpContext context, MemoryStream responseBody, TimeSpan duration)
    {
        LogLevel logLevel = _options.WarningThresholdMilliseconds > 0 &&
                       duration.TotalMilliseconds > _options.WarningThresholdMilliseconds
            ? LogLevel.Warning
            : LogLevel.Information;

        if (!_logger.IsEnabled(logLevel))
            return;

        var responseLog = new HttpResponseLogTemplate
        {
            RequestId = context.Request.Headers[HeaderParameters.XRequestId].FirstOrDefault(),
            TraceIdentifier = context.TraceIdentifier,
            Path = context.Request.Path,
            RemoteAddress = context.Connection.RemoteIpAddress?.ToString(),
            StatusCode = context.Response.StatusCode,
            Duration = duration.TotalMilliseconds,
            ResponseHeaders = FilterHeaders(context.Response.Headers, _options.ExcludedHeaders),
            ResponseBody = await ReadBodyAsync(responseBody)
        };

        _logger.Log(logLevel, "HTTP Response: {@ResponseLog}", responseLog);
    }

    private async Task<object> ReadBodyAsync(Stream stream)
    {
        if (stream.Length == 0)
            return null;

        if (stream.Length > _options.MaxBodyLogLength)
            return $"[Body larger than {_options.MaxBodyLogLength} bytes. Truncated.]";

        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream, leaveOpen: true);
        string bodyAsString = await reader.ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);

        // Try to parse as JSON for structured logging
        try
        {
            return string.IsNullOrWhiteSpace(bodyAsString)
                ? null
                : JsonSerializer.Deserialize(bodyAsString, HttpLoggingJsonContext.Default.Object);
        }
        catch
        {
            return bodyAsString; // Fallback to string if not valid JSON
        }
    }

    private static Dictionary<string, string> FilterHeaders(IHeaderDictionary headers, ISet<string> excludedHeaders)
    {
        if (excludedHeaders.Count == 0)
            return headers.ToDictionary(x => x.Key, x => x.Value.ToString());

        return headers
            .Where(x => !excludedHeaders.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value.ToString());
    }
}
