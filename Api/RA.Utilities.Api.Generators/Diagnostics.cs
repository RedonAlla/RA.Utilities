using Microsoft.CodeAnalysis;

namespace RA.Utilities.Api.Generators;

/// <summary>
/// Contains the diagnostic descriptors reported by the endpoint registration source generator.
/// </summary>
internal static class Diagnostics
{
    /// <summary>
    /// The diagnostic category.
    /// </summary>
    private const string Category = "RA.Utilities.Api.Generators";

    /// <summary>
    /// Reports that two or more <c>IEndpointGroup</c> implementations declare the same constant
    /// <c>GroupName</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateGroupName = new(
        id: "EPMG001",
        title: "Duplicate IEndpointGroup name",
        messageFormat: "IEndpointGroup '{0}' declares the GroupName '{1}', which is also declared by another IEndpointGroup. Group names must be unique within the assembly.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that an <c>IEndpoint</c> references a constant <c>GroupName</c> for which no
    /// <c>IEndpointGroup</c> exists in the assembly.
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownGroupName = new(
        id: "EPMG002",
        title: "Unknown endpoint group name",
        messageFormat: "IEndpoint '{0}' references the GroupName '{1}', but no IEndpointGroup with that name was found in the assembly",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that an <c>IEndpointGroup</c> or <c>IEndpoint</c> implementation is generic, so the
    /// generated registration code cannot reference it without constructing a type argument.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericTypeNotSupported = new(
        id: "EPMG003",
        title: "Generic endpoint types are not supported",
        messageFormat: "IEndpoint/IEndpointGroup implementation '{0}' must not be generic, because the generated registration code cannot supply a type argument for it",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that an <c>IEndpointGroup</c> or <c>IEndpoint</c> implementation (or one of its
    /// containing types) is not accessible from the generated registration code.
    /// </summary>
    public static readonly DiagnosticDescriptor TypeNotAccessible = new(
        id: "EPMG004",
        title: "Endpoint types must be accessible from the generated code",
        messageFormat: "IEndpoint/IEndpointGroup implementation '{0}' and all of its containing types must be internal or public and not file-local, so that the generated registration code can reference it",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that an <c>IEndpointGroup</c> or <c>IEndpoint</c> implementation does not expose its
    /// static interface members as directly callable public static members (for example because they
    /// are implemented explicitly).
    /// </summary>
    public static readonly DiagnosticDescriptor StaticMembersNotAccessible = new(
        id: "EPMG005",
        title: "Endpoint static members must be implicitly implemented",
        messageFormat: "IEndpoint/IEndpointGroup implementation '{0}' must declare 'GroupName' and '{1}' as public static members. Explicit static interface implementations cannot be invoked by the generated registration code.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
