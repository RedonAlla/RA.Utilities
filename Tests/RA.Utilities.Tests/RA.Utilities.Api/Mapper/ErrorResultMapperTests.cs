using System;
using FluentAssertions;
using RA.Utilities.Api.Mapper;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Tests.RA.Utilities.Api.Mapper;

/// <summary>
/// Contains unit tests for the <see cref="ErrorResultMapper"/> class,
/// verifying exception-to-response mapping for all supported exception types.
/// </summary>
public class ErrorResultMapperTests
{
    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithBadRequestException_ShouldReturnBadRequestResponse()
    {
        // Arrange
        ValidationError[] validationErrors =
        [
            new ValidationError("Email is invalid.")
            {
                PropertyName = "Email",
                AttemptedValue = "bad-email",
                ErrorCode = "INVALID_FORMAT"
            }
        ];
        var exception = new BadRequestException(validationErrors);

        // Act
        BadRequestResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<BadRequestResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.BadRequest);
        result.ResponseType.Should().Be(ResponseType.BadRequest);
        result.Result.Should().HaveCount(1);
        result.Result[0].PropertyName.Should().Be("Email");
        result.Result[0].ErrorMessage.Should().Be("Email is invalid.");
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullBadRequestException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<BadRequestResponse> act = () =>
            ErrorResultMapper.ToResponse((BadRequestException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithConflictException_ShouldReturnConflictResponse()
    {
        // Arrange
        var exception = new ConflictException("User", "existing@example.com");

        // Act
        ConflictResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<ConflictResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.Conflict);
        result.ResponseType.Should().Be(ResponseType.Conflict);
        result.Result.Should().NotBeNull();
        result.Result.Entity.Should().Be("User");
        result.Result.Value.Should().Be("existing@example.com");
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullConflictException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<ConflictResponse> act = () =>
            ErrorResultMapper.ToResponse((ConflictException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNotFoundException_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var exception = new NotFoundException("Product", 42);

        // Act
        NotFoundResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<NotFoundResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.NotFound);
        result.ResponseType.Should().Be(ResponseType.NotFound);
        result.Result.Should().NotBeNull();
        result.Result.Entity.Should().Be("Product");
        result.Result.Value.Should().Be(42);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullNotFoundException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<NotFoundResponse> act = () =>
            ErrorResultMapper.ToResponse((NotFoundException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithUnauthorizedException_ShouldReturnUnauthorizedResponse()
    {
        // Arrange
        var exception = new UnauthorizedException("Invalid token.");

        // Act
        UnauthorizedResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<UnauthorizedResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.Unauthorized);
        result.ResponseType.Should().Be(ResponseType.Unauthorized);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullUnauthorizedException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<UnauthorizedResponse> act = () =>
            ErrorResultMapper.ToResponse((UnauthorizedException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithForbiddenException_ShouldReturnForbiddenResponse()
    {
        // Arrange
        var exception = new ForbiddenException("Access denied.");

        // Act
        ForbiddenResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<ForbiddenResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.Forbidden);
        result.ResponseType.Should().Be(ResponseType.Forbidden);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullForbiddenException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<ForbiddenResponse> act = () =>
            ErrorResultMapper.ToResponse((ForbiddenException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithUnprocessableException_ShouldReturnUnprocessableResponse()
    {
        // Arrange
        var exception = new UnprocessableException("Cannot process the request.");

        // Act
        UnprocessableResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<UnprocessableResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.Unprocessable);
        result.ResponseType.Should().Be(ResponseType.Unprocessable);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullUnprocessableException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<UnprocessableResponse> act = () =>
            ErrorResultMapper.ToResponse((UnprocessableException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithTooManyRequestsException_ShouldReturnTooManyRequestsResponse()
    {
        // Arrange
        var exception = new TooManyRequestsException();

        // Act
        TooManyRequestsResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<TooManyRequestsResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.TooManyRequests);
        result.ResponseType.Should().Be(ResponseType.TooManyRequests);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullTooManyRequestsException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<TooManyRequestsResponse> act = () =>
            ErrorResultMapper.ToResponse((TooManyRequestsException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithServiceUnavailableException_ShouldReturnServiceUnavailableResponse()
    {
        // Arrange
        var exception = new ServiceUnavailableException();

        // Act
        ServiceUnavailableResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<ServiceUnavailableResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.ServiceUnavailable);
        result.ResponseType.Should().Be(ResponseType.ServiceUnavailable);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullServiceUnavailableException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<ServiceUnavailableResponse> act = () =>
            ErrorResultMapper.ToResponse((ServiceUnavailableException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithGatewayTimeoutException_ShouldReturnGatewayTimeoutResponse()
    {
        // Arrange
        var exception = new GatewayTimeoutException();

        // Act
        GatewayTimeoutResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<GatewayTimeoutResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.GatewayTimeout);
        result.ResponseType.Should().Be(ResponseType.GatewayTimeout);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullGatewayTimeoutException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<GatewayTimeoutResponse> act = () =>
            ErrorResultMapper.ToResponse((GatewayTimeoutException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithRaBaseException_ShouldReturnErrorResponse()
    {
        // Arrange
        var exception = new RaBaseException(500, ResponseType.Error, "An unexpected error occurred.");

        // Act
        ErrorResponse result = ErrorResultMapper.ToResponse(exception);

        // Assert
        result.Should().BeOfType<ErrorResponse>();
        result.ResponseCode.Should().Be(BaseResponseCode.InternalServerError);
        result.ResponseType.Should().Be(ResponseType.Error);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void ToResponse_WithNullRaBaseException_ShouldThrowArgumentNullException()
    {
        // Act
        Func<ErrorResponse> act = () =>
            ErrorResultMapper.ToResponse((RaBaseException)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
