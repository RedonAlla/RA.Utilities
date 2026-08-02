using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.Mapper;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Tests.RA.Utilities.Api.Mapper;

/// <summary>
/// Contains unit tests for the <see cref="ErrorResultResponse"/> class.
/// </summary>
public class ErrorResultResponseTests
{
    // =================================================================
    // BadRequestException → 400
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithBadRequestException_ShouldReturnJsonResultWith400()
    {
        // Arrange
        var exception = new BadRequestException([new ValidationError("Invalid input.")]);

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.BadRequest);
    }

    // =================================================================
    // ConflictException → 409
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithConflictException_ShouldReturnJsonResultWith409()
    {
        // Arrange
        var exception = new ConflictException("User", "duplicate@example.com");

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.Conflict);
    }

    // =================================================================
    // UnprocessableException → 422
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithUnprocessableException_ShouldReturnJsonResultWith422()
    {
        // Arrange
        var exception = new UnprocessableException("Cannot process.");

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.Unprocessable);
    }

    // =================================================================
    // NotFoundException → 404
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithNotFoundException_ShouldReturnJsonResultWith404()
    {
        // Arrange
        var exception = new NotFoundException("Product", 99);

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.NotFound);
    }

    // =================================================================
    // UnauthorizedException → 401
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithUnauthorizedException_ShouldReturnJsonResultWith401()
    {
        // Arrange
        var exception = new UnauthorizedException("Token expired.");

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.Unauthorized);
    }

    // =================================================================
    // ForbiddenException → 403
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithForbiddenException_ShouldReturnJsonResultWith403()
    {
        // Arrange
        var exception = new ForbiddenException("Insufficient permissions.");

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.Forbidden);
    }

    // =================================================================
    // TooManyRequestsException → 429
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithTooManyRequestsException_ShouldReturnJsonResultWith429()
    {
        // Arrange
        var exception = new TooManyRequestsException();

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.TooManyRequests);
    }

    // =================================================================
    // ServiceUnavailableException → 503
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithServiceUnavailableException_ShouldReturnJsonResultWith503()
    {
        // Arrange
        var exception = new ServiceUnavailableException();

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.ServiceUnavailable);
    }

    // =================================================================
    // GatewayTimeoutException → 504
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithGatewayTimeoutException_ShouldReturnJsonResultWith504()
    {
        // Arrange
        var exception = new GatewayTimeoutException();

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.GatewayTimeout);
    }

    // =================================================================
    // RaBaseException (unmapped) → 500
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithRaBaseException_ShouldReturnJsonResultWith500()
    {
        // Arrange
        var exception = new RaBaseException(500, ResponseType.Error, "Unknown error.");

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.InternalServerError);
    }

    // =================================================================
    // Generic Exception → 500
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Result_WithGenericException_ShouldReturnJsonResultWith500()
    {
        // Arrange
        var exception = new InvalidOperationException("Something broke.");

        // Act
        IResult result = ErrorResultResponse.Result(exception);

        // Assert
        IStatusCodeHttpResult jsonResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        jsonResult.StatusCode.Should().Be(BaseResponseCode.InternalServerError);
    }
}
