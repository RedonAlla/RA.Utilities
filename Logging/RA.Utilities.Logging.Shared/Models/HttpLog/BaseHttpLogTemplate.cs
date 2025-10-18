using System;
using System.Text.Json;

namespace RA.Utilities.Logging.Shared.Models.HttpLog;

/// <summary>
/// Base containing HTTP request response properties for logging.
/// </summary>
public class BaseHttpLogTemplate
{
    /// <summary>
    /// Unique identifier to represent this request in trace logs.
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
    /// Gets the date and time the request was made, in UTC.
    /// </summary>
    public DateTime RequestedOn => DateTime.UtcNow;

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
