using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RA.Utilities.Feature.Generators.Models;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// The combined inputs of the mediator implementation generator.
/// </summary>
internal readonly struct MediatorInputs : IEquatable<MediatorInputs>
{
    public MediatorInputs(
        EquatableArray<MessageModel> sourceMessages,
        EquatableArray<HandlerModel> sourceHandlers,
        ReferencedAssemblyModels referenced,
        bool contractsPresent,
        bool moduleInitializersSupported,
        bool initializerDisabled)
    {
        SourceMessages = sourceMessages;
        SourceHandlers = sourceHandlers;
        Referenced = referenced;
        ContractsPresent = contractsPresent;
        ModuleInitializersSupported = moduleInitializersSupported;
        InitializerDisabled = initializerDisabled;
    }

    public EquatableArray<MessageModel> SourceMessages { get; }

    public EquatableArray<HandlerModel> SourceHandlers { get; }

    public ReferencedAssemblyModels Referenced { get; }

    public bool ContractsPresent { get; }

    public bool ModuleInitializersSupported { get; }

    public bool InitializerDisabled { get; }

    /// <inheritdoc/>
    public bool Equals(MediatorInputs other) =>
        SourceMessages.Equals(other.SourceMessages)
        && SourceHandlers.Equals(other.SourceHandlers)
        && Referenced.Equals(other.Referenced)
        && ContractsPresent == other.ContractsPresent
        && ModuleInitializersSupported == other.ModuleInitializersSupported
        && InitializerDisabled == other.InitializerDisabled;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is MediatorInputs other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = SourceMessages.GetHashCode();
        hash = (hash * 397) ^ SourceHandlers.GetHashCode();
        hash = (hash * 397) ^ Referenced.GetHashCode();
        hash = (hash * 397) ^ ContractsPresent.GetHashCode();
        hash = (hash * 397) ^ ModuleInitializersSupported.GetHashCode();
        return (hash * 397) ^ InitializerDisabled.GetHashCode();
    }
}

/// <summary>
/// An incremental source generator that discovers the message types
/// (<c>IRequest</c>, <c>IRequest&lt;TResponse&gt;</c>, and <c>INotification</c> implementations)
/// visible to the compilation — including those declared in referenced class libraries — and
/// emits a <c>Mediator</c> class implementing <c>IMediator</c> with monomorphized per-message
/// dispatch methods. A module initializer queues the DI registration so that
/// <c>services.AddMediator()</c> resolves <c>IMediator</c> to the generated implementation.
/// </summary>
[Generator]
public sealed class MediatorementationGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Guard against the generator running in a compilation that does not reference the runtime
        // contracts (which only happens if the analyzer is used without the RA.Utilities.Feature
        // package that ships it); without the contracts the generated code could not compile.
        IncrementalValueProvider<bool> runtimeContractsPresent = context.CompilationProvider
            .Select(static (compilation, _) => HasRuntimeContracts(compilation));

        // Module initializers require .NET 5 or later; on older frameworks report a warning
        // instead of silently emitting code that cannot run.
        IncrementalValueProvider<bool> moduleInitializersSupported = context.CompilationProvider
            .Select(static (compilation, _) =>
                compilation.GetTypeByMetadataName(KnownMetadataNames.ModuleInitializerAttributeMetadataName) is not null);

        // The test seam disables the registration module initializer so test hosts can load the
        // generated assembly without polluting the process-wide registration queues.
        IncrementalValueProvider<bool> initializerDisabled = context.ParseOptionsProvider
            .Select(static (parseOptions, _) =>
                parseOptions is CSharpParseOptions csharpParseOptions
                && csharpParseOptions.PreprocessorSymbolNames.Any(
                    static symbol => string.Equals(symbol, KnownMetadataNames.DisableModuleInitializerDefine, StringComparison.Ordinal)));

        IncrementalValuesProvider<MessageModel> sourceMessages = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax { BaseList: not null },
                static (syntaxContext, cancellationToken) => MessageDiscovery.Transform(syntaxContext, cancellationToken))
            .SelectMany(static (models, _) => models);

        IncrementalValuesProvider<HandlerModel> sourceHandlers = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax { BaseList: not null },
                static (syntaxContext, cancellationToken) => HandlerDiscovery.Transform(syntaxContext, cancellationToken))
            .SelectMany(static (models, _) => models);

        IncrementalValueProvider<ReferencedAssemblyModels> referenced = context.CompilationProvider
            .Select(static (compilation, _) => ReferencedAssemblyDiscovery.Discover(compilation));

        IncrementalValueProvider<MediatorInputs> inputs = sourceMessages.Collect()
            .Combine(sourceHandlers.Collect())
            .Combine(referenced)
            .Combine(runtimeContractsPresent)
            .Combine(moduleInitializersSupported)
            .Combine(initializerDisabled)
            .Select(static (data, _) =>
                new MediatorInputs(
                    data.Left.Left.Left.Left.Left,
                    data.Left.Left.Left.Left.Right,
                    data.Left.Left.Left.Right,
                    data.Left.Left.Right,
                    data.Left.Right,
                    data.Right));

        context.RegisterSourceOutput(
            inputs,
            static (productionContext, data) => MediatorEmitter.Emit(
                productionContext,
                data.SourceMessages,
                data.SourceHandlers,
                data.Referenced,
                data.ContractsPresent,
                data.ModuleInitializersSupported,
                data.InitializerDisabled));
    }

    private static bool HasRuntimeContracts(Compilation compilation) =>
        compilation.GetTypeByMetadataName(KnownMetadataNames.IMediatorMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.MediatorRegistrationsMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.IRequestMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.IRequestOfTMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.INotificationMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.RequestResponseHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.VoidRequestHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.NotificationHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName("RA.Utilities.Feature.Models.PipelineContext`1") is not null;
}
