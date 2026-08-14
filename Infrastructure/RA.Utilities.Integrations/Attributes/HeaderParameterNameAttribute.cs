using System;

namespace RA.Utilities.Integrations.Attributes;

/// <summary>
/// Overrides the HTTP header name used for a property of a class marked with
/// <see cref="HeaderParametersAttribute"/>. By default the property name is used as the header name.
/// </summary>
/// <example>
/// <code>
/// [HeaderParameters]
/// public partial class RequestHeaders
/// {
///     [HeaderParameterName("x-request-id")]
///     public string? XCorrelationId { get; init; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class HeaderParameterNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderParameterNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The header name to use for the property.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
    public HeaderParameterNameAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// Gets the header name to use for the property.
    /// </summary>
    public string Name { get; }
}
