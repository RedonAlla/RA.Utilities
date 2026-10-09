#pragma warning disable CA1873

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RA.Utilities.Integrations.Extensions;
using RA.Utilities.Logging.Shared.Models.HttpLog;

namespace RA.Utilities.Integrations.DelegatingHandlers;

/// <summary>
/// A <see cref="DelegatingHandler"/> that logs outgoing HTTP requests and incoming HTTP responses.
/// </summary>
/// <seealso cref="System.Net.Http.DelegatingHandler" />
public class RequestResponseLoggingHandler : DelegatingHandler
{
    /// <summary>
    /// The logger instance for this handler.
    /// </summary>
    private readonly ILogger<RequestResponseLoggingHandler> _logger;

    private const double _warningThresholdMilliseconds = 35000;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestResponseLoggingHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging requests and responses.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is <see langword="null"/>.</exception>
    public RequestResponseLoggingHandler(ILogger<RequestResponseLoggingHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation, logging the request and response.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HttpResponseMessage"/> from the server.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (!_logger.IsEnabled(LogLevel.Information))
            return response;

        var stopwatch = Stopwatch.StartNew();

        #region LogRequest
        var requestDto = new HttpRequestLogTemplate
        {
            Scheme = request.RequestUri?.Scheme,
            Host = request?.RequestUri?.Host,
            Method = request?.Method?.Method,
            Path = request?.RequestUri?.AbsoluteUri,
            QueryString = request?.RequestUri?.Query,
            RemoteAddress = request?.Options?.GetClientIpAddress() ?? string.Empty,
            RequestHeaders = request?.Headers.ToDictionary(),
            RequestBody = await GetHttpContent(request?.Content, cancellationToken)
        };

        _logger.LogInformation("HttpClient Request: {@RequestDto}", requestDto);
        #endregion

        #region LogResponse
        var responseDto = new HttpResponseLogTemplate
        {
            Path = response.RequestMessage?.RequestUri?.AbsoluteUri,
            RemoteAddress = request?.Options?.GetClientIpAddress() ?? string.Empty,
            StatusCode = (int)response.StatusCode,
            ResponseHeaders = response.Headers.ToDictionary(),
            Duration = $"{stopwatch.Elapsed.TotalMilliseconds} ms",
            ResponseBody = await GetHttpContent(response.Content, cancellationToken)
        };

        _logger.LogInformation("HttpClient Response: {@ResponseDto}", responseDto);

        if (stopwatch.Elapsed.TotalMilliseconds > _warningThresholdMilliseconds)
            _logger.LogWarning("HttpClient call to {Method} {Url} took too long ({ElapsedMilliseconds} ms)",
                requestDto.Method, $"{requestDto.Path}?{request?.RequestUri?.Query}", stopwatch.Elapsed.TotalMilliseconds
            );

        stopwatch.Stop();
        #endregion

        return response;
    }

    /// <summary>
    /// Reads the <see cref="HttpContent"/> and returns it as a deserialized object or a string.
    /// </summary>
    /// <param name="httpContent">The HTTP content to read.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the deserialized content as an <see cref="object"/> if it's valid JSON, the raw string content if it's not, or <see langword="null"/> if the content is empty.</returns>
    private async Task<object?> GetHttpContent(HttpContent? httpContent, CancellationToken cancellationToken)
    {
        if (httpContent is null)
            return null;

        string contentString = await httpContent.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(contentString))
            return null;

        try
        {
            return JsonSerializer.Deserialize<object>(contentString);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize body as JSON; it will be logged as a string.");
            // The body is not a valid JSON, so return it as a plain string.
            return contentString;
        }
    }
}
