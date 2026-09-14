using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using RA.Utilities.Feature.Generators.Models;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// Discovers message types (types implementing <c>IRequest</c>, <c>IRequest&lt;TResponse&gt;</c>,
/// or <c>INotification</c>) declared in the current compilation and classifies them for the
/// generated mediator dispatch.
/// </summary>
internal static class MessageDiscovery
{
    /// <summary>
    /// Transforms a type declaration into a message model, or a diagnostic model when the type
    /// implements a message contract that the generated dispatch cannot represent.
    /// </summary>
    /// <param name="context">The generator syntax context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The discovered message model, or an empty array.</returns>
    public static ImmutableArray<MessageModel> Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken)
                is not INamedTypeSymbol typeSymbol
            || typeSymbol.IsAbstract
            || typeSymbol.IsStatic)
        {
            return [];
        }

        if (typeSymbol.TypeKind is not (TypeKind.Class or TypeKind.Struct))
        {
            return [];
        }

        Compilation compilation = context.SemanticModel.Compilation;
        MessageContracts? contracts = Classify(compilation, typeSymbol);

        if (contracts is null)
        {
            return [];
        }

        // A message implementing more than one contract (two distinct IRequest<TResponse>
        // interfaces, or a request contract together with INotification) cannot be keyed in the
        // per-type generated dispatch; the runtime mediator handles it.
        if (contracts.IsAmbiguous)
        {
            return
            [
                MessageModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.MessageInheritsMultipleMessageInterfaces,
                    context.Node.GetLocation(),
                    typeSymbol.Name,
                    contracts.Description), typeSymbol.Name),
            ];
        }

        if (typeSymbol.TypeParameters.Length > 0)
        {
            return
            [
                MessageModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.MessageNotSupportedByGeneratedDispatch,
                    context.Node.GetLocation(),
                    typeSymbol.Name), typeSymbol.Name),
            ];
        }

        if (!SymbolUtilities.IsAccessibleToGeneratedCode(typeSymbol))
        {
            return
            [
                MessageModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.MessageNotSupportedByGeneratedDispatch,
                    context.Node.GetLocation(),
                    typeSymbol.Name), typeSymbol.Name),
            ];
        }

        // Pointer-typed responses cannot appear in a non-unsafe generated method signature.
        if (contracts.ResponseType is { TypeKind: TypeKind.Pointer or TypeKind.FunctionPointer })
        {
            return
            [
                MessageModel.CreateDiagnostic(new DiagnosticModel(
                    Diagnostics.MessageNotSupportedByGeneratedDispatch,
                    context.Node.GetLocation(),
                    typeSymbol.Name), typeSymbol.Name),
            ];
        }

        return [CreateModel(typeSymbol, contracts, context.Node.GetLocation())];
    }

    /// <summary>
    /// Classifies the message contracts implemented by a type discovered in a referenced assembly,
    /// silently skipping types the generated code cannot use.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="typeSymbol">The candidate message type.</param>
    /// <returns>The message model, or <see langword="null"/> when the type is not usable.</returns>
    public static MessageModel? TryCreateReferencedModel(Compilation compilation, INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAbstract || typeSymbol.IsStatic || typeSymbol.TypeKind is not (TypeKind.Class or TypeKind.Struct))
        {
            return null;
        }

        MessageContracts? contracts = Classify(compilation, typeSymbol);
        if (contracts is null || contracts.IsAmbiguous)
        {
            return null;
        }

        // Cross-assembly references must be public; no diagnostic is reported because the
        // declaring assembly is not this compilation.
        if (typeSymbol.TypeParameters.Length > 0 || !SymbolUtilities.IsPubliclyAccessible(typeSymbol))
        {
            return null;
        }

        if (contracts.ResponseType is { TypeKind: TypeKind.Pointer or TypeKind.FunctionPointer })
        {
            return null;
        }

        return CreateModel(typeSymbol, contracts, Location.None);
    }

    private static MessageModel CreateModel(INamedTypeSymbol typeSymbol, MessageContracts contracts, Location location)
    {
        string messageFullyQualifiedName = SymbolUtilities.GetFullyQualifiedName(typeSymbol);

        return contracts.ResponseType is { } responseType
            ? MessageModel.Create(
                MessageKind.RequestResponse,
                messageFullyQualifiedName,
                typeSymbol.Name,
                responseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                responseType.IsValueType,
                typeSymbol.IsValueType,
                location)
            : MessageModel.Create(
                contracts.Kind,
                messageFullyQualifiedName,
                typeSymbol.Name,
                null,
                false,
                typeSymbol.IsValueType,
                location);
    }

    /// <summary>
    /// Scans the implemented interfaces for the message marker contracts and resolves the kind.
    /// </summary>
    private static MessageContracts? Classify(Compilation compilation, INamedTypeSymbol typeSymbol)
    {
        INamedTypeSymbol? requestInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.IRequestMetadataName);
        INamedTypeSymbol? requestOfTInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.IRequestOfTMetadataName);
        INamedTypeSymbol? notificationInterface = compilation.GetTypeByMetadataName(KnownMetadataNames.INotificationMetadataName);

        if (requestInterface is null || requestOfTInterface is null || notificationInterface is null)
        {
            return null;
        }

        ImmutableArray<ITypeSymbol>.Builder responseContracts = ImmutableArray.CreateBuilder<ITypeSymbol>();
        bool implementsRequest = false;
        bool implementsNotification = false;

        foreach (INamedTypeSymbol @interface in typeSymbol.AllInterfaces)
        {
            INamedTypeSymbol definition = @interface.OriginalDefinition;

            if (SymbolEqualityComparer.Default.Equals(definition, requestOfTInterface))
            {
                ITypeSymbol response = @interface.TypeArguments[0];
                if (!responseContracts.Any(existing => SymbolEqualityComparer.Default.Equals(existing, response)))
                {
                    responseContracts.Add(response);
                }
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, requestInterface))
            {
                // IRequest<TResponse> inherits IRequest, so a response message trivially shows
                // the request marker too; the response contract wins below.
                implementsRequest = true;
            }
            else if (SymbolEqualityComparer.Default.Equals(definition, notificationInterface))
            {
                implementsNotification = true;
            }
        }

        if (responseContracts.Count == 0 && !implementsRequest && !implementsNotification)
        {
            return null;
        }

        bool hasResponse = responseContracts.Count == 1;
        bool hasVoid = responseContracts.Count == 0 && implementsRequest;
        bool isAmbiguous = responseContracts.Count > 1
            || (implementsNotification && (hasResponse || hasVoid));

        string description = BuildContractsDescription(responseContracts, implementsRequest, implementsNotification);

        if (hasResponse)
        {
            return new MessageContracts(MessageKind.RequestResponse, responseContracts[0], isAmbiguous, description);
        }

        if (hasVoid)
        {
            return new MessageContracts(MessageKind.VoidRequest, null, isAmbiguous, description);
        }

        return new MessageContracts(MessageKind.Notification, null, isAmbiguous, description);
    }

    private static string BuildContractsDescription(ImmutableArray<ITypeSymbol>.Builder responseContracts, bool implementsRequest, bool implementsNotification)
    {
        var contracts = ImmutableArray.CreateBuilder<string>();

        foreach (ITypeSymbol response in responseContracts)
        {
            contracts.Add($"IRequest<{response.Name}>");
        }

        if (implementsRequest)
        {
            contracts.Add("IRequest");
        }

        if (implementsNotification)
        {
            contracts.Add("INotification");
        }

        return string.Join(", ", contracts);
    }

    /// <summary>
    /// The resolved message contract of a type.
    /// </summary>
    private sealed class MessageContracts
    {
        public MessageContracts(MessageKind kind, ITypeSymbol? responseType, bool isAmbiguous, string description)
        {
            Kind = kind;
            ResponseType = responseType;
            IsAmbiguous = isAmbiguous;
            Description = description;
        }

        /// <summary>
        /// Gets the kind of message contract.
        /// </summary>
        public MessageKind Kind { get; }

        /// <summary>
        /// Gets the response type for request/response messages, or <see langword="null"/>.
        /// </summary>
        public ITypeSymbol? ResponseType { get; }

        /// <summary>
        /// Gets a value indicating whether the message implements multiple contracts.
        /// </summary>
        public bool IsAmbiguous { get; }

        /// <summary>
        /// Gets a human-readable description of the implemented contracts, used in diagnostics.
        /// </summary>
        public string Description { get; }
    }
}
