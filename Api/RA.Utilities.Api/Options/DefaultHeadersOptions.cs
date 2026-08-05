using System;
using System.Collections.Generic;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Options;

/// <summary>
/// Options for configuring the <see cref="DefaultHeadersMiddleware"/>.
/// </summary>
public class DefaultHeadersOptions
{
    /// <summary>
    /// Gets or sets a set of request path prefixes to ignore for header enforcement.
    /// Paths starting with any value in this set will skip header validation.
    /// Comparisons are case-insensitive.
    /// </summary>
    public ISet<string> PathsToIgnore { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the collection of HTTP headers that must be present on incoming requests.
    /// Each entry defines the header name, whether to auto-generate a value when missing,
    /// whether to echo it in the response, and an optional custom error message.
    /// </summary>
    /// <remarks>
    /// Defaults to a single entry requiring <c>x-request-id</c> with auto-generation
    /// and response echoing enabled.
    /// </remarks>
    public ICollection<RequiredHeaderDefinition> RequiredHeaders { get; set; } =
    [
        new()
        {
            Name = HeaderParameters.XRequestId,
            AutoGenerate = true,
            EchoInResponse = true,
        },
    ];
}
