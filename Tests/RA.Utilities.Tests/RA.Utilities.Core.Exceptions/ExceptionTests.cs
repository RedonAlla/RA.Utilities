using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Tests.RA.Utilities.Core.Exceptions;

/// <summary>
/// Contains unit tests for custom exception types, focusing on serialization.
/// </summary>
public class ExceptionTests
{
    // =================================================================
    // Tests for Exception Serialization
    // =================================================================

    /// <summary>
    /// Tests the default constructor of RaBaseException.
    /// </summary>
    [Fact]
    public void RaBaseException_DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var ex = new RaBaseException();

        // Assert
        ex.Should().NotBeNull();
        ex.InnerException.Should().BeNull();
    }

    /// <summary>
    /// Tests the constructor of RaBaseException with a message.
    /// </summary>
    [Fact]
    public void RaBaseException_WithMessage_ShouldSetMessage()
    {
        // Arrange
        string message = "Test message";

        // Act
        var ex = new RaBaseException(500, ResponseType.Error, message);

        // Assert
        ex.Message.Should().Be(message);
    }

    /// <summary>
    /// Tests the constructor of ConflictException with a message and inner exception.
    /// </summary>
    [Fact]
    public void ConflictException_ShouldSetProperties()
    {
        // Arrange
        string entityName = "Student";
        int value = 1225;

        // Act
        var ex = new ConflictException(entityName, value);

        // Assert
        ex.Message.Should().Be($"{entityName} with value '{value}' already exists.");
        ex.EntityName.Should().NotBeNull();
        ex.EntityValue.Should().NotBeNull();
    }

    /// <summary>
    /// Tests the constructor of ConflictException with a message and inner exception.
    /// </summary>
    [Fact]
    public void ConflictException_WithMessage_ShouldSetProperties()
    {
        // Arrange
        string entityName = "Student";
        int value = 1225;
        string message = "Custom message for ConflictException";

        // Act
        var ex = new ConflictException(entityName, value, message: message);

        // Assert
        ex.Message.Should().Be(message);
        ex.EntityName.Should().Be(entityName);
        ex.EntityValue.Should().Be(value);
    }

    /// <summary>
    /// Tests the constructor of NotFoundException with a message and inner exception.
    /// </summary>
    [Fact]
    public void NotFoundException_WithMessageAndInnerException_ShouldSetProperties()
    {
        // Arrange
        string entityName = "Student";
        string entityValue = "Smith";

        // Act
        var ex = new NotFoundException(entityName, entityValue);

        // Assert
        ex.Message.Should().Be($"{entityName} with value '{entityValue}' was not found.");
        ex.EntityName.Should().Be(entityName);
        ex.EntityValue.Should().Be(entityValue);
    }

    /// <summary>
    /// Tests the constructor of NotFoundException with a message and inner exception.
    /// </summary>
    [Fact]
    public void NotFoundException_WithMessageAndInnerException_ShouldSetProperties2()
    {
        // Arrange
        string entityName = "Student";
        string entityValue = "Smith";
        string message = "Custom message for NotFoundException";

        // Act
        var ex = new NotFoundException(entityName, entityValue, message: message);

        // Assert
        ex.Message.Should().Be(message);
        ex.EntityName.Should().Be(entityName);
        ex.EntityValue.Should().Be(entityValue);
    }

    /// <summary>
    /// Tests the default constructor of UnauthorizedException.
    /// </summary>
    [Fact]
    public void UnauthorizedException_DefaultConstructor_ShouldSetDefaultMessage()
    {
        // Act
        var ex = new UnauthorizedException();

        // Assert
        ex.Message.Should().Be(BaseResponseMessages.Unauthorized);
        ex.ErrorCode.Should().Be(401);
    }

    /// <summary>
    /// Tests the constructor of UnauthorizedException with a custom message.
    /// </summary>
    [Fact]
    public void UnauthorizedException_WithMessage_ShouldSetCustomMessage()
    {
        // Act
        var ex = new UnauthorizedException(message: "You shall not pass!");

        // Assert
        ex.Message.Should().Be("You shall not pass!");
        ex.ErrorCode.Should().Be(401);
    }

    /// <summary>
    /// Tests the constructor of BadRequestException with default values.
    /// </summary>
    [Fact]
    public void BadRequestException_Constructor_ShouldSetDefaultProperties()
    {
        // Arrange
        ValidationError[] validationErrors =
        [
            new ValidationError("Email is not in a valid format.")
            {
                PropertyName = "Email",
                AttemptedValue = "not-an-email",
                ErrorCode = "InvalidFormat"
            }
        ];

        // Act
        var ex = new BadRequestException(validationErrors);

        // Assert
        ex.Errors.Should().BeEquivalentTo(validationErrors);
        ex.ErrorCode.Should().Be(BaseResponseCode.BadRequest);
        ex.Message.Should().Be(BaseResponseMessages.BadRequest);
    }

    /// <summary>
    /// Tests the constructor of BadRequestException with a custom message and code.
    /// </summary>
    [Fact]
    public void BadRequestException_Constructor_WithCustomValues_ShouldSetProperties()
    {
        // Arrange
        ValidationError[] validationErrors = [new ValidationError("An error")];
        const string customMessage = "A custom error occurred.";
        const int customCode = 499;

        // Act
        var ex = new BadRequestException(validationErrors, customCode, customMessage);

        // Assert
        ex.Errors.Should().BeEquivalentTo(validationErrors);
        ex.ErrorCode.Should().Be(customCode);
        ex.Message.Should().Be(customMessage);
    }

    // =================================================================
    // Tests for TooManyRequestsException
    // =================================================================

    /// <summary>
    /// Tests the default constructor of TooManyRequestsException.
    /// </summary>
    [Fact]
    public void TooManyRequestsException_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Act
        var ex = new TooManyRequestsException();

        // Assert
        ex.Message.Should().Be(BaseResponseMessages.TooManyRequests);
        ex.ErrorCode.Should().Be(BaseResponseCode.TooManyRequests);
        ex.ResponseType.Should().Be(ResponseType.TooManyRequests);
    }

    /// <summary>
    /// Tests the constructor of TooManyRequestsException with a custom message.
    /// </summary>
    [Fact]
    public void TooManyRequestsException_WithMessage_ShouldSetCustomMessage()
    {
        // Arrange
        const string message = "Rate limit of 100 requests per hour exceeded.";

        // Act
        var ex = new TooManyRequestsException(message);

        // Assert
        ex.Message.Should().Be(message);
        ex.ErrorCode.Should().Be(BaseResponseCode.TooManyRequests);
    }

    /// <summary>
    /// Tests the constructor of TooManyRequestsException with a custom error code and message.
    /// </summary>
    [Fact]
    public void TooManyRequestsException_WithErrorCodeAndMessage_ShouldSetProperties()
    {
        // Arrange
        const int errorCode = 429;
        const string message = "Tenant quota exceeded. Upgrade your plan.";

        // Act
        var ex = new TooManyRequestsException(errorCode, message);

        // Assert
        ex.ErrorCode.Should().Be(errorCode);
        ex.Message.Should().Be(message);
    }

    // =================================================================
    // Tests for PaymentRequiredException
    // =================================================================

    /// <summary>
    /// Tests the default constructor of PaymentRequiredException.
    /// </summary>
    [Fact]
    public void PaymentRequiredException_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Act
        var ex = new PaymentRequiredException();

        // Assert
        ex.Message.Should().Be(BaseResponseMessages.PaymentRequired);
        ex.ErrorCode.Should().Be(BaseResponseCode.PaymentRequired);
        ex.ResponseType.Should().Be(ResponseType.PaymentRequired);
    }

    /// <summary>
    /// Tests the constructor of PaymentRequiredException with a custom message.
    /// </summary>
    [Fact]
    public void PaymentRequiredException_WithMessage_ShouldSetCustomMessage()
    {
        // Arrange
        const string message = "An active subscription is required.";

        // Act
        var ex = new PaymentRequiredException(message);

        // Assert
        ex.Message.Should().Be(message);
        ex.ErrorCode.Should().Be(BaseResponseCode.PaymentRequired);
    }

    /// <summary>
    /// Tests the constructor of PaymentRequiredException with a custom error code and message.
    /// </summary>
    [Fact]
    public void PaymentRequiredException_WithErrorCodeAndMessage_ShouldSetProperties()
    {
        // Arrange
        const int errorCode = 402;
        const string message = "Tenant quota exhausted. Upgrade your plan.";

        // Act
        var ex = new PaymentRequiredException(errorCode, message);

        // Assert
        ex.ErrorCode.Should().Be(errorCode);
        ex.Message.Should().Be(message);
    }

    // =================================================================
    // Tests for ServiceUnavailableException
    // =================================================================

    /// <summary>
    /// Tests the default constructor of ServiceUnavailableException.
    /// </summary>
    [Fact]
    public void ServiceUnavailableException_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Act
        var ex = new ServiceUnavailableException();

        // Assert
        ex.Message.Should().Be(BaseResponseMessages.ServiceUnavailable);
        ex.ErrorCode.Should().Be(BaseResponseCode.ServiceUnavailable);
        ex.ResponseType.Should().Be(ResponseType.ServiceUnavailable);
    }

    /// <summary>
    /// Tests the constructor of ServiceUnavailableException with a custom message.
    /// </summary>
    [Fact]
    public void ServiceUnavailableException_WithMessage_ShouldSetCustomMessage()
    {
        // Arrange
        const string message = "Database cluster is currently failing over.";

        // Act
        var ex = new ServiceUnavailableException(message);

        // Assert
        ex.Message.Should().Be(message);
        ex.ErrorCode.Should().Be(BaseResponseCode.ServiceUnavailable);
    }

    /// <summary>
    /// Tests the constructor of ServiceUnavailableException with a custom error code and message.
    /// </summary>
    [Fact]
    public void ServiceUnavailableException_WithErrorCodeAndMessage_ShouldSetProperties()
    {
        // Arrange
        const int errorCode = 503;
        const string message = "Scheduled maintenance in progress.";

        // Act
        var ex = new ServiceUnavailableException(errorCode, message);

        // Assert
        ex.ErrorCode.Should().Be(errorCode);
        ex.Message.Should().Be(message);
    }

    // =================================================================
    // Tests for default message fallbacks and v10.1.0 convenience constructors
    // =================================================================

    /// <summary>
    /// Omitting the message must fall back to the package default, not produce an empty message.
    /// </summary>
    [Fact]
    public void Exceptions_WithErrorCodeOnly_ShouldUseDefaultMessage()
    {
        // Act
        var notFound = new NotFoundException(BaseResponseCode.NotFound);
        var conflict = new ConflictException(BaseResponseCode.Conflict);
        var badRequest = new BadRequestException([new ValidationError("An error")], BaseResponseCode.BadRequest);
        var tooManyRequests = new TooManyRequestsException(BaseResponseCode.TooManyRequests);

        // Assert
        notFound.Message.Should().Be(BaseResponseMessages.NotFound);
        conflict.Message.Should().Be(BaseResponseMessages.Conflict);
        badRequest.Message.Should().Be(BaseResponseMessages.BadRequest);
        tooManyRequests.Message.Should().Be(BaseResponseMessages.TooManyRequests);
    }

    /// <summary>
    /// Passing null for the message must fall back to the package default.
    /// </summary>
    [Fact]
    public void Exceptions_WithNullMessage_ShouldUseDefaultMessage()
    {
        // Act
        var notFound = new NotFoundException(404, null);
        var conflict = new ConflictException(409, null);
        var forbidden = new ForbiddenException(403, null);
        var baseException = new RaBaseException(500, ResponseType.Error, null);

        // Assert
        notFound.Message.Should().Be(BaseResponseMessages.NotFound);
        conflict.Message.Should().Be(BaseResponseMessages.Conflict);
        forbidden.Message.Should().Be(BaseResponseMessages.Forbidden);
        baseException.Message.Should().Be(BaseResponseMessages.Error);
    }

    /// <summary>
    /// An explicitly empty message must be preserved rather than replaced with the default.
    /// </summary>
    [Fact]
    public void Exceptions_WithEmptyMessage_ShouldPreserveEmptyMessage()
    {
        // Act
        var ex = new UnauthorizedException(string.Empty);

        // Assert
        ex.Message.Should().BeEmpty();
    }

    /// <summary>
    /// The entity/value/message overload must keep the entity details and use the custom message.
    /// </summary>
    [Fact]
    public void EntityExceptions_WithCustomMessage_ShouldSetEntityDetails()
    {
        // Act
        var notFound = new NotFoundException("Student", 1225, "Student is no longer enrolled.");
        var conflict = new ConflictException("Student", 1225, "Student is already enrolled.");

        // Assert
        notFound.Message.Should().Be("Student is no longer enrolled.");
        notFound.EntityName.Should().Be("Student");
        notFound.EntityValue.Should().Be(1225);
        conflict.Message.Should().Be("Student is already enrolled.");
        conflict.EntityName.Should().Be("Student");
        conflict.EntityValue.Should().Be(1225);
    }

    /// <summary>
    /// The parameterless constructors must use the documented defaults.
    /// </summary>
    [Fact]
    public void ParameterlessExceptions_ShouldUseDocumentedDefaults()
    {
        // Act
        var notFound = new NotFoundException();
        var conflict = new ConflictException();

        // Assert
        notFound.ErrorCode.Should().Be(BaseResponseCode.NotFound);
        notFound.ResponseType.Should().Be(ResponseType.NotFound);
        notFound.Message.Should().Be(BaseResponseMessages.NotFound);
        conflict.ErrorCode.Should().Be(BaseResponseCode.Conflict);
        conflict.ResponseType.Should().Be(ResponseType.Conflict);
        conflict.Message.Should().Be(BaseResponseMessages.Conflict);
    }
}
