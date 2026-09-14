using System;
using Microsoft.CodeAnalysis;

namespace RA.Utilities.Feature.Generators.Models;

/// <summary>
/// Identifies which message contract a discovered type implements.
/// </summary>
internal enum MessageKind
{
    /// <summary>
    /// An <c>IRequest</c> implementation without a response, dispatched through
    /// <c>IRequestHandler&lt;TRequest&gt;</c>.
    /// </summary>
    VoidRequest,

    /// <summary>
    /// An <c>IRequest&lt;TResponse&gt;</c> implementation, dispatched through
    /// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c>.
    /// </summary>
    RequestResponse,

    /// <summary>
    /// An <c>INotification</c> implementation, dispatched through
    /// <c>INotificationHandler&lt;TNotification&gt;</c>.
    /// </summary>
    Notification,
}

/// <summary>
/// Represents a discovered message type and everything needed to emit its monomorphic dispatch
/// overloads. Equality deliberately excludes the <see cref="Location"/> so that partial
/// declarations of the same class deduplicate in the incremental pipeline.
/// </summary>
internal readonly struct MessageModel : IEquatable<MessageModel>
{
    private MessageModel(
        MessageKind kind,
        string messageFullyQualifiedName,
        string typeName,
        string? responseFullyQualifiedName,
        bool responseIsValueType,
        bool messageIsValueType,
        Location location,
        DiagnosticModel? diagnostic)
    {
        Kind = kind;
        MessageFullyQualifiedName = messageFullyQualifiedName;
        TypeName = typeName;
        ResponseFullyQualifiedName = responseFullyQualifiedName;
        ResponseIsValueType = responseIsValueType;
        MessageIsValueType = messageIsValueType;
        Location = location;
        Diagnostic = diagnostic;
    }

    /// <summary>
    /// Creates a model that produces generated source.
    /// </summary>
    /// <param name="kind">The kind of message contract the type implements.</param>
    /// <param name="messageFullyQualifiedName">The fully qualified name of the message type, including <c>global::</c>.</param>
    /// <param name="typeName">The simple name of the message type, used in messages.</param>
    /// <param name="responseFullyQualifiedName">The fully qualified name of the response type, or <see langword="null"/> for void requests and notifications.</param>
    /// <param name="responseIsValueType">Whether the response type is a value type.</param>
    /// <param name="messageIsValueType">Whether the message type is a value type.</param>
    /// <param name="location">The location of the type declaration, used to anchor diagnostics.</param>
    /// <returns>The created model.</returns>
    public static MessageModel Create(
        MessageKind kind,
        string messageFullyQualifiedName,
        string typeName,
        string? responseFullyQualifiedName,
        bool responseIsValueType,
        bool messageIsValueType,
        Location location) =>
        new(kind, messageFullyQualifiedName, typeName, responseFullyQualifiedName, responseIsValueType, messageIsValueType, location, null);

    /// <summary>
    /// Creates a model that produces a diagnostic instead of source.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to report for the type.</param>
    /// <param name="typeName">The simple name of the type, used in messages.</param>
    /// <returns>The created model.</returns>
    public static MessageModel CreateDiagnostic(DiagnosticModel diagnostic, string typeName) =>
        new(MessageKind.VoidRequest, string.Empty, typeName, null, false, false, diagnostic.Location, diagnostic);

    /// <summary>
    /// Gets the kind of message contract the type implements.
    /// </summary>
    public MessageKind Kind { get; }

    /// <summary>
    /// Gets the fully qualified name of the message type, including <c>global::</c>.
    /// </summary>
    public string MessageFullyQualifiedName { get; }

    /// <summary>
    /// Gets the simple name of the message type, used in messages.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the fully qualified name of the response type, including <c>global::</c>, or
    /// <see langword="null"/> for void requests and notifications.
    /// </summary>
    public string? ResponseFullyQualifiedName { get; }

    /// <summary>
    /// Gets a value indicating whether the response type is a value type.
    /// </summary>
    public bool ResponseIsValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the message type is a value type.
    /// </summary>
    public bool MessageIsValueType { get; }

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
    public bool Equals(MessageModel other) =>
        Kind == other.Kind
        && string.Equals(MessageFullyQualifiedName, other.MessageFullyQualifiedName, StringComparison.Ordinal)
        && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
        && string.Equals(ResponseFullyQualifiedName, other.ResponseFullyQualifiedName, StringComparison.Ordinal)
        && ResponseIsValueType == other.ResponseIsValueType
        && MessageIsValueType == other.MessageIsValueType
        && Diagnostic.Equals(other.Diagnostic);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is MessageModel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = (int)Kind;
        hash = (hash * 397) ^ MessageFullyQualifiedName.GetHashCode();
        hash = (hash * 397) ^ TypeName.GetHashCode();
        hash = (hash * 397) ^ (ResponseFullyQualifiedName?.GetHashCode() ?? 0);
        hash = (hash * 397) ^ ResponseIsValueType.GetHashCode();
        hash = (hash * 397) ^ MessageIsValueType.GetHashCode();
        return (hash * 397) ^ Diagnostic.GetHashCode();
    }
}
