using Microsoft.CodeAnalysis;

namespace RA.Utilities.Integrations.Generators;

/// <summary>
/// Contains the diagnostic descriptors reported by the request parameter source generator.
/// </summary>
internal static class Diagnostics
{
    /// <summary>
    /// The diagnostic category.
    /// </summary>
    private const string Category = "RA.Utilities.Integrations.Generators";

    /// <summary>
    /// Reports that a class marked with a query/header parameter attribute is not partial
    /// (or is nested inside a type that is not partial).
    /// </summary>
    public static readonly DiagnosticDescriptor NotPartial = new(
        id: "RPIG001",
        title: "Query/header parameter classes must be partial",
        messageFormat: "Class '{0}' with a query/header parameter attribute must be partial, and so must all of its containing types",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a query/header parameter attribute was applied to an unsupported type.
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedType = new(
        id: "RPIG002",
        title: "Query/header parameter attributes require a class or record",
        messageFormat: "The query/header parameter attributes can only be applied to classes and records, but '{0}' is not one",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
