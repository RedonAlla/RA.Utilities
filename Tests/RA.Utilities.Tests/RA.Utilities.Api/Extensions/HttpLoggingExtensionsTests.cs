using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using RA.Utilities.Api.Extensions;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Api.Options;

namespace RA.Utilities.Tests.RA.Utilities.Api.Extensions;

/// <summary>
/// Contains unit tests for the logging middleware configuration.
/// </summary>
public class HttpLoggingExtensionsTests
{
    // =================================================================
    // AddLoggingMiddleware Tests
    // =================================================================

    [Fact]
    public void AddLoggingMiddleware_ShouldRegisterLoggingMiddleware()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddLoggingMiddleware();
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        LoggingMiddleware middleware = provider.GetRequiredService<LoggingMiddleware>();
        middleware.Should().NotBeNull();
    }

    [Fact]
    public void AddLoggingMiddleware_ShouldRegisterRecyclableMemoryStreamManager()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddLoggingMiddleware();
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        RecyclableMemoryStreamManager manager = provider.GetRequiredService<RecyclableMemoryStreamManager>();
        manager.Should().NotBeNull();
    }

    [Fact]
    public void AddLoggingMiddleware_ShouldReturnServiceCollection_ForFluentChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        IServiceCollection result = services.AddLoggingMiddleware();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddLoggingMiddleware_WithConfigureOptions_ShouldApplySettings()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddLoggingMiddleware(options =>
        {
            options.MaxBodyLogLength = 4096;
            options.WarningThresholdMilliseconds = 1000;
            options.PathsToIgnore.Add("/health");
            options.ExcludedHeaders.Add("Authorization");
        });
        ServiceProvider provider = services.BuildServiceProvider();
        HttpLoggingOptions resolved = provider.GetRequiredService<IOptions<HttpLoggingOptions>>().Value;

        // Assert
        resolved.MaxBodyLogLength.Should().Be(4096);
        resolved.WarningThresholdMilliseconds.Should().Be(1000);
        resolved.PathsToIgnore.Should().Contain("/health");
        resolved.ExcludedHeaders.Should().Contain("Authorization");
    }

    [Fact]
    public void AddLoggingMiddleware_WithoutConfigureOptions_ShouldUseDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddLoggingMiddleware();
        ServiceProvider provider = services.BuildServiceProvider();
        HttpLoggingOptions resolved = provider.GetRequiredService<IOptions<HttpLoggingOptions>>().Value;

        // Assert
        resolved.MaxBodyLogLength.Should().Be(32 * 1024);
        resolved.WarningThresholdMilliseconds.Should().Be(0);
        resolved.PathsToIgnore.Should().BeEmpty();
        resolved.ExcludedHeaders.Should().BeEmpty();
    }

    // =================================================================
    // UseLoggingMiddleware Tests
    // =================================================================

    [Fact]
    public void UseLoggingMiddleware_ShouldNotThrow_WhenCalledWithApp()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        builder.Services.AddLoggingMiddleware();
        WebApplication app = builder.Build();

        // Act
        IApplicationBuilder result = app.UseLoggingMiddleware();

        // Assert
        result.Should().BeAssignableTo<IApplicationBuilder>();
    }

    [Fact]
    public void UseLoggingMiddleware_ShouldReturnIApplicationBuilder_ForFluentChaining()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        builder.Services.AddLoggingMiddleware();
        WebApplication app = builder.Build();

        // Act
        IApplicationBuilder result = app.UseLoggingMiddleware();

        // Assert
        result.Should().BeSameAs(app);
    }
}
