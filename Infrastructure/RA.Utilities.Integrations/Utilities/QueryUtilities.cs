using System;
using System.Collections.Generic;
using System.Linq;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.Models;

namespace RA.Utilities.Integrations.Utilities;

/// <summary>
/// Provides utility methods for handling query string parameters.
/// </summary>
public static class QueryUtilities
{
    /// <summary>
    /// Converts an action and an optional <see cref="IQueryStringRequest"/> into a URL-encoded query string.
    /// </summary>
    /// <param name="action">The base action or endpoint.</param>
    /// <param name="request">An object containing query string parameters, or null.</param>
    /// <returns>
    /// A URL-encoded query string, prefixed with the action, and potentially a question mark ('?')
    /// followed by parameters, or just the action if no parameters are provided.
    /// </returns>
    public static string ToQueryString(string action, IQueryStringRequest? request)
    {
        if (request is null)
            return action;

        return request.ToQueryString(action);
    }

    /// <summary>
    /// Converts a collection of query parameters into a URL-encoded query string.
    /// Parameters with empty values are skipped.
    /// </summary>
    /// <param name="request">The collection of query parameters.</param>
    /// <returns>
    /// A URL-encoded query string, prefixed with a question mark ('?'), or an empty string
    /// if the collection is null, empty, or contains no parameters with values.
    /// </returns>
    public static string ToQueryString(QueryParams? request)
    {
        if (request is null)
            return string.Empty;

        IEnumerable<string> queryParameters = request
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}");

        string queryString = string.Join("&", queryParameters);
        return queryString.Length == 0 ? string.Empty : $"?{queryString}";
    }
}
