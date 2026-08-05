using System;
using System.Text.Json;

namespace RA.Utilities.Logging.Shared.Models.HttpLog;

/// <summary>
/// Base class containing HTTP request/response properties for logging.
/// </summary>
public class BaseHttpLogTemplate
{
    /// <summary>
    /// The value of the <c>x-request-id</c> header from the HTTP request.
    /// Provides end-to-end correlation across services.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Unique identifier to represent this request in trace logs.
    /// Value from Microsoft.AspNetCore.Http.
    /// </summary>
    public string? TraceIdentifier { get; set; }

    /// <summary>
    /// The URI used by the request message.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// The date and time the request was made, in UTC.
    /// Defaults to <see cref="DateTime.UtcNow"/> at the time the instance is created.
    /// </summary>
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The host name requested.
    /// This is usually the DNS host name or IP address of the server.
    /// </summary>
    public string? RemoteAddress { get; set; }

    /// <summary>
    /// Serializes the current <see cref="BaseHttpLogTemplate"/> object to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the current object.</returns>
    public override string ToString() => JsonSerializer.Serialize(this);
}
