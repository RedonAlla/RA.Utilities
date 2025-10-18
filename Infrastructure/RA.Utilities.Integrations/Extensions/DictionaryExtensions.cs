using System;
using System.Linq;
using RA.Utilities.Integrations.Models;

namespace RA.Utilities.Integrations.Extensions;

/// <summary>
/// Extension methods for Dictionary and related models.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Converts a collection of key-value pairs into a URL query string.
    /// This method correctly handles URL encoding for both keys and values and constructs a valid query string.
    /// </summary>
    /// <param name="request">The <see cref="QueryParams"/> collection to convert.</param>
    /// <returns>
    /// A URL-encoded query string starting with '?', or an empty string if the request is null or contains no valid parameters.
    /// For example, `?key1=value1&amp;key2=value2`.
    /// </returns>
    public static string ToQueryString(this QueryParams request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        var queryParameters = request
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}")
            .ToList();

        if (queryParameters.Count == 0)
        {
            return string.Empty;
        }

        return $"?{string.Join("&", queryParameters)}";
    }
}
