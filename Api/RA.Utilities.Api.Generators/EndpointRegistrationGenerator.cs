using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RA.Utilities.Api.Generators.Models;

namespace RA.Utilities.Api.Generators;

/// <summary>
/// An incremental source generator that discovers every non-abstract type implementing
/// <c>RA.Utilities.Api.Abstractions.IEndpointGroup</c> or
/// <c>RA.Utilities.Api.Abstractions.IEndpoint</c> in the referencing assembly and emits the body of
/// the <c>MapEndpoints</c> registration extension method.
/// </summary>
/// <remarks>
/// The emitted code first invokes <c>MapGroup</c> on every discovered group, storing the resulting
/// <see cref="Microsoft.AspNetCore.Routing.RouteGroupBuilder"/> in a dictionary keyed by
/// <c>GroupName</c>, and then invokes <c>MapEndpoint</c> on every discovered endpoint, looking up its
/// group by name. Group-name mismatches (duplicate groups, endpoints referencing an unknown group) are
/// reported as compile-time diagnostics whenever the names are compile-time constants.
/// </remarks>
[Generator]
public sealed class EndpointRegistrationGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Guard against the generator running in a compilation that does not reference the runtime
        // contracts (which only happens if the analyzer is used without the RA.Utilities.Api package
        // that ships it); without the contracts there is nothing to discover or register.
        IncrementalValueProvider<bool> runtimeContractsPresent = context.CompilationProvider
            .Select(static (compilation, _) => HasRuntimeContracts(compilation));

        IncrementalValuesProvider<EndpointModel> groups = CreateModelsProvider(context, EndpointKind.Group);
        IncrementalValuesProvider<EndpointModel> endpoints = CreateModelsProvider(context, EndpointKind.Endpoint);

        context.RegisterSourceOutput(
            groups.Collect().Combine(endpoints.Collect()).Combine(runtimeContractsPresent),
            static (productionContext, data) =>
            {
                if (!data.Right)
                {
                    return;
                }

                SourceEmitter.Emit(productionContext, data.Left.Left, data.Left.Right);
            });
    }

    // Only types that can possibly implement an interface (they must declare a base list)
    // are worth a semantic look.
    private static IncrementalValuesProvider<EndpointModel> CreateModelsProvider(
        IncrementalGeneratorInitializationContext context,
        EndpointKind kind) =>
        context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax { BaseList: not null },
                (syntaxContext, cancellationToken) => Transform(syntaxContext, kind, cancellationToken))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value);

    private static bool HasRuntimeContracts(Compilation compilation) =>
        compilation.GetTypeByMetadataName(KnownMetadataNames.EndpointGroupInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.EndpointInterfaceMetadataName) is not null;

    private static EndpointModel? Transform(
        GeneratorSyntaxContext context,
        EndpointKind kind,
        CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken)
                is not INamedTypeSymbol { TypeKind: TypeKind.Class or TypeKind.Struct } typeSymbol
            || typeSymbol.IsAbstract
            || typeSymbol.IsStatic)
        {
            return null;
        }

        string interfaceMetadataName = kind == EndpointKind.Group
            ? KnownMetadataNames.EndpointGroupInterface
            : KnownMetadataNames.EndpointInterface;

        // Only types that implement the contract directly are registered. Types that merely inherit
        // the static members from a base class would register the same logical group or endpoint twice.
        if (!typeSymbol.Interfaces.Any(@interface =>
                @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceMetadataName))
        {
            return null;
        }

        if (typeSymbol.TypeParameters.Length > 0)
        {
            return EndpointModel.CreateDiagnostic(kind, new DiagnosticModel(
                Diagnostics.GenericTypeNotSupported,
                context.Node.GetLocation(),
                typeSymbol.Name));
        }

        if (!IsAccessibleToGeneratedCode(typeSymbol))
        {
            return EndpointModel.CreateDiagnostic(kind, new DiagnosticModel(
                Diagnostics.TypeNotAccessible,
                context.Node.GetLocation(),
                typeSymbol.Name));
        }

        string mappingMemberName = kind == EndpointKind.Group
            ? KnownMetadataNames.MapGroupMember
            : KnownMetadataNames.MapEndpointMember;

        if (!HasPublicStaticMember(typeSymbol, KnownMetadataNames.GroupNameMember, SymbolKind.Property)
            || !HasPublicStaticMember(typeSymbol, mappingMemberName, SymbolKind.Method))
        {
            return EndpointModel.CreateDiagnostic(kind, new DiagnosticModel(
                Diagnostics.StaticMembersNotAccessible,
                context.Node.GetLocation(),
                typeSymbol.Name,
                mappingMemberName));
        }

        string? groupNameConstant = TryGetConstantGroupName(typeSymbol, context.SemanticModel, cancellationToken);

        return EndpointModel.Create(
            kind,
            GetFullyQualifiedName(typeSymbol),
            typeSymbol.Name,
            groupNameConstant,
            context.Node.GetLocation());
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

    private static bool HasPublicStaticMember(INamedTypeSymbol typeSymbol, string name, SymbolKind symbolKind) =>
        typeSymbol.GetMembers(name).Any(member =>
            member.Kind == symbolKind
            && member.IsStatic
            && member.DeclaredAccessibility == Accessibility.Public);

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

    /// <summary>
    /// Extracts the compile-time constant value of the type's static <c>GroupName</c> property, so
    /// that group-name mismatches can be validated at compile time. Returns <see langword="null"/>
    /// when the property value is not a compile-time constant, in which case the name is still
    /// resolved correctly at runtime (the generated code calls the property itself, not this value).
    /// </summary>
    /// <param name="typeSymbol">The type whose <c>GroupName</c> property to evaluate.</param>
    /// <param name="semanticModel">The semantic model of the type's declaration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The constant group name, or <see langword="null"/>.</returns>
    private static string? TryGetConstantGroupName(
        INamedTypeSymbol typeSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        IPropertySymbol? groupNameProperty = typeSymbol.GetMembers(KnownMetadataNames.GroupNameMember)
            .OfType<IPropertySymbol>()
            .FirstOrDefault(property => property.IsStatic && property.DeclaredAccessibility == Accessibility.Public);

        if (groupNameProperty is null)
        {
            return null;
        }

        foreach (SyntaxReference reference in groupNameProperty.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax(cancellationToken) is not PropertyDeclarationSyntax declaration)
            {
                continue;
            }

            ExpressionSyntax? valueExpression = GetValueExpression(declaration);
            if (valueExpression is null)
            {
                continue;
            }

            SemanticModel declarationModel = ReferenceEquals(reference.SyntaxTree, semanticModel.SyntaxTree)
                ? semanticModel
                : semanticModel.Compilation.GetSemanticModel(reference.SyntaxTree);
            Optional<object?> constant = declarationModel.GetConstantValue(valueExpression, cancellationToken);

            if (constant.HasValue && constant.Value is string { Length: > 0 } value)
            {
                return value;
            }
        }

        return null;
    }

    private static ExpressionSyntax? GetValueExpression(PropertyDeclarationSyntax declaration)
    {
        if (declaration.ExpressionBody is not null)
        {
            return declaration.ExpressionBody.Expression;
        }

        if (declaration.Initializer is not null)
        {
            return declaration.Initializer.Value;
        }

        AccessorDeclarationSyntax? getter = declaration.AccessorList?.Accessors
            .FirstOrDefault(accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration));
        if (getter?.ExpressionBody is not null)
        {
            return getter.ExpressionBody.Expression;
        }

        return getter?.Body?.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault()?.Expression;
    }
}
