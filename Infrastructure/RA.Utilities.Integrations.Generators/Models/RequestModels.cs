using System;
using Microsoft.CodeAnalysis;
using RA.Utilities.Integrations.Generators;

namespace RA.Utilities.Integrations.Generators.Models;

/// <summary>
/// Identifies which kind of request parameter a marker class represents.
/// </summary>
internal enum ParameterKind
{
    /// <summary>
    /// A query string parameter class marked with <c>[QueryParameters]</c>.
    /// </summary>
    Query,

    /// <summary>
    /// A header parameter class marked with <c>[HeaderParameters]</c>.
    /// </summary>
    Header,
}

/// <summary>
/// Describes how a property is mapped into the generated output.
/// </summary>
internal enum PropertyKind
{
    /// <summary>
    /// A single key-value pair.
    /// </summary>
    Scalar,

    /// <summary>
    /// A sequence of values mapped to repeated keys.
    /// </summary>
    Enumerable,

    /// <summary>
    /// A dictionary mapped to one key-value pair per entry.
    /// </summary>
    Dictionary,
}

/// <summary>
/// Represents a single public instance property of a marked parameter class.
/// All equality members are implemented manually so the incremental pipeline can cache models by value.
/// </summary>
internal readonly struct PropertyModel : IEquatable<PropertyModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyModel"/> struct.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="kind">How the property is mapped into the generated output.</param>
    /// <param name="nameOverride">The parameter key override, or <see langword="null"/> to use the property name.</param>
    public PropertyModel(string name, PropertyKind kind, string? nameOverride)
    {
        Name = name;
        Kind = kind;
        NameOverride = nameOverride;
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets how the property is mapped into the generated output.
    /// </summary>
    public PropertyKind Kind { get; }

    /// <summary>
    /// Gets the parameter key override, or <see langword="null"/> to use the property name.
    /// </summary>
    public string? NameOverride { get; }

    /// <summary>
    /// Gets the parameter key emitted for the property: the override if present, otherwise the property name.
    /// </summary>
    public string ParameterName => NameOverride ?? Name;

    /// <inheritdoc/>
    public bool Equals(PropertyModel other) =>
        string.Equals(Name, other.Name, StringComparison.Ordinal)
        && Kind == other.Kind
        && string.Equals(NameOverride, other.NameOverride, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PropertyModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        ((Name.GetHashCode() * 397) ^ (int)Kind) ^ (NameOverride?.GetHashCode() ?? 0);
}

/// <summary>
/// Represents a containing type of a marked parameter class. Containing types are redeclared
/// as partial declarations in the generated code.
/// </summary>
internal readonly struct ContainingTypeModel : IEquatable<ContainingTypeModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainingTypeModel"/> struct.
    /// </summary>
    /// <param name="name">The name of the containing type.</param>
    /// <param name="typeParameters">The names of the type parameters of the containing type.</param>
    /// <param name="isRecord">Whether the containing type is a record.</param>
    public ContainingTypeModel(string name, EquatableArray<string> typeParameters, bool isRecord)
    {
        Name = name;
        TypeParameters = typeParameters;
        IsRecord = isRecord;
    }

    /// <summary>
    /// Gets the name of the containing type.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the names of the type parameters of the containing type.
    /// </summary>
    public EquatableArray<string> TypeParameters { get; }

    /// <summary>
    /// Gets a value indicating whether the containing type is a record.
    /// </summary>
    public bool IsRecord { get; }

    /// <inheritdoc/>
    public bool Equals(ContainingTypeModel other) =>
        string.Equals(Name, other.Name, StringComparison.Ordinal)
        && TypeParameters.Equals(other.TypeParameters)
        && IsRecord == other.IsRecord;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ContainingTypeModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (Name.GetHashCode() * 397) ^ TypeParameters.GetHashCode() ^ (IsRecord ? 1 : 0);
}

/// <summary>
/// Represents a diagnostic to report instead of generating source.
/// </summary>
internal readonly struct DiagnosticModel : IEquatable<DiagnosticModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticModel"/> struct.
    /// </summary>
    /// <param name="descriptor">The diagnostic descriptor.</param>
    /// <param name="location">The location to report the diagnostic at.</param>
    public DiagnosticModel(DiagnosticDescriptor descriptor, Location location)
    {
        Descriptor = descriptor;
        Location = location;
    }

    /// <summary>
    /// Gets the diagnostic descriptor.
    /// </summary>
    public DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the location to report the diagnostic at.
    /// </summary>
    public Location Location { get; }

    /// <inheritdoc/>
    public bool Equals(DiagnosticModel other) =>
        ReferenceEquals(Descriptor, other.Descriptor) && ReferenceEquals(Location, other.Location);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DiagnosticModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Descriptor.GetHashCode() ^ Location.GetHashCode();
}

/// <summary>
/// Represents a marked parameter class and everything needed to generate its partial part.
/// </summary>
internal readonly struct RequestModel : IEquatable<RequestModel>
{
    private RequestModel(
        ParameterKind parameterKind,
        string? @namespace,
        EquatableArray<ContainingTypeModel> containingTypes,
        string typeName,
        EquatableArray<string> typeParameters,
        bool isRecord,
        EquatableArray<PropertyModel> properties,
        DiagnosticModel? diagnostic)
    {
        ParameterKind = parameterKind;
        Namespace = @namespace;
        ContainingTypes = containingTypes;
        TypeName = typeName;
        TypeParameters = typeParameters;
        IsRecord = isRecord;
        Properties = properties;
        Diagnostic = diagnostic;
    }

    /// <summary>
    /// Creates a model that produces generated source.
    /// </summary>
    /// <param name="parameterKind">The kind of parameter class.</param>
    /// <param name="namespace">The namespace of the class, or <see langword="null"/> for the global namespace.</param>
    /// <param name="containingTypes">The containing types of the class, outermost first.</param>
    /// <param name="typeName">The name of the class.</param>
    /// <param name="typeParameters">The names of the type parameters of the class.</param>
    /// <param name="isRecord">Whether the class is a record.</param>
    /// <param name="properties">The public instance properties of the class.</param>
    /// <returns>The created model.</returns>
    public static RequestModel Create(
        ParameterKind parameterKind,
        string? @namespace,
        EquatableArray<ContainingTypeModel> containingTypes,
        string typeName,
        EquatableArray<string> typeParameters,
        bool isRecord,
        EquatableArray<PropertyModel> properties) =>
        new(parameterKind, @namespace, containingTypes, typeName, typeParameters, isRecord, properties, null);

    /// <summary>
    /// Creates a model that produces a diagnostic instead of source.
    /// </summary>
    /// <param name="parameterKind">The kind of parameter class.</param>
    /// <param name="descriptor">The diagnostic descriptor.</param>
    /// <param name="location">The location to report the diagnostic at.</param>
    /// <param name="typeName">The name of the class, used as the diagnostic message argument.</param>
    /// <returns>The created model.</returns>
    public static RequestModel CreateDiagnostic(
        ParameterKind parameterKind,
        DiagnosticDescriptor descriptor,
        Location location,
        string typeName) =>
        new(parameterKind, null, default, typeName, default, false, default, new DiagnosticModel(descriptor, location));

    /// <summary>
    /// Gets the kind of parameter class.
    /// </summary>
    public ParameterKind ParameterKind { get; }

    /// <summary>
    /// Gets the namespace of the class, or <see langword="null"/> for the global namespace.
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets the containing types of the class, outermost first.
    /// </summary>
    public EquatableArray<ContainingTypeModel> ContainingTypes { get; }

    /// <summary>
    /// Gets the name of the class.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the names of the type parameters of the class.
    /// </summary>
    public EquatableArray<string> TypeParameters { get; }

    /// <summary>
    /// Gets a value indicating whether the class is a record.
    /// </summary>
    public bool IsRecord { get; }

    /// <summary>
    /// Gets the public instance properties of the class.
    /// </summary>
    public EquatableArray<PropertyModel> Properties { get; }

    /// <summary>
    /// Gets the diagnostic to report instead of generating source, or <see langword="null"/> when source should be generated.
    /// </summary>
    public DiagnosticModel? Diagnostic { get; }

    /// <summary>
    /// Gets a value indicating whether this model carries a diagnostic instead of source.
    /// </summary>
    public bool IsDiagnostic => Diagnostic is not null;

    /// <inheritdoc/>
    public bool Equals(RequestModel other) =>
        ParameterKind == other.ParameterKind
        && string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
        && ContainingTypes.Equals(other.ContainingTypes)
        && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
        && TypeParameters.Equals(other.TypeParameters)
        && IsRecord == other.IsRecord
        && Properties.Equals(other.Properties)
        && Diagnostic.Equals(other.Diagnostic);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is RequestModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = (int)ParameterKind;
        hash = (hash * 397) ^ (Namespace?.GetHashCode() ?? 0);
        hash = (hash * 397) ^ ContainingTypes.GetHashCode();
        hash = (hash * 397) ^ TypeName.GetHashCode();
        hash = (hash * 397) ^ TypeParameters.GetHashCode();
        hash = (hash * 397) ^ (IsRecord ? 1 : 0);
        hash = (hash * 397) ^ Properties.GetHashCode();
        return (hash * 397) ^ Diagnostic.GetHashCode();
    }
}
