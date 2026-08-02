using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RA.Utilities.Api.ExceptionHandlers;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Tests.RA.Utilities.Api.ExceptionHandlers;

/// <summary>
/// Contains unit tests for the <see cref="GlobalExceptionHandler"/> class.
/// </summary>
public class GlobalExceptionHandlerTests
{
    private static GlobalExceptionHandler CreateHandler()
        => new(NullLogger<GlobalExceptionHandler>.Instance);

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new System.IO.MemoryStream();

        // Set up required services for IResult execution (Results.Json requires IServiceProvider)
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();
        context.RequestServices = services.BuildServiceProvider();

        return context;
    }

    // =================================================================
    // TryHandleAsync — Exception Type → Status Code Mapping
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithBadRequestException_ShouldSet400StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new BadRequestException([new ValidationError("Invalid input.")]);

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.BadRequest);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithConflictException_ShouldSet409StatusCode()
    {
        // Arrange
        DefaultHttpContext context = CreateHttpContext();
        var exception = new ConflictException("User", "duplicate@example.com");

        // Act
        bool handled = await CreateHandler().TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.Conflict);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithNotFoundException_ShouldSet404StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new NotFoundException("Product", 42);

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.NotFound);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithUnauthorizedException_ShouldSet401StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new UnauthorizedException("Token expired.");

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.Unauthorized);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithForbiddenException_ShouldSet403StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new ForbiddenException("Access denied.");

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.Forbidden);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithTooManyRequestsException_ShouldSet429StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new TooManyRequestsException();

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.TooManyRequests);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithServiceUnavailableException_ShouldSet503StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new ServiceUnavailableException();

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.ServiceUnavailable);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithGatewayTimeoutException_ShouldSet504StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new GatewayTimeoutException();

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.GatewayTimeout);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithUnprocessableException_ShouldSet422StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new UnprocessableException("Cannot process entity.");

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.Unprocessable);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_WithGenericException_ShouldSet500StatusCode()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new InvalidOperationException("Something went wrong.");

        // Act
        bool handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(BaseResponseCode.InternalServerError);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_ShouldReturnTrue_ForAllExceptions()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();

        // Act
        bool handled = await handler.TryHandleAsync(context, new Exception("test"), default);

        // Assert
        handled.Should().BeTrue();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_ShouldWriteJsonContentType()
    {
        // Arrange
        GlobalExceptionHandler handler = CreateHandler();
        DefaultHttpContext context = CreateHttpContext();
        var exception = new NotFoundException("Item", 1);

        // Act
        await handler.TryHandleAsync(context, exception, default);

        // Assert
        context.Response.ContentType.Should().Contain("application/json");
    }
}
