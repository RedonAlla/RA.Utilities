using System;

namespace RA.Utilities.Integrations.Attributes;

/// <summary>
/// Marks a partial class or record as a strongly-typed container of query string parameters for an HTTP request.
/// The <c>RA.Utilities.Integrations.Generators</c> source generator implements
/// <see cref="RA.Utilities.Integrations.Abstractions.IQueryStringRequest"/> on a generated partial part,
/// mapping each public instance property to a query string key-value pair.
/// Null property values are skipped when building the query string.
/// </summary>
/// <example>
/// <code>
/// [QueryParameters]
/// public partial class GetProductsQuery
/// {
///     public int? CategoryId { get; init; }
///     public string? Search { get; init; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class QueryParametersAttribute : Attribute
{
}
