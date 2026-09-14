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

    /// <summary>
    /// Reports that a message type implements multiple message contracts (for example two distinct
    /// <c>IRequest&lt;TResponse&gt;</c> interfaces, or a request contract together with
    /// <c>INotification</c>), which the generated mediator cannot represent in its per-message
    /// dispatch. The message is excluded from the generated dispatch.
    /// </summary>
    public static readonly DiagnosticDescriptor MessageInheritsMultipleMessageInterfaces = new(
        id: "FEAG006",
        title: "Message implements multiple message contracts",
        messageFormat: "Message type '{0}' implements multiple message contracts ({1}). It is excluded from the generated mediator dispatch. Give the message a single contract.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a request message declared in the current compilation has no handler
    /// implementation anywhere in the compilation, so sending it would fail at runtime.
    /// </summary>
    public static readonly DiagnosticDescriptor RequestMessageHasNoHandler = new(
        id: "FEAG007",
        title: "Request message has no handler",
        messageFormat: "Request message '{0}' has no IRequestHandler implementation in the compilation. Sending it will fail at runtime; add a handler or register one explicitly.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a message type is generic or not accessible from the generated code, so the
    /// generated mediator cannot emit monomorphic overloads for it and the generic interface
    /// methods dispatch it instead.
    /// </summary>
    public static readonly DiagnosticDescriptor MessageNotSupportedByGeneratedDispatch = new(
        id: "FEAG008",
        title: "Message falls back to generic dispatch",
        messageFormat: "Message type '{0}' is generic or not accessible from the generated code, so it dispatches through the generic interface methods. Make the message non-generic and internal or public for the fastest dispatch path.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a handler class implements both the void and the request/response contract for
    /// the same message, which is ambiguous: the void contract is only reachable through the
    /// generic <c>Send&lt;TRequest&gt;</c> overload and the registration order decides the winner.
    /// </summary>
    public static readonly DiagnosticDescriptor HandlerImplementsBothContracts = new(
        id: "FEAG009",
        title: "Handler implements both request contracts for the same message",
        messageFormat: "Handler '{0}' implements both IRequestHandler<{1}> and IRequestHandler<{1}, TResponse> for the same message. Split the void and the request/response handling into separate handler classes.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Reports that a pipeline behavior implementation is generic, so the generated registration
    /// code cannot supply a type argument for it. Register it explicitly with <c>AddDecoration</c>;
    /// the generated mediator resolves it through DI like any explicitly registered behavior.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericBehaviorNotInjected = new(
        id: "FEAG010",
        title: "Generic behavior types are not auto-registered",
        messageFormat: "Pipeline behavior '{0}' is generic, so it is not auto-registered. Register it explicitly with AddDecoration — the generated mediator resolves explicitly registered behaviors from DI.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
