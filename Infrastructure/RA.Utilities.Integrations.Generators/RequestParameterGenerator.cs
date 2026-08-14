using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RA.Utilities.Integrations.Generators.Models;

namespace RA.Utilities.Integrations.Generators;

/// <summary>
/// An incremental source generator that implements the strongly-typed query and header parameter
/// contracts for classes marked with <c>[QueryParameters]</c> and <c>[HeaderParameters]</c>.
/// </summary>
/// <remarks>
/// For each marked partial class or record the generator emits a partial declaration implementing
/// <c>RA.Utilities.Integrations.Abstractions.IQueryStringRequest</c> or
/// <c>RA.Utilities.Integrations.Abstractions.IHeaderRequest</c>, mapping each public instance
/// property to a query string or header key-value pair.
/// </remarks>
[Generator]
public sealed class RequestParameterGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterPipeline(context, KnownMetadataNames.QueryParametersAttribute, ParameterKind.Query);
        RegisterPipeline(context, KnownMetadataNames.HeaderParametersAttribute, ParameterKind.Header);
    }

    private static void RegisterPipeline(
        IncrementalGeneratorInitializationContext context,
        string attributeMetadataName,
        ParameterKind parameterKind)
    {
        IncrementalValuesProvider<RequestModel> models = context.SyntaxProvider
            .ForAttributeWithMetadataName<RequestModel?>(
                attributeMetadataName,
                static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                (generatorContext, cancellationToken) => Transform(generatorContext, parameterKind, cancellationToken))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value);

        context.RegisterSourceOutput(
            models.Where(static model => !model.IsDiagnostic),
            static (productionContext, model) => SourceEmitter.Emit(productionContext, model));

        context.RegisterSourceOutput(
            models.Where(static model => model.IsDiagnostic),
            static (productionContext, model) =>
            {
                DiagnosticModel diagnostic = model.Diagnostic!.Value;
                productionContext.ReportDiagnostic(Diagnostic.Create(diagnostic.Descriptor, diagnostic.Location, model.TypeName));
            });
    }

    private static RequestModel? Transform(
        GeneratorAttributeSyntaxContext context,
        ParameterKind parameterKind,
        CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }

        // Defensive: the marker attribute can only target classes, but guard against unusual scenarios.
        if (typeSymbol.TypeKind != TypeKind.Class)
        {
            return RequestModel.CreateDiagnostic(
                parameterKind,
                Diagnostics.UnsupportedType,
                context.TargetNode.GetLocation(),
                typeSymbol.Name);
        }

        // The class itself and every containing type must be partial, otherwise the generated
        // partial declaration cannot be merged into it (CS0260).
        for (INamedTypeSymbol? current = typeSymbol; current is not null; current = current.ContainingType)
        {
            foreach (SyntaxReference declarationReference in current.DeclaringSyntaxReferences)
            {
                SyntaxNode declarationNode = declarationReference.GetSyntax(cancellationToken);
                if (declarationNode is TypeDeclarationSyntax declaration && !IsPartial(declaration))
                {
                    return RequestModel.CreateDiagnostic(
                        parameterKind,
                        Diagnostics.NotPartial,
                        declaration.Identifier.GetLocation(),
                        current.Name);
                }
            }
        }

        // Skip types that already provide the mapping, either hand-written or inherited from a base class.
        string interfaceMetadataName = parameterKind == ParameterKind.Query
            ? KnownMetadataNames.IQueryStringRequest
            : KnownMetadataNames.IHeaderRequest;
        string methodName = parameterKind == ParameterKind.Query
            ? KnownMetadataNames.QueryStringValuesMethod
            : KnownMetadataNames.ToHeadersMethod;

        if (typeSymbol.AllInterfaces.Any(@interface =>
                @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceMetadataName)
            || typeSymbol.GetMembers(methodName).Length > 0
            || (parameterKind == ParameterKind.Query
                && typeSymbol.GetMembers(KnownMetadataNames.ToQueryStringMethod).Length > 0))
        {
            return null;
        }

        return RequestModel.Create(
            parameterKind,
            GetNamespace(typeSymbol),
            GetContainingTypes(typeSymbol),
            typeSymbol.Name,
            GetTypeParameters(typeSymbol),
            typeSymbol.IsRecord,
            GetProperties(typeSymbol, parameterKind));
    }

    private static string? GetNamespace(INamedTypeSymbol typeSymbol) =>
        typeSymbol.ContainingNamespace.IsGlobalNamespace ? null : typeSymbol.ContainingNamespace.ToDisplayString();

    private static EquatableArray<ContainingTypeModel> GetContainingTypes(INamedTypeSymbol typeSymbol)
    {
        ImmutableArray<ContainingTypeModel>.Builder builder = ImmutableArray.CreateBuilder<ContainingTypeModel>();

        for (INamedTypeSymbol? current = typeSymbol.ContainingType; current is not null; current = current.ContainingType)
        {
            builder.Add(new ContainingTypeModel(current.Name, GetTypeParameters(current), current.IsRecord));
        }

        // Containers are emitted outermost first.
        builder.Reverse();
        return builder.ToImmutable();
    }

    private static EquatableArray<string> GetTypeParameters(INamedTypeSymbol typeSymbol) =>
        typeSymbol.TypeParameters.Select(parameter => parameter.Name).ToImmutableArray();

    private static EquatableArray<PropertyModel> GetProperties(INamedTypeSymbol typeSymbol, ParameterKind parameterKind)
    {
        ImmutableArray<PropertyModel>.Builder builder = ImmutableArray.CreateBuilder<PropertyModel>();

        foreach (ISymbol member in typeSymbol.GetMembers())
        {
            if (member is not IPropertySymbol { IsStatic: false, IsIndexer: false } property
                || property.GetMethod is not { DeclaredAccessibility: Accessibility.Public })
            {
                continue;
            }

            builder.Add(new PropertyModel(
                property.Name,
                GetPropertyKind(property.Type),
                GetParameterNameOverride(property, parameterKind)));
        }

        return builder.ToImmutable();
    }

    private static string? GetParameterNameOverride(IPropertySymbol property, ParameterKind parameterKind)
    {
        string nameOverrideAttributeMetadataName = parameterKind == ParameterKind.Query
            ? KnownMetadataNames.QueryParameterNameAttribute
            : KnownMetadataNames.HeaderParameterNameAttribute;

        foreach (AttributeData attribute in property.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                != nameOverrideAttributeMetadataName)
            {
                continue;
            }

            if (attribute.ConstructorArguments.Length == 1
                && attribute.ConstructorArguments[0].Value is string { Length: > 0 } name
                && !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
        }

        return null;
    }

    private static PropertyKind GetPropertyKind(ITypeSymbol type)
    {
        // Strings implement IEnumerable<char>, so they must be treated as scalars explicitly.
        if (type.SpecialType == SpecialType.System_String)
        {
            return PropertyKind.Scalar;
        }

        if (IsOrImplements(type, KnownMetadataNames.SystemCollectionsIDictionary)
            || IsOrImplements(type, KnownMetadataNames.GenericIReadOnlyDictionary))
        {
            return PropertyKind.Dictionary;
        }

        if (IsOrImplements(type, KnownMetadataNames.SystemCollectionsIEnumerable)
            || IsOrImplements(type, KnownMetadataNames.GenericIEnumerableOfT))
        {
            return PropertyKind.Enumerable;
        }

        return PropertyKind.Scalar;
    }

    private static bool IsOrImplements(ITypeSymbol type, string fullyQualifiedOriginalDefinitionName)
    {
        if (type.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedOriginalDefinitionName)
        {
            return true;
        }

        foreach (INamedTypeSymbol @interface in type.AllInterfaces)
        {
            if (@interface.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedOriginalDefinitionName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsPartial(TypeDeclarationSyntax declaration) =>
        declaration.Modifiers.Any(SyntaxKind.PartialKeyword);
}
