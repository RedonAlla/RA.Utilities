using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Api.Options;

namespace RA.Utilities.Tests.RA.Utilities.Api.Middlewares;

/// <summary>
/// Contains unit tests for the <see cref="DefaultHeadersMiddleware"/> class.
/// </summary>
public class DefaultHeadersMiddlewareTests
{
    private static DefaultHttpContext CreateContext(string path = "/api/test")
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static DefaultHeadersMiddleware CreateMiddleware(DefaultHeadersOptions? options = null)
    {
        options ??= new DefaultHeadersOptions();
        return new DefaultHeadersMiddleware(Options.Create(options));
    }

    // =================================================================
    // Default behavior
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithDefaultOptionsAndMissingXRequestId_ShouldAutoGenerateAndEcho()
    {
        // Arrange
        DefaultHeadersMiddleware middleware = CreateMiddleware();
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Headers.Should().ContainKey("x-request-id");
        string value = context.Response.Headers["x-request-id"].ToString();
        value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvokeAsync_WithExistingXRequestId_ShouldEchoExistingValue()
    {
        // Arrange
        DefaultHeadersMiddleware middleware = CreateMiddleware();
        DefaultHttpContext context = CreateContext();
        string existingId = "abc-123-def";
        context.Request.Headers["x-request-id"] = existingId;

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Headers["x-request-id"].ToString().Should().Be(existingId);
    }

    // =================================================================
    // Dynamic required headers — AutoGenerate
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithAutoGenerateHeaderMissing_ShouldGenerateValue()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-correlation-id",
            AutoGenerate = true,
            EchoInResponse = true,
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Headers.Should().ContainKey("x-correlation-id");
    }

    [Fact]
    public async Task InvokeAsync_WithAutoGenerateHeaderPresent_ShouldUseExistingValue()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-correlation-id",
            AutoGenerate = true,
            EchoInResponse = true,
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();
        context.Request.Headers["x-correlation-id"] = "existing-correlation";

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Headers["x-correlation-id"].ToString().Should().Be("existing-correlation");
    }

    // =================================================================
    // Dynamic required headers — mandatory (no AutoGenerate)
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithMandatoryHeaderMissing_ShouldReturn400()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-api-key",
            AutoGenerate = false,
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithMandatoryHeaderMissing_ShouldIncludeInErrorResponse()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-api-key",
            AutoGenerate = false,
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        string body = await reader.ReadToEndAsync();
        body.Should().Contain("x-api-key");
        body.Should().Contain("is required");
    }

    [Fact]
    public async Task InvokeAsync_WithMandatoryHeaderPresent_ShouldContinuePipeline()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-api-key",
            AutoGenerate = false,
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();
        context.Request.Headers["x-api-key"] = "secret-key";
        bool nextCalled = false;

        // Act
        await middleware.InvokeAsync(context, _ => { nextCalled = true; return Task.CompletedTask; });

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    // =================================================================
    // Custom error message
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithCustomErrorMessage_ShouldUseCustomMessage()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-api-key",
            AutoGenerate = false,
            ErrorMessage = "API key is mandatory for this endpoint.",
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        string body = await reader.ReadToEndAsync();
        body.Should().Contain("API key is mandatory for this endpoint.");
    }

    // =================================================================
    // Multiple required headers
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithMultipleMissingHeaders_ShouldReturnAllErrors()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition { Name = "x-api-key" });
        options.RequiredHeaders.Add(new RequiredHeaderDefinition { Name = "x-tenant-id", ErrorMessage = "Tenant ID required." });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        string body = await reader.ReadToEndAsync();
        body.Should().Contain("x-api-key");
        body.Should().Contain("x-tenant-id");
        body.Should().Contain("Tenant ID required.");
    }

    [Fact]
    public async Task InvokeAsync_WithMixedHeaders_ShouldCollectOnlyMandatoryFailures()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition { Name = "x-api-key", AutoGenerate = false });
        options.RequiredHeaders.Add(new RequiredHeaderDefinition { Name = "x-correlation-id", AutoGenerate = true, EchoInResponse = true });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        // The auto-generated header should not be in the response since pipeline was short-circuited
        context.Response.Headers.Should().NotContainKey("x-correlation-id");
    }

    // =================================================================
    // EchoInResponse
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithEchoDisabled_ShouldNotAddToResponse()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition
        {
            Name = "x-request-id",
            AutoGenerate = true,
            EchoInResponse = false,
        });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.Headers.Should().NotContainKey("x-request-id");
    }

    // =================================================================
    // Path exclusion
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithExcludedPath_ShouldSkipHeaderCheck()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.PathsToIgnore.Add("/health");
        // Remove default x-request-id and add a mandatory header
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition { Name = "x-api-key" });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext("/health/ready");
        bool nextCalled = false;

        // Act
        await middleware.InvokeAsync(context, _ => { nextCalled = true; return Task.CompletedTask; });

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WithExcludedPathCaseInsensitive_ShouldSkipHeaderCheck()
    {
        // Arrange
        var options = new DefaultHeadersOptions();
        options.PathsToIgnore.Add("/Health");
        options.RequiredHeaders.Clear();
        options.RequiredHeaders.Add(new RequiredHeaderDefinition { Name = "x-api-key" });
        DefaultHeadersMiddleware middleware = CreateMiddleware(options);
        DefaultHttpContext context = CreateContext("/health/ready");
        bool nextCalled = false;

        // Act
        await middleware.InvokeAsync(context, _ => { nextCalled = true; return Task.CompletedTask; });

        // Assert
        nextCalled.Should().BeTrue();
    }
}
