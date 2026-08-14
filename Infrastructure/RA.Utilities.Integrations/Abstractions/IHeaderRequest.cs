using System.Collections.Generic;

namespace RA.Utilities.Integrations.Abstractions;

/// <summary>
/// Defines a contract for request models that can provide HTTP header values.
/// Implementing this interface allows objects to be converted into a collection of header key-value pairs.
/// The <c>RA.Utilities.Integrations.Generators</c> source generator implements this interface automatically
/// for classes marked with the <c>[HeaderParameters]</c> attribute.
/// </summary>
public interface IHeaderRequest
{
    /// <summary>
    /// When implemented in a class, returns the header values as a dictionary of header names to header values.
    /// </summary>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> containing the header names and values for the request.
    /// </returns>
    Dictionary<string, string> ToHeaders();
}
