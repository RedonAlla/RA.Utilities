namespace RA.Utilities.Api.Generators;

/// <summary>
/// Contains the fully qualified metadata names used by the generator to match the endpoint
/// contracts and to reference the runtime types living in the <c>RA.Utilities.Api</c> package.
/// </summary>
internal static class KnownMetadataNames
{
    /// <summary>
    /// The metadata name of the <c>IEndpointGroup</c> marker interface, without the <c>global::</c>
    /// prefix (as required by <see cref="Microsoft.CodeAnalysis.Compilation.GetTypeByMetadataName"/>).
    /// </summary>
    public const string EndpointGroupInterfaceMetadataName = "RA.Utilities.Api.Abstractions.IEndpointGroup";

    /// <summary>
    /// The metadata name of the <c>IEndpoint</c> marker interface, without the <c>global::</c> prefix
    /// (as required by <see cref="Microsoft.CodeAnalysis.Compilation.GetTypeByMetadataName"/>).
    /// </summary>
    public const string EndpointInterfaceMetadataName = "RA.Utilities.Api.Abstractions.IEndpoint";

    /// <summary>
    /// The fully qualified name of the <c>IEndpointGroup</c> marker interface, as produced by
    /// <c>SymbolDisplayFormat.FullyQualifiedFormat</c>.
    /// </summary>
    public const string EndpointGroupInterface = "global::RA.Utilities.Api.Abstractions.IEndpointGroup";

    /// <summary>
    /// The fully qualified name of the <c>IEndpoint</c> marker interface, as produced by
    /// <c>SymbolDisplayFormat.FullyQualifiedFormat</c>.
    /// </summary>
    public const string EndpointInterface = "global::RA.Utilities.Api.Abstractions.IEndpoint";

    /// <summary>
    /// The name of the group name property every implementation must declare.
    /// </summary>
    public const string GroupNameMember = "GroupName";

    /// <summary>
    /// The name of the group factory method declared by <c>IEndpointGroup</c> implementations.
    /// </summary>
    public const string MapGroupMember = "MapGroup";

    /// <summary>
    /// The name of the mapping method declared by <c>IEndpoint</c> implementations.
    /// </summary>
    public const string MapEndpointMember = "MapEndpoint";

    /// <summary>
    /// The namespace of the partial class the generated registration method is emitted into. It must
    /// match the handwritten partial declaration shipped in the <c>RA.Utilities.Api</c> package.
    /// </summary>
    public const string RegistrationNamespace = "RA.Utilities.Api.Extensions";

    /// <summary>
    /// The name of the partial class the generated registration method is emitted into. It must match
    /// the handwritten partial declaration shipped in the <c>RA.Utilities.Api</c> package.
    /// </summary>
    public const string RegistrationClassName = "HttpEndpointServiceCollectionExtensions";

    /// <summary>
    /// The name of the generated registration extension method.
    /// </summary>
    public const string MapEndpointsMethod = "MapEndpoints";
}
