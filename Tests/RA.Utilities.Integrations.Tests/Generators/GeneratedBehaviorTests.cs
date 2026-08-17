using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.Models;

namespace RA.Utilities.Integrations.Tests.Generators;

/// <summary>
/// Contains behavior tests that compile marked classes with the generator, load the resulting
/// assembly and verify the generated mapping at runtime.
/// </summary>
public class GeneratedBehaviorTests
{
    /// <summary>
    /// The input sources: the marker attributes, plus a query and a header parameter class
    /// exercising every supported property shape.
    /// </summary>
    private const string InputSources = """
        namespace Sample;

        [RA.Utilities.Integrations.Attributes.QueryParametersAttribute]
        public partial class SearchQuery
        {
            public int Page { get; set; }
            public int? CategoryId { get; set; }
            public string? Term { get; set; }
            public bool Featured { get; set; }
            public bool? Active { get; set; }
            public double Score { get; set; }
            public System.DateTimeOffset UpdatedSince { get; set; }
            public System.DayOfWeek Weekday { get; set; }
            public int[] Ids { get; set; } = System.Array.Empty<int>();
            public System.Collections.Generic.List<string?> Tags { get; set; } = new();
            public System.Collections.Generic.Dictionary<string, string> Filters { get; set; } = new();

            [RA.Utilities.Integrations.Attributes.QueryParameterNameAttribute("sort_by")]
            public string? SortBy { get; set; }
        }

        [RA.Utilities.Integrations.Attributes.HeaderParametersAttribute]
        public partial class ApiHeaders
        {
            [RA.Utilities.Integrations.Attributes.HeaderParameterNameAttribute("x-request-id")]
            public string? XCorrelationId { get; set; }
            public string? AcceptLanguage { get; set; }
            public int MaxRetries { get; set; }
            public System.Collections.Generic.Dictionary<string, string> Metadata { get; set; } = new();
        }
        """;

    /// <summary>
    /// The compiled and loaded test assembly.
    /// </summary>
    private static readonly Lazy<Assembly> TestAssembly = new(() =>
        GeneratorTestHost.CompileAndLoad(GeneratorTestHost.AttributeSources, InputSources));

    /// <summary>
    /// The expected query string values for the property-shape test.
    /// </summary>
    private static readonly string[] ExpectedQueryValues =
    [
        "Page=2",
        "Term=red widget",
        "Featured=true",
        "Active=false",
        "Score=1.5",
        "UpdatedSince=2026-08-14T10:30:00.0000000+02:00",
        "Weekday=Friday",
        "Ids=1",
        "Ids=2",
        "Tags=x",
        "Tags=y",
        "brand=acme",
        "sort_by=name",
    ];

    /// <summary>
    /// The identifiers used by the property-shape test.
    /// </summary>
    private static readonly int[] ExpectedIds = [1, 2];

    /// <summary>
    /// Verifies that every property shape is mapped into the query string values correctly,
    /// and that null values are skipped.
    /// </summary>
    [Fact]
    public void QueryStringValues_ShouldMapAllPropertyShapes_AndSkipNullValues()
    {
        // Arrange
        IQueryStringRequest query = CreateInstance<IQueryStringRequest>("Sample.SearchQuery");

        SetProperty(query, "Page", 2);
        SetProperty(query, "Term", "red widget");
        SetProperty(query, "Featured", true);
        SetProperty(query, "Active", false);
        SetProperty(query, "Score", 1.5);
        SetProperty(query, "UpdatedSince", new DateTimeOffset(2026, 8, 14, 10, 30, 0, TimeSpan.FromHours(2)));
        SetProperty(query, "Weekday", DayOfWeek.Friday);
        SetProperty(query, "Ids", ExpectedIds);
        SetProperty(query, "Tags", new List<string?> { "x", null, "y" });
        SetProperty(query, "Filters", new Dictionary<string, string> { ["brand"] = "acme" });
        SetProperty(query, "SortBy", "name");

        // Act
        QueryParams values = query.QueryStringValues();

        // Assert
        // Projected to a string sequence so that FluentAssertions compares in order and keeps
        // duplicate keys, instead of treating the KeyValuePair sequence with dictionary semantics.
        values.Select(pair => $"{pair.Key}={pair.Value}").Should().Equal(ExpectedQueryValues);
    }

    /// <summary>
    /// Verifies that the query string is URL encoded and that null values are skipped.
    /// Non-nullable value types have no null state, so they are always included.
    /// </summary>
    [Fact]
    public void ToQueryString_ShouldEncodeValues_AndSkipNullValues()
    {
        // Arrange
        IQueryStringRequest query = CreateInstance<IQueryStringRequest>("Sample.SearchQuery");

        SetProperty(query, "Page", 2);
        SetProperty(query, "Term", "red widget");
        SetProperty(query, "UpdatedSince", new DateTimeOffset(2026, 8, 14, 10, 30, 0, TimeSpan.FromHours(2)));

        // Act
        string queryString = query.ToQueryString("search");

        // Assert
        queryString.Should().Be(
            "search?Page=2&Term=red%20widget&Featured=false&Score=0&UpdatedSince=2026-08-14T10%3A30%3A00.0000000%2B02%3A00&Weekday=Sunday");

        // The generated partial also exposes ToQueryString on the class itself, since default
        // interface members are not callable through class-typed references.
        MethodInfo classMethod = query.GetType().GetMethod("ToQueryString")!;
        Assert.NotNull(classMethod);
        classMethod.Invoke(query, ["search"]).Should().Be(queryString);
    }

    /// <summary>
    /// Verifies that header properties are mapped into the header dictionary and that
    /// null values are skipped.
    /// </summary>
    [Fact]
    public void ToHeaders_ShouldMapProperties_AndSkipNullValues()
    {
        // Arrange
        IHeaderRequest headers = CreateInstance<IHeaderRequest>("Sample.ApiHeaders");

        SetProperty(headers, "XCorrelationId", "trace-9");
        SetProperty(headers, "MaxRetries", 3);
        SetProperty(headers, "Metadata", new Dictionary<string, string> { ["X-Tenant"] = "tenant-1" });

        // Act
        Dictionary<string, string> values = headers.ToHeaders();

        // Assert
        values.Should().Contain(KeyValuePair.Create("x-request-id", "trace-9"));
        values.Should().Contain(KeyValuePair.Create("MaxRetries", "3"));
        values.Should().Contain(KeyValuePair.Create("X-Tenant", "tenant-1"));
        values.Should().NotContainKey("AcceptLanguage");
    }

    /// <summary>
    /// Creates an instance of the given type from the compiled test assembly and casts it
    /// to the interface implemented by the generated code.
    /// </summary>
    /// <typeparam name="TInterface">The interface to cast to.</typeparam>
    /// <param name="typeName">The full name of the type to instantiate.</param>
    /// <returns>The created instance.</returns>
    private static TInterface CreateInstance<TInterface>(string typeName)
        where TInterface : class
    {
        Type? type = TestAssembly.Value.GetType(typeName);
        Assert.NotNull(type);

        object? instance = Activator.CreateInstance(type);
        Assert.NotNull(instance);

        return (TInterface)instance;
    }

    /// <summary>
    /// Sets a public property value on the given instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to set.</param>
    private static void SetProperty(object instance, string propertyName, object? value) =>
        instance.GetType().GetProperty(propertyName)!.SetValue(instance, value);
}
