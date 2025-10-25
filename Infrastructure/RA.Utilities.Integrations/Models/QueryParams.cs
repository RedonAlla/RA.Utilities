using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RA.Utilities.Integrations.Models;

/// <summary>
/// Represents a collection of key-value pairs for building URL query parameters.
/// This class extends <see cref="List{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
public class QueryParams : List<KeyValuePair<string, string>>
{
    /// <summary>
    /// Adds a key and value to the query parameter collection.
    /// </summary>
    /// <param name="key">The key of the query parameter.</param>
    /// <param name="value">The value of the query parameter.</param>
    public void Add(string key, string value)
    {
        Add(new KeyValuePair<string, string>(key, value));
    }

    /// <summary>
    /// Converts the collection of query parameters to a URL-encoded string.
    /// </summary>
    /// <returns>A URL-encoded query string, or an empty string if there are no parameters.</returns>
    public override string ToString()
    {
        if (Count == 0)
        {
            return string.Empty;
        }

        var segments = this.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}");

        return string.Join("&", segments);
    }
}
