using System;
using System.Collections.Generic;

namespace RA.Utilities.Api.Options;

/// <summary>
/// Options for configuring the HttpLoggingMiddleware.
/// </summary>
public class HttpLoggingOptions
{
    /// <summary>
    /// Gets or sets a set of request path prefixes to ignore during logging.
    /// If a request path starts with any of these values, it will be ignored.
    /// The comparison is case-insensitive.
    /// </summary>
    public ISet<string> PathsToIgnore { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the maximum length of the request or response body to log in bytes.
    /// Payloads larger than this will be replaced with a placeholder message.
    /// Defaults to 32 KB (32 * 1024 bytes).
    /// </summary>
    public int MaxBodyLogLength { get; set; } = 32 * 1024;

    /// <summary>
    /// Gets or sets a set of header names to exclude from both request and response logging.
    /// Header name comparison is case-insensitive.
    /// Defaults to an empty set (all headers are logged).
    /// </summary>
    public ISet<string> ExcludedHeaders { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the response duration threshold in milliseconds above which
    /// the response will be logged at <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> instead of
    /// <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/>.
    /// Set to 0 to disable the warning threshold (default).
    /// </summary>
    public double WarningThresholdMilliseconds { get; set; }
}
