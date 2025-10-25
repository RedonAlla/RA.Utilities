using System.Linq;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Middlewares.Extensions;

/// <summary>
/// Class congaing extensions methods related with <see cref="HttpContext"/>
/// </summary>
internal static class HttpContextExtensions
{
    /// <summary>
    /// Retrieves the value of the <c>X-Request-Id</c> header from the specified <see cref="IHeaderDictionary"/>.
    /// </summary>
    /// <param name="headers">The collection of HTTP headers.</param>
    /// <returns>
    /// The value of the <c>X-Request-Id</c> header if present; otherwise, <c>null</c>.
    /// </returns>
    public static string? GetXRequestId(this IHeaderDictionary headers) =>
        headers?.FirstOrDefault(k => k.Key == HeaderParameters.XRequestId)!.Value;

    /// <summary>
    /// Adds new header value in to <see cref="IHeaderDictionary"/>.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <param name="key">Key to add on header.</param>
    /// <param name="value">The value of header for the given key.</param>
    /// <returns><see cref="IHeaderDictionary"/></returns>
    public static IHeaderDictionary AddSafe(this IHeaderDictionary headers, string key, string value)
    {
        headers.Remove(key);
        headers.Append(key, value);

        return headers;
    }
}
