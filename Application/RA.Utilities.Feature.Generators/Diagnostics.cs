using Microsoft.CodeAnalysis;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// Contains the diagnostic descriptors reported by the handler registration source generator.
/// </summary>
internal static class Diagnostics
{
    /// <summary>
    /// The diagnostic category.
    /// </summary>
    private const string Category = "RA.Utilities.Feature.Generators";

    /// <summary>
    /// Reports that the consuming compilation targets a framework without the
    /// <c>ModuleInitializerAttribute</c> (pre-.NET 5), so handlers cannot be registered
    /// automatically and must be registered explicitly.
    /// </summary>
    public static readonly DiagnosticDescriptor ModuleInitializersUnavailable = new(
        id: "FEAG001",
        title: "Compile-time handler registration requires module initializers",
        messageFormat: "RA.Utilities.Feature handler auto-registration requires .NET 5 or later (System.Runtime.CompilerServices.ModuleInitializerAttribute). Register handlers explicitly with AddFeature or AddNotification instead.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a handler implementation is generic, so the generated registration code
    /// cannot reference it without supplying a type argument.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericHandlerNotSupported = new(
        id: "FEAG002",
        title: "Generic handler types are not auto-registered",
        messageFormat: "Handler implementation '{0}' must not be generic, because the generated registration code cannot supply a type argument for it. Register it explicitly with AddFeature or AddNotification.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a handler implementation (or one of its containing types) is not accessible
    /// from the generated registration code.
    /// </summary>
    public static readonly DiagnosticDescriptor HandlerNotAccessible = new(
        id: "FEAG003",
        title: "Handler types must be accessible from the generated code",
        messageFormat: "Handler implementation '{0}' and all of its containing types must be internal or public and not file-local, so that the generated registration code can reference it",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a handler implementation is a struct, which the generated registration
    /// code does not support.
    /// </summary>
    public static readonly DiagnosticDescriptor StructHandlerNotSupported = new(
        id: "FEAG004",
        title: "Struct handlers are not supported",
        messageFormat: "Handler implementation '{0}' must be a class, because handlers are registered and resolved as class services",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that two or more classes register the same request handler pair within the
    /// assembly, which would make the winning registration load-order dependent.
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateRequestHandler = new(
        id: "FEAG005",
        title: "Duplicate request handler registration",
        messageFormat: "Handler '{0}' implements '{1}', which is also implemented by another handler in this assembly. A request may have exactly one handler; remove one of the implementations.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
