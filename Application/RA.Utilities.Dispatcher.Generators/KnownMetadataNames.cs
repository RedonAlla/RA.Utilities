namespace RA.Utilities.Dispatcher.Generators;

/// <summary>
/// Contains the fully qualified metadata names used by the generator to match the handler
/// contracts and to reference the runtime types living in the <c>RA.Utilities.Dispatcher</c> package.
/// </summary>
internal static class KnownMetadataNames
{
    /// <summary>
    /// The metadata name of the request/response handler interface, without the <c>global::</c>
    /// prefix (as required by <see cref="Microsoft.CodeAnalysis.Compilation.GetTypeByMetadataName"/>).
    /// </summary>
    public const string RequestResponseHandlerInterfaceMetadataName = "RA.Utilities.Dispatcher.Abstractions.IRequestHandler`2";

    /// <summary>
    /// The metadata name of the void request handler interface.
    /// </summary>
    public const string VoidRequestHandlerInterfaceMetadataName = "RA.Utilities.Dispatcher.Abstractions.IRequestHandler`1";

    /// <summary>
    /// The metadata name of the notification handler interface.
    /// </summary>
    public const string NotificationHandlerInterfaceMetadataName = "RA.Utilities.Dispatcher.Abstractions.INotificationHandler`1";

    /// <summary>
    /// The metadata name of the runtime registration queue the generated module initializers feed.
    /// </summary>
    public const string HandlerRegistrationsMetadataName = "RA.Utilities.Dispatcher.Generated.HandlerRegistrations";

    /// <summary>
    /// The metadata name of the mediator registration queue the generated mediator initializers feed.
    /// </summary>
    public const string MediatorRegistrationsMetadataName = "RA.Utilities.Dispatcher.Generated.MediatorRegistrations";

    /// <summary>
    /// The metadata name of the mediator interface implemented by the generated class.
    /// </summary>
    public const string IDispatcherMetadataName = "RA.Utilities.Dispatcher.Abstractions.IDispatcher";

    /// <summary>
    /// The metadata name of the request/response pipeline behavior interface.
    /// </summary>
    public const string RequestResponsePipelineBehaviorInterfaceMetadataName = "RA.Utilities.Dispatcher.Abstractions.IPipelineBehavior`2";

    /// <summary>
    /// The metadata name of the void request pipeline behavior interface.
    /// </summary>
    public const string VoidRequestPipelineBehaviorInterfaceMetadataName = "RA.Utilities.Dispatcher.Abstractions.IPipelineBehavior`1";

    /// <summary>
    /// The metadata name of the notification behavior interface.
    /// </summary>
    public const string NotificationBehaviorInterfaceMetadataName = "RA.Utilities.Dispatcher.Abstractions.INotificationBehavior`1";

    /// <summary>
    /// The metadata name of the void request marker interface.
    /// </summary>
    public const string IRequestMetadataName = "RA.Utilities.Dispatcher.Abstractions.IRequest";

    /// <summary>
    /// The metadata name of the request/response marker interface.
    /// </summary>
    public const string IRequestOfTMetadataName = "RA.Utilities.Dispatcher.Abstractions.IRequest`1";

    /// <summary>
    /// The metadata name of the notification marker interface.
    /// </summary>
    public const string INotificationMetadataName = "RA.Utilities.Dispatcher.Abstractions.INotification";

    /// <summary>
    /// The metadata name of the module initializer attribute. It exists in the BCL since .NET 5,
    /// so a missing attribute means the consumer compiles against a framework that cannot run
    /// module initializers.
    /// </summary>
    public const string ModuleInitializerAttributeMetadataName = "System.Runtime.CompilerServices.ModuleInitializerAttribute";

    /// <summary>
    /// The preprocessor symbol that, when defined in the consuming compilation, makes the mediator
    /// generator emit the <c>Mediator</c> class without the module initializer that registers
    /// it. Used by test hosts to avoid polluting the process-wide registration queues.
    /// </summary>
    public const string DisableModuleInitializerDefine = "RA_UTILITIES_DISPATCHER_DISABLE_MODULE_INITIALIZER";

    /// <summary>
    /// The namespace the generated module initializer is emitted into. It must match the namespace
    /// of the <c>HandlerRegistrations</c> runtime type shipped in the package.
    /// </summary>
    public const string GeneratedNamespace = "RA.Utilities.Dispatcher.Generated";

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
