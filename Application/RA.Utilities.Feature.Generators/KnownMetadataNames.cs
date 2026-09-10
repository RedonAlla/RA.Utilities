namespace RA.Utilities.Feature.Generators;

/// <summary>
/// Contains the fully qualified metadata names used by the generator to match the handler
/// contracts and to reference the runtime types living in the <c>RA.Utilities.Feature</c> package.
/// </summary>
internal static class KnownMetadataNames
{
    /// <summary>
    /// The metadata name of the request/response handler interface, without the <c>global::</c>
    /// prefix (as required by <see cref="Microsoft.CodeAnalysis.Compilation.GetTypeByMetadataName"/>).
    /// </summary>
    public const string RequestResponseHandlerInterfaceMetadataName = "RA.Utilities.Feature.Abstractions.IRequestHandler`2";

    /// <summary>
    /// The metadata name of the void request handler interface.
    /// </summary>
    public const string VoidRequestHandlerInterfaceMetadataName = "RA.Utilities.Feature.Abstractions.IRequestHandler`1";

    /// <summary>
    /// The metadata name of the notification handler interface.
    /// </summary>
    public const string NotificationHandlerInterfaceMetadataName = "RA.Utilities.Feature.Abstractions.INotificationHandler`1";

    /// <summary>
    /// The metadata name of the runtime registration queue the generated module initializers feed.
    /// </summary>
    public const string HandlerRegistrationsMetadataName = "RA.Utilities.Feature.Generated.HandlerRegistrations";

    /// <summary>
    /// The metadata name of the module initializer attribute. It exists in the BCL since .NET 5,
    /// so a missing attribute means the consumer compiles against a framework that cannot run
    /// module initializers.
    /// </summary>
    public const string ModuleInitializerAttributeMetadataName = "System.Runtime.CompilerServices.ModuleInitializerAttribute";

    /// <summary>
    /// The namespace the generated module initializer is emitted into. It must match the namespace
    /// of the <c>HandlerRegistrations</c> runtime type shipped in the package.
    /// </summary>
    public const string GeneratedNamespace = "RA.Utilities.Feature.Generated";

    /// <summary>
    /// The name of the generated module initializer class.
    /// </summary>
    public const string InitializerClassName = "HandlerRegistrationInitializer";

    /// <summary>
    /// The name of the method on the runtime <c>HandlerRegistrations</c> type that queues a
    /// registration callback.
    /// </summary>
    public const string AddMethod = "Add";

    /// <summary>
    /// The fully qualified name of the DI extension class used to register handler services.
    /// </summary>
    public const string ServiceCollectionServiceExtensions =
        "global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions";
}
