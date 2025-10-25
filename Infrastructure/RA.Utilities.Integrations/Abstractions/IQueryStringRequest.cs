using RA.Utilities.Integrations.Extensions;
using RA.Utilities.Integrations.Models;

namespace RA.Utilities.Integrations.Abstractions;

/// <summary>
/// Defines a contract for request models that can provide query string parameters.
/// Implementing this interface allows objects to be converted into a URL query string.
/// </summary>
public interface IQueryStringRequest
{
    /// <summary>
    /// When implemented in a class, returns the query string parameters as a collection of key-value pairs.
    /// </summary>
    /// <returns>
    /// A <see cref="QueryParams"/> collection containing the key-value pairs for the query string.
    /// </returns>
    QueryParams QueryStringValues();

    /// <summary>
    /// Converts the object's query string values into a URL-encoded query string.
    /// This is a default implementation that uses the <see cref="QueryStringValues"/> method.
    /// </summary>
    /// <param name="action">Endpoint to call.</param>
    /// <returns>
    /// A URL-encoded query string, prefixed with a question mark ('?'), or an empty string if there are no parameters.
    /// For example: "?key1=value1&amp;key2=value2".
    /// </returns>
    string ToQueryString(string action) => action + QueryStringValues()?.ToQueryString();
}
