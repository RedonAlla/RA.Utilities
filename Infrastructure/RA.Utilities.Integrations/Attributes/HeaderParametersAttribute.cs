using System;

namespace RA.Utilities.Integrations.Attributes;

/// <summary>
/// Marks a partial class or record as a strongly-typed container of HTTP header values for an HTTP request.
/// The <c>RA.Utilities.Integrations.Generators</c> source generator implements
/// <see cref="RA.Utilities.Integrations.Abstractions.IHeaderRequest"/> on a generated partial part,
/// mapping each public instance property to a header key-value pair.
/// Null property values are skipped when building the headers.
/// </summary>
/// <example>
/// <code>
/// [HeaderParameters]
/// public partial class RequestHeaders
/// {
///     public string? XCorrelationId { get; init; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class HeaderParametersAttribute : Attribute
{
}
