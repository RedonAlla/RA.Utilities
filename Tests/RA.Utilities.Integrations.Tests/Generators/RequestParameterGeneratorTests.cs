using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using RA.Utilities.Integrations.Generators;

namespace RA.Utilities.Integrations.Tests.Generators;

/// <summary>
/// Contains unit tests for the <see cref="RequestParameterGenerator"/> incremental source generator.
/// </summary>
public class RequestParameterGeneratorTests
{
    /// <summary>
    /// Verifies that a <c>[QueryParameters]</c> class gets a generated partial part implementing
    /// <see cref="RA.Utilities.Integrations.Abstractions.IQueryStringRequest"/> that maps its properties.
    /// </summary>
    [Fact]
    public void QueryParameters_ShouldGeneratePartialImplementingIQueryStringRequest()
    {
        // Arrange
        const string source = """
            namespace Sample;

            [RA.Utilities.Integrations.Attributes.QueryParametersAttribute]
            public partial class GetProductsQuery
            {
                public int? CategoryId { get; set; }
                public string? Search { get; set; }
            }
            """;

        // Act
        (Compilation outputCompilation, GeneratorDriverRunResult runResult) = GeneratorTestHost.RunGenerator(
            GeneratorTestHost.AttributeSources,
            source);

        // Assert
        runResult.Results.Should().ContainSingle()
            .Which.GeneratedSources.Should().ContainSingle()
            .Which.HintName.Should().Be("Sample.GetProductsQuery.QueryParameters.g.cs");

        string generatedSource = runResult.Results[0].GeneratedSources[0].SourceText.ToString();

        generatedSource.Should().Contain("partial class GetProductsQuery : global::RA.Utilities.Integrations.Abstractions.IQueryStringRequest");
        generatedSource.Should().Contain("public global::RA.Utilities.Integrations.Models.QueryParams QueryStringValues()");
        generatedSource.Should().Contain("values.Add(\"CategoryId\", categoryIdValue);");
        generatedSource.Should().Contain("values.Add(\"Search\", searchValue);");

        outputCompilation.GetDiagnostics().Should().NotContain(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
    }

    /// <summary>
    /// Verifies that a <c>[HeaderParameters]</c> class gets a generated partial part implementing
    /// <see cref="RA.Utilities.Integrations.Abstractions.IHeaderRequest"/> that maps its properties.
    /// </summary>
    [Fact]
    public void HeaderParameters_ShouldGeneratePartialImplementingIHeaderRequest()
    {
        // Arrange
        const string source = """
            namespace Sample;

            [RA.Utilities.Integrations.Attributes.HeaderParametersAttribute]
            public partial class RequestHeaders
            {
                public string? XCorrelationId { get; set; }

                [RA.Utilities.Integrations.Attributes.HeaderParameterNameAttribute("x-request-id")]
                public string? TraceIdentifier { get; set; }
            }
            """;

        // Act
        (Compilation outputCompilation, GeneratorDriverRunResult runResult) = GeneratorTestHost.RunGenerator(
            GeneratorTestHost.AttributeSources,
            source);

        // Assert
        runResult.Results.Should().ContainSingle()
            .Which.GeneratedSources.Should().ContainSingle()
            .Which.HintName.Should().Be("Sample.RequestHeaders.HeaderParameters.g.cs");

        string generatedSource = runResult.Results[0].GeneratedSources[0].SourceText.ToString();

        generatedSource.Should().Contain("partial class RequestHeaders : global::RA.Utilities.Integrations.Abstractions.IHeaderRequest");
        generatedSource.Should().Contain("public global::System.Collections.Generic.Dictionary<string, string> ToHeaders()");
        generatedSource.Should().Contain("values[\"XCorrelationId\"] = xCorrelationIdValue;");
        generatedSource.Should().Contain("values[\"x-request-id\"] = traceIdentifierValue;");

        outputCompilation.GetDiagnostics().Should().NotContain(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
    }

    /// <summary>
    /// Verifies that a marked class that is not partial is reported with the RPIG001 diagnostic
    /// instead of producing broken generated code.
    /// </summary>
    [Fact]
    public void NonPartialClass_ShouldReportDiagnostic()
    {
        // Arrange
        const string source = """
            namespace Sample;

            [RA.Utilities.Integrations.Attributes.QueryParametersAttribute]
            public class GetProductsQuery
            {
                public int? CategoryId { get; set; }
            }
            """;

        // Act
        (Compilation _, GeneratorDriverRunResult runResult) = GeneratorTestHost.RunGenerator(
            GeneratorTestHost.AttributeSources,
            source);

        // Assert
        runResult.Results.Should().ContainSingle()
            .Which.GeneratedSources.Should().BeEmpty();

        ImmutableArray<Diagnostic> diagnostics = runResult.Results[0].Diagnostics;
        diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be("RPIG001");
    }

    /// <summary>
    /// Verifies that a class that already implements the interface is left untouched.
    /// </summary>
    [Fact]
    public void ClassAlreadyImplementingInterface_ShouldBeSkipped()
    {
        // Arrange
        const string source = """
            namespace Sample;

            [RA.Utilities.Integrations.Attributes.QueryParametersAttribute]
            public partial class HandWrittenQuery : RA.Utilities.Integrations.Abstractions.IQueryStringRequest
            {
                public RA.Utilities.Integrations.Models.QueryParams QueryStringValues() => new();
            }
            """;

        // Act
        (Compilation _, GeneratorDriverRunResult runResult) = GeneratorTestHost.RunGenerator(
            GeneratorTestHost.AttributeSources,
            source);

        // Assert
        runResult.Results.Should().ContainSingle()
            .Which.GeneratedSources.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that a generic record and nested classes are supported.
    /// </summary>
    [Fact]
    public void GenericRecordAndNestedClasses_ShouldGenerateValidCode()
    {
        // Arrange
        const string source = """
            namespace Sample;

            public partial class RequestModels
            {
                [RA.Utilities.Integrations.Attributes.QueryParametersAttribute]
                public partial record PagedQuery<T>(int Page) where T : class;
            }
            """;

        // Act
        (Compilation outputCompilation, GeneratorDriverRunResult runResult) = GeneratorTestHost.RunGenerator(
            GeneratorTestHost.AttributeSources,
            source);

        // Assert
        string generatedSource = runResult.Results[0].GeneratedSources[0].SourceText.ToString();

        generatedSource.Should().Contain("partial class RequestModels");
        generatedSource.Should().Contain("partial record PagedQuery<T> : global::RA.Utilities.Integrations.Abstractions.IQueryStringRequest");

        outputCompilation.GetDiagnostics().Should().NotContain(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
    }
}
