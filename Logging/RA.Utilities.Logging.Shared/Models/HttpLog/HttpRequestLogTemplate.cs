using System.Collections.Generic;

namespace RA.Utilities.Logging.Shared.Models.HttpLog;

/// <summary>
/// Model for logging HTTP request.
/// </summary>
public class HttpRequestLogTemplate : BaseHttpLogTemplate
{
    /// <summary>
    /// Scheme string of request Uri used by the request message.
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// The HTTP method used by the request message.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// The host name used by the request message.
    /// This is usually the DNS host name or IP address of the server.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// The query string used by the request message.
    /// </summary>
    public string? QueryString { get; set; }

    /// <summary>
    /// The collection of HTTP request headers used by the request message.
    /// </summary>
    public IDictionary<string, string>? RequestHeaders { get; set; }

    /// <summary>
    /// HTTP Request body.
    /// </summary>
    public object? RequestBody { get; set; }
}
