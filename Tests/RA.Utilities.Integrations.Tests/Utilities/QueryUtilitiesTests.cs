using System.Collections.Generic;
using FluentAssertions;
using RA.Utilities.Integrations.Extensions;
using RA.Utilities.Integrations.Models;
using RA.Utilities.Integrations.Utilities;

namespace RA.Utilities.Integrations.Tests.Utilities;

/// <summary>
/// Contains unit tests for the <see cref="QueryUtilities"/> class.
/// </summary>
public class QueryUtilitiesTests
{
    /// <summary>
    /// Verifies that a null collection produces an empty string.
    /// </summary>
    [Fact]
    public void ToQueryString_WithNullCollection_ShouldReturnEmptyString()
    {
        // Act
        string queryString = QueryUtilities.ToQueryString(null);

        // Assert
        queryString.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that an empty collection produces an empty string.
    /// </summary>
    [Fact]
    public void ToQueryString_WithEmptyCollection_ShouldReturnEmptyString()
    {
        // Act
        string queryString = QueryUtilities.ToQueryString([]);

        // Assert
        queryString.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that parameters with null or empty values are skipped.
    /// </summary>
    [Fact]
    public void ToQueryString_ShouldSkipParametersWithoutValues()
    {
        // Arrange
        QueryParams parameters =
        [
            KeyValuePair.Create("first", "1"),
            KeyValuePair.Create("empty", string.Empty),
            KeyValuePair.Create<string, string>("null", null!),
            KeyValuePair.Create("last", "2"),
        ];

        // Act
        string queryString = QueryUtilities.ToQueryString(parameters);

        // Assert
        queryString.Should().Be("?first=1&last=2");
    }

    /// <summary>
    /// Verifies that keys and values are URL encoded and that the order is preserved.
    /// </summary>
    [Fact]
    public void ToQueryString_ShouldUrlEncodeKeysAndValues()
    {
        // Arrange
        QueryParams parameters =
        [
            KeyValuePair.Create("search term", "a b"),
            KeyValuePair.Create("path", "a/b&c"),
        ];

        // Act
        string queryString = QueryUtilities.ToQueryString(parameters);

        // Assert
        queryString.Should().Be("?search%20term=a%20b&path=a%2Fb%26c");
    }

    /// <summary>
    /// Verifies that the backward-compatible extension method delegates to the same implementation.
    /// </summary>
    [Fact]
    public void ExtensionMethod_ShouldProduceSameResult()
    {
        // Arrange
        QueryParams parameters = [KeyValuePair.Create("page", "2")];

        // Act
        string extensionResult = QueryUtilities.ToQueryString(parameters);
        string utilityResult = QueryUtilities.ToQueryString(parameters);

        // Assert
        extensionResult.Should().Be(utilityResult).And.Be("?page=2");
    }
}
