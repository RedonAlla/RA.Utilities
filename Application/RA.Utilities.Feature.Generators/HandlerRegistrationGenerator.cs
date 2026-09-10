using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RA.Utilities.Feature.Generators.Models;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// An incremental source generator that discovers every non-abstract class implementing
/// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c>, <c>IRequestHandler&lt;TRequest&gt;</c>, or
/// <c>INotificationHandler&lt;TNotification&gt;</c> in the referencing assembly and emits a module
/// initializer that queues their DI registrations, so <c>services.AddMediator()</c> registers them
/// without explicit configuration.
/// </summary>
[Generator]
public sealed class HandlerRegistrationGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Guard against the generator running in a compilation that does not reference the runtime
        // contracts (which only happens if the analyzer is used without the RA.Utilities.Feature
        // package that ships it); without the contracts there is nothing to discover or register.
        IncrementalValueProvider<bool> runtimeContractsPresent = context.CompilationProvider
            .Select(static (compilation, _) => HasRuntimeContracts(compilation));

        // Module initializers require .NET 5 or later; on older frameworks report a warning
        // instead of silently emitting code that cannot run.
        IncrementalValueProvider<bool> moduleInitializersSupported = context.CompilationProvider
            .Select(static (compilation, _) =>
                compilation.GetTypeByMetadataName(KnownMetadataNames.ModuleInitializerAttributeMetadataName) is not null);

        IncrementalValuesProvider<HandlerModel> handlers = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax { BaseList: not null },
                Transform)
            .SelectMany(static (models, _) => models);

        context.RegisterSourceOutput(
            handlers.Collect().Combine(runtimeContractsPresent).Combine(moduleInitializersSupported),
            static (productionContext, data) =>
            {
                if (!data.Left.Right)
                {
                    return;
                }

                if (!data.Right)
                {
                    productionContext.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.ModuleInitializersUnavailable,
                        Location.None));
                    return;
                }

                SourceEmitter.Emit(productionContext, data.Left.Left);
            });
    }

    private static bool HasRuntimeContracts(Compilation compilation) =>
        compilation.GetTypeByMetadataName(KnownMetadataNames.RequestResponseHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.VoidRequestHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.NotificationHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.HandlerRegistrationsMetadataName) is not null;

    private static ImmutableArray<HandlerModel> Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
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

        if (requestResponseHandlerInterface is null || voidRequestHandlerInterface is null || notificationHandlerInterface is null)
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
        }

        if (matched.Count == 0)
        {
            return [];
        }

        // Class-level validation for actual handler implementations: generic handlers cannot be
        // referenced without a type argument (explicit registration remains available), structs
        // cannot be registered as class services, and inaccessible types cannot be referenced by
        // the generated code at all.
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
            return
            [
                HandlerModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.GenericHandlerNotSupported,
                    context.Node.GetLocation(),
                    typeSymbol.Name), typeSymbol.Name),
            ];
        }

        if (!IsAccessibleToGeneratedCode(typeSymbol))
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
            models.Add(HandlerModel.Create(
                kind,
                GetFullyQualifiedName(typeSymbol),
                @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                typeSymbol.Name,
                context.Node.GetLocation()));
        }

        return models.ToImmutable();
    }

    /// <summary>
    /// Determines whether the generated registration code (which lives in a plain static class in the
    /// same assembly) can reference the given type.
    /// </summary>
    /// <param name="typeSymbol">The type to check, including its containing types.</param>
    /// <returns><see langword="true"/> when the type is accessible to the generated code.</returns>
    private static bool IsAccessibleToGeneratedCode(INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? current = typeSymbol; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal)
                || current.IsFileLocal)
            {
                return false;
            }
        }

        return true;
    }

    private static string GetFullyQualifiedName(INamedTypeSymbol typeSymbol)
    {
        // Containers are emitted outermost first.
        ImmutableArray<string>.Builder names = ImmutableArray.CreateBuilder<string>();
        for (INamedTypeSymbol? current = typeSymbol; current is not null; current = current.ContainingType)
        {
            names.Add(current.Name);
        }

        names.Reverse();

        string qualifiedName = string.Join(".", names);
        string? @namespace = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : typeSymbol.ContainingNamespace.ToDisplayString();

        return @namespace is null
            ? $"global::{qualifiedName}"
            : $"global::{@namespace}.{qualifiedName}";
    }
}
