using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.ExceptionHandlers;
using RA.Utilities.Api.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Api.Extensions;

/// <summary>
/// Contains unit tests for the <see cref="RaExceptionHandlingExtensions"/> class.
/// </summary>
public class RaExceptionHandlingExtensionsTests
{
    // =================================================================
    // AddRaExceptionHandling Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void AddRaExceptionHandling_ShouldRegisterGlobalExceptionHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddRaExceptionHandling();
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        IEnumerable<IExceptionHandler> handlers = provider.GetServices<IExceptionHandler>();
        handlers.Should().ContainSingle(h => h is GlobalExceptionHandler);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void AddRaExceptionHandling_ShouldReturnServiceCollection_ForFluentChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        IServiceCollection result = services.AddRaExceptionHandling();

        // Assert
        result.Should().BeSameAs(services);
    }

    // =================================================================
    // UseRaExceptionHandling Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void UseRaExceptionHandling_ShouldNotThrow_WhenCalledWithApp()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        builder.Services.AddRaExceptionHandling();
        WebApplication app = builder.Build();

        // Act
        Func<IApplicationBuilder> act = () => app.UseRaExceptionHandling();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void UseRaExceptionHandling_ShouldReturnIApplicationBuilder_ForFluentChaining()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        builder.Services.AddRaExceptionHandling();
        WebApplication app = builder.Build();

        // Act
        IApplicationBuilder result = app.UseRaExceptionHandling();

        // Assert
        result.Should().BeAssignableTo<IApplicationBuilder>();
    }
}
