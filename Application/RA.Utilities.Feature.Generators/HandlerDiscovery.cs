using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using RA.Utilities.Feature.Generators.Models;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// Shared discovery logic that turns type declarations into <see cref="HandlerModel"/>s for
/// handlers and pipeline behaviors. Both the handler registration generator and the mediator
/// implementation generator reuse this transform (each wires its own syntax provider, since
/// providers cannot be shared across generator instances).
/// </summary>
internal static class HandlerDiscovery
{
    /// <summary>
    /// Transforms a type declaration into the handler/behavior models it implements, or diagnostic
    /// models when the implementation cannot be used by generated code.
    /// </summary>
    /// <param name="context">The generator syntax context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The discovered handler models.</returns>
    public static ImmutableArray<HandlerModel> Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken)
                is not INamedTypeSymbol typeSymbol
            || typeSymbol.IsAbstract
            || typeSymbol.IsStatic)
        {
            return [];
        }

        Compilation compilation = context.SemanticModel.Compilation;
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
            return [];
        }

        // AllInterfaces (rather than only directly implemented interfaces) ensures that concrete
        // classes deriving from an abstract handler base are registered as themselves. The match
        // runs before the class-level validation below so that unrelated types are never flagged.
        ImmutableArray<(HandlerKind Kind, INamedTypeSymbol Interface)>.Builder matched =
            ImmutableArray.CreateBuilder<(HandlerKind, INamedTypeSymbol)>();

        foreach (INamedTypeSymbol @interface in typeSymbol.AllInterfaces)
        {
            INamedTypeSymbol definition = @interface.OriginalDefinition;

            if (SymbolEqualityComparer.Default.Equals(definition, requestResponseHandlerInterface))
            {
                matched.Add((HandlerKind.RequestResponse, @interface));
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, voidRequestHandlerInterface))
            {
                matched.Add((HandlerKind.VoidRequest, @interface));
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, notificationHandlerInterface))
            {
                matched.Add((HandlerKind.Notification, @interface));
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, requestResponseBehaviorInterface))
            {
                matched.Add((HandlerKind.PipelineBehavior, @interface));
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, voidRequestBehaviorInterface))
            {
                matched.Add((HandlerKind.PipelineBehavior, @interface));
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, notificationBehaviorInterface))
            {
                matched.Add((HandlerKind.NotificationBehavior, @interface));
            }
        }

        if (matched.Count == 0)
        {
            return [];
        }

        // Class-level validation for actual implementations: generic types cannot be referenced
        // without a type argument (explicit registration remains available), structs cannot be
        // registered as class services, and inaccessible types cannot be referenced by the
        // generated code at all.
        if (typeSymbol.TypeKind == TypeKind.Struct)
        {
            return
            [
                HandlerModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.StructHandlerNotSupported,
                    context.Node.GetLocation(),
                    typeSymbol.Name), typeSymbol.Name),
            ];
        }

        if (typeSymbol.TypeParameters.Length > 0)
        {
            // Generic behaviors cannot be injected by the generated mediator; warn so that the
            // consumer knows the behavior only applies through explicit registration.
            bool isBehavior = matched.Any(static pair =>
                pair.Kind is HandlerKind.PipelineBehavior or HandlerKind.NotificationBehavior);

            DiagnosticModel diagnostic = isBehavior
                ? new DiagnosticModel(
                    Diagnostics.GenericBehaviorNotInjected,
                    context.Node.GetLocation(),
                    typeSymbol.Name)
                : new DiagnosticModel(
                    Diagnostics.GenericHandlerNotSupported,
                    context.Node.GetLocation(),
                    typeSymbol.Name);

            return
            [
                HandlerModel.CreateDiagnostic(diagnostic, typeSymbol.Name),
            ];
        }

        if (!SymbolUtilities.IsAccessibleToGeneratedCode(typeSymbol))
        {
            return
            [
                HandlerModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.HandlerNotAccessible,
                    context.Node.GetLocation(),
                    typeSymbol.Name), typeSymbol.Name),
            ];
        }

        ImmutableArray<HandlerModel>.Builder models = ImmutableArray.CreateBuilder<HandlerModel>();

        foreach ((HandlerKind kind, INamedTypeSymbol @interface) in matched)
        {
            // Notification handlers carry no request type in the registration model; every other
            // contract (handlers and behaviors) names the message it applies to.
            string requestFullyQualifiedName = kind == HandlerKind.Notification
                ? string.Empty
                : @interface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            models.Add(HandlerModel.Create(
                kind,
                SymbolUtilities.GetFullyQualifiedName(typeSymbol),
                @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                requestFullyQualifiedName,
                typeSymbol.Name,
                context.Node.GetLocation()));
        }

        return models.ToImmutable();
    }
}
