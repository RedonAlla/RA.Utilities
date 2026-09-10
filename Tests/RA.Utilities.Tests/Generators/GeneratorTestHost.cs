using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RA.Utilities.Feature.Generators;

namespace RA.Utilities.Tests.Generators;

/// <summary>
/// Shared host for running the handler registration source generator over in-memory compilations.
/// </summary>
internal static class GeneratorTestHost
{
    /// <summary>
    /// The sources of the runtime contracts (the marker interfaces, the handler interfaces, the
    /// pipeline context type they reference, and the registration queue the generated module
    /// initializer feeds), inlined so that generator tests are self-contained.
    /// </summary>
    public const string RuntimeSources = """
        namespace RA.Utilities.Feature.Abstractions
        {
            public interface IRequest { }

            public interface IRequest<out TResponse> : IRequest { }

            public interface INotification { }

            public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
            {
                System.Threading.Tasks.Task<TResponse> HandleAsync(TRequest request, System.Threading.CancellationToken cancellationToken);

                System.Threading.Tasks.Task<TResponse> HandleAsync<TContext>(TRequest request, RA.Utilities.Feature.Models.PipelineContext<TContext> context, System.Threading.CancellationToken cancellationToken)
                    where TContext : class, new()
                    => HandleAsync(request, cancellationToken);
            }

            public interface IRequestHandler<in TRequest> where TRequest : IRequest
            {
                System.Threading.Tasks.Task HandleAsync(TRequest request, System.Threading.CancellationToken cancellationToken);

                System.Threading.Tasks.Task HandleAsync<TContext>(TRequest request, RA.Utilities.Feature.Models.PipelineContext<TContext> context, System.Threading.CancellationToken cancellationToken)
                    where TContext : class, new()
                    => HandleAsync(request, cancellationToken);
            }

            public interface INotificationHandler<in TNotification> where TNotification : INotification
            {
                System.Threading.Tasks.Task HandleAsync(TNotification notification, System.Threading.CancellationToken cancellationToken);

                System.Threading.Tasks.Task HandleAsync<TContext>(TNotification notification, RA.Utilities.Feature.Models.PipelineContext<TContext> context, System.Threading.CancellationToken cancellationToken)
                    where TContext : class, new()
                    => HandleAsync(notification, cancellationToken);
            }
        }

        namespace RA.Utilities.Feature.Models
        {
            public class PipelineContext<T> where T : class, new()
            {
                public T Data { get; } = new T();
            }
        }

        namespace RA.Utilities.Feature.Generated
        {
            public static class HandlerRegistrations
            {
                private static readonly System.Collections.Generic.List<System.Action<Microsoft.Extensions.DependencyInjection.IServiceCollection>> Registrations = new();

                public static void Add(System.Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> registration) => Registrations.Add(registration);

                public static void ApplyAll(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
                {
                    foreach (System.Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> registration in Registrations)
                    {
                        registration(services);
                    }
                }
            }
        }
        """;

    /// <summary>
    /// Runs the generator over the given sources and returns the resulting compilation and driver run result.
    /// </summary>
    /// <param name="sources">The input sources, including the runtime contracts and the handler types.</param>
    /// <returns>The output compilation and the driver run result.</returns>
    public static (Compilation OutputCompilation, GeneratorDriverRunResult RunResult) RunGenerator(params string[] sources)
    {
        Compilation compilation = CreateCompilation(sources);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new HandlerRegistrationGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out _);

        return (outputCompilation, driver.GetRunResult());
    }

    /// <summary>
    /// Runs the generator and emits the resulting compilation into a loaded assembly, so that the
    /// generated module initializer runs and its queued registrations can be applied reflectively.
    /// </summary>
    /// <param name="sources">The input sources, including the runtime contracts and the handler types.</param>
    /// <returns>The loaded assembly containing the generated module initializer.</returns>
    public static Assembly CompileAndLoad(params string[] sources)
    {
        (Compilation outputCompilation, _) = RunGenerator(sources);

        using var stream = new MemoryStream();
        Microsoft.CodeAnalysis.Emit.EmitResult emitResult = outputCompilation.Emit(stream);

        if (!emitResult.Success)
        {
            string errors = string.Join(Environment.NewLine, emitResult.Diagnostics.Select(diagnostic => diagnostic.ToString()));
            throw new InvalidOperationException($"Compilation failed:{Environment.NewLine}{errors}");
        }

        return Assembly.Load(stream.ToArray());
    }

    /// <summary>
    /// Creates an in-memory compilation referencing all trusted platform assemblies (including
    /// <c>Microsoft.Extensions.DependencyInjection.Abstractions</c>, which the generated code and
    /// the inlined contracts need).
    /// </summary>
    /// <param name="sources">The input sources.</param>
    /// <returns>The compilation.</returns>
    private static CSharpCompilation CreateCompilation(string[] sources)
    {
        SyntaxTree[] syntaxTrees = sources.Select(source => CSharpSyntaxTree.ParseText(source)).ToArray();

        return CSharpCompilation.Create(
            "RA.Utilities.Feature.Generators.TestAssembly",
            syntaxTrees,
            GetMetadataReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    /// <summary>
    /// Gets the metadata references for the in-memory compilation: all trusted platform assemblies
    /// (the runtime implementation assemblies).
    /// </summary>
    /// <returns>The metadata references.</returns>
    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        string trustedPlatformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;

        foreach (string path in trustedPlatformAssemblies.Split(Path.PathSeparator))
        {
            // Reference packs would conflict with the implementation assemblies (CS1703), and
            // the list can contain non-managed files.
            if (path.Contains("Microsoft.NETCore.App.Ref", StringComparison.Ordinal)
                || path.Contains("Microsoft.AspNetCore.App.Ref", StringComparison.Ordinal))
            {
                continue;
            }

            // The package under test ships in this test process's dependency closure, which makes it
            // a trusted platform assembly; exclude it so in-memory compilations see only the inlined
            // runtime contracts and the "no contracts referenced" scenario stays meaningful.
            if (path.Contains("RA.Utilities", StringComparison.Ordinal))
            {
                continue;
            }

            MetadataReference? reference = TryCreateMetadataReference(path);
            if (reference is not null)
            {
                yield return reference;
            }
        }
    }

    /// <summary>
    /// Creates a metadata reference for the given assembly path, or <see langword="null"/>
    /// when the file is not a managed assembly.
    /// </summary>
    /// <param name="path">The path of the assembly.</param>
    /// <returns>The metadata reference, or <see langword="null"/>.</returns>
    private static PortableExecutableReference? TryCreateMetadataReference(string path)
    {
        try
        {
            return MetadataReference.CreateFromFile(path);
        }
        catch (BadImageFormatException)
        {
            return null;
        }
    }
}
