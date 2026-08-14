using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.Generators;

namespace RA.Utilities.Integrations.Tests.Generators;

/// <summary>
/// Shared host for running the request parameter source generator over in-memory compilations.
/// </summary>
internal static class GeneratorTestHost
{
    /// <summary>
    /// The sources of the marker attributes, inlined so that generator tests are self-contained.
    /// </summary>
    public const string AttributeSources = """
        namespace RA.Utilities.Integrations.Attributes
        {
            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            public sealed class QueryParametersAttribute : System.Attribute
            {
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            public sealed class HeaderParametersAttribute : System.Attribute
            {
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
            public sealed class QueryParameterNameAttribute : System.Attribute
            {
                public QueryParameterNameAttribute(string name) => Name = name;
                public string Name { get; }
            }

            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
            public sealed class HeaderParameterNameAttribute : System.Attribute
            {
                public HeaderParameterNameAttribute(string name) => Name = name;
                public string Name { get; }
            }
        }
        """;

    /// <summary>
    /// Runs the generator over the given sources and returns the resulting compilation and driver run result.
    /// </summary>
    /// <param name="sources">The input sources, including the marker attributes and the marked classes.</param>
    /// <returns>The output compilation and the driver run result.</returns>
    public static (Compilation OutputCompilation, GeneratorDriverRunResult RunResult) RunGenerator(params string[] sources)
    {
        Compilation compilation = CreateCompilation(sources);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new RequestParameterGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out _);

        return (outputCompilation, driver.GetRunResult());
    }

    /// <summary>
    /// Runs the generator, emits the output compilation and loads the resulting assembly.
    /// </summary>
    /// <param name="sources">The input sources, including the marker attributes and the marked classes.</param>
    /// <returns>The loaded assembly.</returns>
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
    /// Creates an in-memory compilation with the runtime assemblies and the Integrations assembly referenced.
    /// </summary>
    /// <param name="sources">The input sources.</param>
    /// <returns>The compilation.</returns>
    private static CSharpCompilation CreateCompilation(string[] sources)
    {
        SyntaxTree[] syntaxTrees = sources.Select(source => CSharpSyntaxTree.ParseText(source)).ToArray();

        return CSharpCompilation.Create(
            "RA.Utilities.Integrations.Generators.TestAssembly",
            syntaxTrees,
            GetMetadataReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    /// <summary>
    /// Gets the metadata references for the in-memory compilation: all trusted platform assemblies
    /// (the runtime implementation assemblies) plus the Integrations assembly.
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

        yield return MetadataReference.CreateFromFile(typeof(IQueryStringRequest).Assembly.Location);
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
