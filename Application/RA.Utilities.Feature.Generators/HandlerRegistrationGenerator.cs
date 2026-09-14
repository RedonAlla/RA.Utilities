using System;
using System.Linq;
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

        // The test seam disables the registration module initializer so test hosts can load the
        // generated assembly without polluting the process-wide registration queues.
        IncrementalValueProvider<bool> initializerDisabled = context.ParseOptionsProvider
            .Select(static (parseOptions, _) =>
                parseOptions is CSharpParseOptions csharpParseOptions
                && csharpParseOptions.PreprocessorSymbolNames.Any(
                    static symbol => string.Equals(symbol, KnownMetadataNames.DisableModuleInitializerDefine, StringComparison.Ordinal)));

        IncrementalValuesProvider<HandlerModel> handlers = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax { BaseList: not null },
                static (syntaxContext, cancellationToken) => HandlerDiscovery.Transform(syntaxContext, cancellationToken))
            .SelectMany(static (models, _) => models);

        context.RegisterSourceOutput(
            handlers.Collect().Combine(runtimeContractsPresent).Combine(moduleInitializersSupported).Combine(initializerDisabled),
            static (productionContext, data) =>
            {
                if (!data.Left.Left.Right)
                {
                    return;
                }

                if (data.Right)
                {
                    return;
                }

                if (!data.Left.Right)
                {
                    productionContext.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.ModuleInitializersUnavailable,
                        Location.None));
                    return;
                }

                SourceEmitter.Emit(productionContext, data.Left.Left.Left);
            });
    }

    private static bool HasRuntimeContracts(Compilation compilation) =>
        compilation.GetTypeByMetadataName(KnownMetadataNames.RequestResponseHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.VoidRequestHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.NotificationHandlerInterfaceMetadataName) is not null
        && compilation.GetTypeByMetadataName(KnownMetadataNames.HandlerRegistrationsMetadataName) is not null;
}
