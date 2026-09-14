using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using RA.Utilities.Feature.Generators.Models;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// The messages and handlers discovered in assemblies referenced by the compilation, so that the
/// generated mediator also emits monomorphic dispatch for messages declared in class libraries.
/// </summary>
internal readonly struct ReferencedAssemblyModels : IEquatable<ReferencedAssemblyModels>
{
    public ReferencedAssemblyModels(EquatableArray<MessageModel> messages, EquatableArray<HandlerModel> handlers)
    {
        Messages = messages;
        Handlers = handlers;
    }

    /// <summary>
    /// Gets the message models discovered in referenced assemblies.
    /// </summary>
    public EquatableArray<MessageModel> Messages { get; }

    /// <summary>
    /// Gets the handler models discovered in referenced assemblies.
    /// </summary>
    public EquatableArray<HandlerModel> Handlers { get; }

    /// <inheritdoc/>
    public bool Equals(ReferencedAssemblyModels other) =>
        Messages.Equals(other.Messages) && Handlers.Equals(other.Handlers);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ReferencedAssemblyModels other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (Messages.GetHashCode() * 397) ^ Handlers.GetHashCode();
}

/// <summary>
/// Walks the assemblies referenced by the compilation looking for message and handler types, so
/// that the generated mediator covers messages declared in class libraries too. The walk is
/// prefiltered to assemblies that reference the RA.Utilities.Feature runtime assembly (a message
/// must reference it to name the marker interfaces), which keeps the walk off the BCL.
/// </summary>
internal static class ReferencedAssemblyDiscovery
{
    /// <summary>
    /// The simple assembly name of the RA.Utilities.Feature runtime package.
    /// </summary>
    private const string FeatureAssemblyName = "RA.Utilities.Feature";

    /// <summary>
    /// Discovers messages and handlers in the assemblies referenced by the compilation.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The discovered models.</returns>
    public static ReferencedAssemblyModels Discover(Compilation compilation)
    {
        ImmutableArray<MessageModel>.Builder messages = ImmutableArray.CreateBuilder<MessageModel>();
        ImmutableArray<HandlerModel>.Builder handlers = ImmutableArray.CreateBuilder<HandlerModel>();

        foreach (MetadataReference reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assembly
                || !ReferencesFeatureRuntime(assembly))
            {
                continue;
            }

            CollectTypes(assembly.GlobalNamespace, compilation, messages, handlers);
        }

        return new ReferencedAssemblyModels(messages.ToImmutable(), handlers.ToImmutable());
    }

    /// <summary>
    /// Determines whether the assembly directly references the RA.Utilities.Feature runtime
    /// assembly. Any assembly declaring message or handler types must do so, because it has to
    /// name the marker interfaces.
    /// </summary>
    private static bool ReferencesFeatureRuntime(IAssemblySymbol assembly) =>
        assembly.Modules.Any(
            static module => module.ReferencedAssemblies.Any(
                static identity => string.Equals(identity.Name, FeatureAssemblyName, StringComparison.Ordinal)));

    private static void CollectTypes(
        INamespaceSymbol @namespace,
        Compilation compilation,
        ImmutableArray<MessageModel>.Builder messages,
        ImmutableArray<HandlerModel>.Builder handlers)
    {
        foreach (INamedTypeSymbol type in @namespace.GetTypeMembers())
        {
            CollectType(type, compilation, messages, handlers);
        }

        foreach (INamespaceSymbol nestedNamespace in @namespace.GetNamespaceMembers())
        {
            CollectTypes(nestedNamespace, compilation, messages, handlers);
        }
    }

    private static void CollectType(
        INamedTypeSymbol type,
        Compilation compilation,
        ImmutableArray<MessageModel>.Builder messages,
        ImmutableArray<HandlerModel>.Builder handlers)
    {
        if (MessageDiscovery.TryCreateReferencedModel(compilation, type) is { } message)
        {
            messages.Add(message);
        }

        TryCreateReferencedHandlers(type, compilation, handlers);

        foreach (INamedTypeSymbol nestedType in type.GetTypeMembers())
        {
            CollectType(nestedType, compilation, messages, handlers);
        }
    }

    private static void TryCreateReferencedHandlers(
        INamedTypeSymbol typeSymbol,
        Compilation compilation,
        ImmutableArray<HandlerModel>.Builder handlers)
    {
        if (typeSymbol.IsAbstract || typeSymbol.IsStatic || typeSymbol.TypeParameters.Length > 0
            || !SymbolUtilities.IsPubliclyAccessible(typeSymbol))
        {
            return;
        }

        INamedTypeSymbol? requestResponseHandlerInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.RequestResponseHandlerInterfaceMetadataName);
        INamedTypeSymbol? voidRequestHandlerInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.VoidRequestHandlerInterfaceMetadataName);
        INamedTypeSymbol? notificationHandlerInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.NotificationHandlerInterfaceMetadataName);
        INamedTypeSymbol? requestResponseBehaviorInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.RequestResponsePipelineBehaviorInterfaceMetadataName);
        INamedTypeSymbol? voidRequestBehaviorInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.VoidRequestPipelineBehaviorInterfaceMetadataName);
        INamedTypeSymbol? notificationBehaviorInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.NotificationBehaviorInterfaceMetadataName);

        if (requestResponseHandlerInterface is null
            || voidRequestHandlerInterface is null
            || notificationHandlerInterface is null
            || requestResponseBehaviorInterface is null
            || voidRequestBehaviorInterface is null
            || notificationBehaviorInterface is null)
        {
            return;
        }

        foreach (INamedTypeSymbol @interface in typeSymbol.AllInterfaces)
        {
            INamedTypeSymbol definition = @interface.OriginalDefinition;
            HandlerKind? kind = null;

            if (SymbolEqualityComparer.Default.Equals(definition, requestResponseHandlerInterface))
            {
                kind = HandlerKind.RequestResponse;
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, voidRequestHandlerInterface))
            {
                kind = HandlerKind.VoidRequest;
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, notificationHandlerInterface))
            {
                kind = HandlerKind.Notification;
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, requestResponseBehaviorInterface)
                || SymbolEqualityComparer.Default.Equals(definition, voidRequestBehaviorInterface))
            {
                kind = HandlerKind.PipelineBehavior;
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, notificationBehaviorInterface))
            {
                kind = HandlerKind.NotificationBehavior;
            }

            if (kind is not { } matchedKind)
            {
                continue;
            }

            string requestFullyQualifiedName = matchedKind == HandlerKind.Notification
                ? string.Empty
                : @interface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            handlers.Add(HandlerModel.Create(
                matchedKind,
                SymbolUtilities.GetFullyQualifiedName(typeSymbol),
                @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                requestFullyQualifiedName,
                typeSymbol.Name,
                Location.None));
        }
    }
}
