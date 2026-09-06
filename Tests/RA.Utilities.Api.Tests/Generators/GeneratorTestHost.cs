using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RA.Utilities.Api.Generators;

namespace RA.Utilities.Api.Tests.Generators;

/// <summary>
/// Shared host for running the endpoint registration source generator over in-memory compilations.
/// </summary>
internal static class GeneratorTestHost
{
    /// <summary>
    /// The sources of the runtime contracts (<c>IEndpointGroup</c>, <c>IEndpoint</c>, and the empty
    /// partial registration class that the package ships), inlined so that generator tests are
    /// self-contained.
    /// </summary>
    public const string RuntimeSources = """
        namespace RA.Utilities.Api.Abstractions
        {
            public interface IEndpointGroup
            {
                static abstract string GroupName { get; }
                static abstract Microsoft.AspNetCore.Routing.RouteGroupBuilder MapGroup(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app);
            }

            public interface IEndpoint
            {
                static abstract string GroupName { get; }
                static abstract void MapEndpoint(Microsoft.AspNetCore.Routing.RouteGroupBuilder group);
            }
        }

        namespace RA.Utilities.Api.Extensions
        {
            public static partial class HttpEndpointServiceCollectionExtensions
            {
            }
        }
        """;

    /// <summary>
    /// Runs the generator over the given sources and returns the resulting compilation and driver run result.
    /// </summary>
    /// <param name="sources">The input sources, including the runtime contracts and the endpoint types.</param>
    /// <returns>The output compilation and the driver run result.</returns>
    public static (Compilation OutputCompilation, GeneratorDriverRunResult RunResult) RunGenerator(params string[] sources)
    {
        Compilation compilation = CreateCompilation(sources);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new EndpointRegistrationGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out _);

        return (outputCompilation, driver.GetRunResult());
    }

    /// <summary>
    /// Runs the generator and emits the resulting compilation into a loaded assembly, so that the
    /// generated <c>MapEndpoints</c> method can be invoked reflectively.
    /// </summary>
    /// <param name="sources">The input sources, including the runtime contracts and the endpoint types.</param>
    /// <returns>The loaded assembly containing the generated registration method.</returns>
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
    /// <c>Microsoft.AspNetCore.Routing</c>, which the generated code and the inlined contracts need).
    /// </summary>
    /// <param name="sources">The input sources.</param>
    /// <returns>The compilation.</returns>
    private static CSharpCompilation CreateCompilation(string[] sources)
    {
        SyntaxTree[] syntaxTrees = sources.Select(source => CSharpSyntaxTree.ParseText(source)).ToArray();

        return CSharpCompilation.Create(
            "RA.Utilities.Api.Generators.TestAssembly",
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
