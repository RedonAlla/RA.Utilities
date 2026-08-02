using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Api.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Api.Extensions;

/// <summary>
/// Contains unit tests for the <see cref="EndpointExtensions"/> class.
/// </summary>
public class EndpointExtensionsTests
{
    /// <summary>
    /// A test endpoint implementation used for testing endpoint discovery.
    /// </summary>
    private sealed class TestEndpoint : IEndpoint
    {
        public bool WasCalled { get; private set; }

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            WasCalled = true;
        }
    }

    /// <summary>
    /// Another test endpoint to verify multiple endpoints are discovered.
    /// </summary>
    private sealed class AnotherTestEndpoint : IEndpoint
    {
        public bool WasCalled { get; private set; }

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            WasCalled = true;
        }
    }

    // =================================================================
    // AddEndpoints Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void AddEndpoints_ShouldRegisterIEndpointImplementations_FromAssembly()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddEndpoints(assembly);
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        IEnumerable<IEndpoint> endpoints = provider.GetServices<IEndpoint>();
        endpoints.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void AddEndpoints_ShouldReturnServiceCollection_ForFluentChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        IServiceCollection result = services.AddEndpoints(Assembly.GetExecutingAssembly());

        // Assert
        result.Should().BeSameAs(services);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void AddEndpoints_ShouldNotRegisterAbstractOrInterfaceTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddEndpoints(assembly);
        ServiceProvider provider = services.BuildServiceProvider();
        IEnumerable<IEndpoint> endpoints = provider.GetServices<IEndpoint>();

        // Assert - only concrete implementations should be registered
        endpoints.Should().AllSatisfy(e => e.GetType().IsAbstract.Should().BeFalse());
        endpoints.Should().AllSatisfy(e => e.GetType().IsInterface.Should().BeFalse());
    }

    // =================================================================
    // MapEndpoints Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void MapEndpoints_ShouldInvokeMapEndpoint_OnRegisteredEndpoints()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        var testEndpoint = new TestEndpoint();
        builder.Services.AddSingleton<IEndpoint>(testEndpoint);

        WebApplication app = builder.Build();

        // Act
        WebApplication result = app.MapEndpoints();

        // Assert
        testEndpoint.WasCalled.Should().BeTrue();
        result.Should().BeSameAs(app);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void MapEndpoints_ShouldInvokeAllRegisteredEndpoints()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        var endpoint1 = new TestEndpoint();
        var endpoint2 = new AnotherTestEndpoint();
        builder.Services.AddSingleton<IEndpoint>(endpoint1);
        builder.Services.AddSingleton<IEndpoint>(endpoint2);

        WebApplication app = builder.Build();

        // Act
        app.MapEndpoints();

        // Assert
        endpoint1.WasCalled.Should().BeTrue();
        endpoint2.WasCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void MapEndpoints_ShouldReturnWebApplication_ForFluentChaining()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        builder.Services.AddSingleton<IEndpoint>(new TestEndpoint());
        WebApplication app = builder.Build();

        // Act
        WebApplication result = app.MapEndpoints();

        // Assert
        result.Should().BeOfType<WebApplication>();
    }
}
