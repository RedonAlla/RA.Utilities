using System;
using Microsoft.CodeAnalysis;

namespace RA.Utilities.Api.Generators.Models;

/// <summary>
/// Identifies which endpoint contract a discovered type implements.
/// </summary>
internal enum EndpointKind
{
    /// <summary>
    /// An <c>IEndpointGroup</c> implementation that creates a shared route group.
    /// </summary>
    Group,

    /// <summary>
    /// An <c>IEndpoint</c> implementation that maps routes into a group.
    /// </summary>
    Endpoint,
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
/// Represents a discovered <c>IEndpointGroup</c> or <c>IEndpoint</c> implementation and everything
/// needed to register it in the generated code. All equality members are implemented manually so the
/// incremental pipeline can cache models by value.
/// </summary>
internal readonly struct EndpointModel : IEquatable<EndpointModel>
{
    private EndpointModel(
        EndpointKind kind,
        string fullyQualifiedName,
        string typeName,
        string? groupNameConstant,
        Location location,
        DiagnosticModel? diagnostic)
    {
        Kind = kind;
        FullyQualifiedName = fullyQualifiedName;
        TypeName = typeName;
        GroupNameConstant = groupNameConstant;
        Location = location;
        Diagnostic = diagnostic;
    }

    /// <summary>
    /// Creates a model that produces generated source.
    /// </summary>
    /// <param name="kind">The kind of endpoint contract the type implements.</param>
    /// <param name="fullyQualifiedName">The fully qualified name of the type, including <c>global::</c>.</param>
    /// <param name="typeName">The simple name of the type, used in messages.</param>
    /// <param name="groupNameConstant">The compile-time constant value of the type's <c>GroupName</c> property, or <see langword="null"/> when it is not constant.</param>
    /// <param name="location">The location of the type declaration, used to anchor diagnostics.</param>
    /// <returns>The created model.</returns>
    public static EndpointModel Create(
        EndpointKind kind,
        string fullyQualifiedName,
        string typeName,
        string? groupNameConstant,
        Location location) =>
        new(kind, fullyQualifiedName, typeName, groupNameConstant, location, null);

    /// <summary>
    /// Creates a model that produces a diagnostic instead of source.
    /// </summary>
    /// <param name="kind">The kind of endpoint contract the type implements.</param>
    /// <param name="diagnostic">The diagnostic to report for the type.</param>
    /// <returns>The created model.</returns>
    public static EndpointModel CreateDiagnostic(EndpointKind kind, DiagnosticModel diagnostic) =>
        new(kind, string.Empty, string.Empty, null, diagnostic.Location, diagnostic);

    /// <summary>
    /// Gets the kind of endpoint contract the type implements.
    /// </summary>
    public EndpointKind Kind { get; }

    /// <summary>
    /// Gets the fully qualified name of the type, including <c>global::</c>.
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// Gets the simple name of the type, used in messages.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the compile-time constant value of the type's <c>GroupName</c> property, or
    /// <see langword="null"/> when it is not constant.
    /// </summary>
    public string? GroupNameConstant { get; }

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
    public bool Equals(EndpointModel other) =>
        Kind == other.Kind
        && string.Equals(FullyQualifiedName, other.FullyQualifiedName, StringComparison.Ordinal)
        && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
        && string.Equals(GroupNameConstant, other.GroupNameConstant, StringComparison.Ordinal)
        && ReferenceEquals(Location, other.Location)
        && Diagnostic.Equals(other.Diagnostic);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EndpointModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = (int)Kind;
        hash = (hash * 397) ^ FullyQualifiedName.GetHashCode();
        hash = (hash * 397) ^ TypeName.GetHashCode();
        hash = (hash * 397) ^ (GroupNameConstant?.GetHashCode() ?? 0);
        hash = (hash * 397) ^ Location.GetHashCode();
        return (hash * 397) ^ Diagnostic.GetHashCode();
    }
}
