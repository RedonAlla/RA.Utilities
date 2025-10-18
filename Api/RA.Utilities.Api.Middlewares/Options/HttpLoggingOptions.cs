using System;
using System.Collections.Generic;

namespace RA.Utilities.Api.Middlewares.Options;

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
}
