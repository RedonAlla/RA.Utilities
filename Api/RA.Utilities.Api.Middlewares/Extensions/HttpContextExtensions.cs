using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
    public static string? GetXRequestId(this IHeaderDictionary headers)
    {
        if (headers == null)
        {
            return null;
        }

        return headers.TryGetValue(HeaderParameters.XRequestId, out StringValues value) ? value.ToString() : null;
    }
}
