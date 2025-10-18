using System;
using System.Collections.Generic;

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
}
