using System;
using Microsoft.CodeAnalysis;

namespace RA.Utilities.Feature.Generators.Models;

/// <summary>
/// Identifies which handler contract a discovered type implements.
/// </summary>
internal enum HandlerKind
{
    /// <summary>
    /// An <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> implementation, registered scoped.
    /// </summary>
    RequestResponse,

    /// <summary>
    /// An <c>IRequestHandler&lt;TRequest&gt;</c> implementation for void requests, registered scoped.
    /// </summary>
    VoidRequest,

    /// <summary>
    /// An <c>INotificationHandler&lt;TNotification&gt;</c> implementation, registered transient.
    /// </summary>
    Notification,
}

/// <summary>
/// Represents a diagnostic to report instead of generating source for a type.
/// </summary>
internal readonly struct DiagnosticModel : IEquatable<DiagnosticModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticModel"/> struct.
    /// </summary>
    /// <param name="descriptor">The diagnostic descriptor.</param>
    /// <param name="location">The location to report the diagnostic at.</param>
    /// <param name="arguments">The message format arguments.</param>
    public DiagnosticModel(DiagnosticDescriptor descriptor, Location location, params object?[] arguments)
    {
        Descriptor = descriptor;
        Location = location;
        Arguments = arguments;
    }

    /// <summary>
    /// Gets the diagnostic descriptor.
    /// </summary>
    public DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the location to report the diagnostic at.
    /// </summary>
    public Location Location { get; }

    /// <summary>
    /// Gets the message format arguments.
    /// </summary>
    public object?[] Arguments { get; }

    /// <inheritdoc/>
    public bool Equals(DiagnosticModel other) =>
        ReferenceEquals(Descriptor, other.Descriptor) && ReferenceEquals(Location, other.Location);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DiagnosticModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Descriptor.GetHashCode() ^ Location.GetHashCode();
}

/// <summary>
/// Represents a discovered handler implementation and everything needed to register it in the
/// generated code. Equality deliberately excludes the <see cref="Location"/> so that partial
/// declarations of the same class deduplicate in the incremental pipeline.
/// </summary>
internal readonly struct HandlerModel : IEquatable<HandlerModel>
{
    private HandlerModel(
        HandlerKind kind,
        string handlerFullyQualifiedName,
        string interfaceFullyQualifiedName,
        string typeName,
        Location location,
        DiagnosticModel? diagnostic)
    {
        Kind = kind;
        HandlerFullyQualifiedName = handlerFullyQualifiedName;
        InterfaceFullyQualifiedName = interfaceFullyQualifiedName;
        TypeName = typeName;
        Location = location;
        Diagnostic = diagnostic;
    }

    /// <summary>
    /// Creates a model that produces generated source.
    /// </summary>
    /// <param name="kind">The kind of handler contract the type implements.</param>
    /// <param name="handlerFullyQualifiedName">The fully qualified name of the handler class, including <c>global::</c>.</param>
    /// <param name="interfaceFullyQualifiedName">The fully qualified name of the closed handler interface, including <c>global::</c>.</param>
    /// <param name="typeName">The simple name of the handler class, used in messages.</param>
    /// <param name="location">The location of the type declaration, used to anchor diagnostics.</param>
    /// <returns>The created model.</returns>
    public static HandlerModel Create(
        HandlerKind kind,
        string handlerFullyQualifiedName,
        string interfaceFullyQualifiedName,
        string typeName,
        Location location) =>
        new(kind, handlerFullyQualifiedName, interfaceFullyQualifiedName, typeName, location, null);

    /// <summary>
    /// Creates a model that produces a diagnostic instead of source.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to report for the type.</param>
    /// <param name="typeName">The simple name of the type, used in messages.</param>
    /// <returns>The created model.</returns>
    public static HandlerModel CreateDiagnostic(DiagnosticModel diagnostic, string typeName) =>
        new(HandlerKind.RequestResponse, string.Empty, string.Empty, typeName, diagnostic.Location, diagnostic);

    /// <summary>
    /// Gets the kind of handler contract the type implements.
    /// </summary>
    public HandlerKind Kind { get; }

    /// <summary>
    /// Gets the fully qualified name of the handler class, including <c>global::</c>.
    /// </summary>
    public string HandlerFullyQualifiedName { get; }

    /// <summary>
    /// Gets the fully qualified name of the closed handler interface, including <c>global::</c>.
    /// </summary>
    public string InterfaceFullyQualifiedName { get; }

    /// <summary>
    /// Gets the simple name of the handler class, used in messages.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the location of the type declaration, used to anchor diagnostics.
    /// </summary>
    public Location Location { get; }

    /// <summary>
    /// Gets the diagnostic to report instead of generating source, or <see langword="null"/> when
    /// source should be generated for the type.
    /// </summary>
    public DiagnosticModel? Diagnostic { get; }

    /// <summary>
    /// Gets a value indicating whether this model carries a diagnostic instead of source.
    /// </summary>
    public bool IsDiagnostic => Diagnostic is not null;

    /// <inheritdoc/>
    public bool Equals(HandlerModel other) =>
        Kind == other.Kind
        && string.Equals(HandlerFullyQualifiedName, other.HandlerFullyQualifiedName, StringComparison.Ordinal)
        && string.Equals(InterfaceFullyQualifiedName, other.InterfaceFullyQualifiedName, StringComparison.Ordinal)
        && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
        && Diagnostic.Equals(other.Diagnostic);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is HandlerModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = (int)Kind;
        hash = (hash * 397) ^ HandlerFullyQualifiedName.GetHashCode();
        hash = (hash * 397) ^ InterfaceFullyQualifiedName.GetHashCode();
        hash = (hash * 397) ^ TypeName.GetHashCode();
        return (hash * 397) ^ Diagnostic.GetHashCode();
    }
}
