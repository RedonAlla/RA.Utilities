using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.OpenApi;
using RA.Utilities.OpenApi.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.OpenApi.Extensions;

/// <summary>
/// Tests for the <see cref="DependencyInjectionExtensions"/> class.
/// </summary>
public class DependencyInjectionExtensionsTests
{
    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddDefaultsDocumentTransformer"/> throws an <see cref="ArgumentNullException"/> when options are null.
    /// </summary>
    [Fact]
    public void AddDefaultsDocumentTransformer_WithNullOptions_ShouldThrowArgumentNullException()
    {
        Action act = () => ((OpenApiOptions)null!).AddDefaultsDocumentTransformer();
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddFluentValidationRules"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddFluentValidationRules_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        OpenApiOptions result = options.AddFluentValidationRules();
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddDefaultResponsesOperationTransformer"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddDefaultResponsesOperationTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        OpenApiOptions result = options.AddDefaultResponsesOperationTransformer();
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddDocumentInfoTransformer"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddDocumentInfoTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        OpenApiOptions result = options.AddDocumentInfoTransformer();
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddBearerSecurityDocumentTransformer"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddBearerSecurityDocumentTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        OpenApiOptions result = options.AddBearerSecurityDocumentTransformer();
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddHeadersParameterTransformer"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddHeadersParameterTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        OpenApiOptions result = options.AddHeadersParameterTransformer();
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddEnumXmlDescriptionTransformer"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddEnumXmlDescriptionTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        OpenApiOptions result = options.AddEnumXmlDescriptionTransformer("test.xml");
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddPolymorphismDocumentTransformer{TBase}"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddPolymorphismDocumentTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        var types = new Dictionary<string, Type> { ["Cat"] = typeof(object) };
        OpenApiOptions result = options.AddPolymorphismDocumentTransformer<object>(types);
        result.Should().BeSameAs(options);
    }

    /// <summary>
    /// Tests that <see cref="DependencyInjectionExtensions.AddTagOperationTransformer"/> returns the same <see cref="OpenApiOptions"/> instance.
    /// </summary>
    [Fact]
    public void AddTagOperationTransformer_ShouldReturnOptions()
    {
        var options = new OpenApiOptions();
        var tags = new Dictionary<string, string> { ["Users"] = "User operations" };
        OpenApiOptions result = options.AddTagOperationTransformer(tags);
        result.Should().BeSameAs(options);
    }
}
