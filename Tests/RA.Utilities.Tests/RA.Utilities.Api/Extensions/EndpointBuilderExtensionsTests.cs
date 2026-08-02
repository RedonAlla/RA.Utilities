using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using RA.Utilities.Api.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Api.Extensions;

/// <summary>
/// Contains unit tests for the <see cref="EndpointBuilderExtensions"/> class.
/// </summary>
public class EndpointBuilderExtensionsTests
{
    /// <summary>
    /// A test model class used for validation filter testing.
    /// </summary>
    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    // =================================================================
    // Validate<TModel> Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Validate_ShouldReturnRouteHandlerBuilder_ForChaining()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act - Validate<T> can be chained to the route handler builder
        RouteHandlerBuilder chained = app.MapGet("/test", () => "test").Validate<TestModel>();

        // Assert
        chained.Should().NotBeNull();
        chained.Should().BeAssignableTo<RouteHandlerBuilder>();
    }
}
