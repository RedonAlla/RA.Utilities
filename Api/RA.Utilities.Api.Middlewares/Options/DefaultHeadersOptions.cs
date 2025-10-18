using System;
using System.Collections.Generic;

namespace RA.Utilities.Api.Middlewares.Options;

/// <summary>
/// Options for configuring the <see cref="DefaultHeadersMiddleware"/>.
/// </summary>
public class DefaultHeadersOptions
{
    /// <summary>
    /// Gets or sets a set of request path prefixes to ignore for header enforcement.
    /// </summary>
    public ISet<string> PathsToIgnore { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
