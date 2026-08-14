using System;

namespace RA.Utilities.Integrations.Attributes;

/// <summary>
/// Overrides the query string key used for a property of a class marked with
/// <see cref="QueryParametersAttribute"/>. By default the property name is used as the query string key.
/// </summary>
/// <example>
/// <code>
/// [QueryParameters]
/// public partial class GetProductsQuery
/// {
///     [QueryParameterName("category_id")]
///     public int? CategoryId { get; init; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class QueryParameterNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The query string key to use for the property.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
    public QueryParameterNameAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// Gets the query string key to use for the property.
    /// </summary>
    public string Name { get; }
}
