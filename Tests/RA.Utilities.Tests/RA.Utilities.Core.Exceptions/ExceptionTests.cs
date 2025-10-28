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
        var ex = new RaBaseException(message);

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
        var ex = new ConflictException(message: message, entityName, value);

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
        var ex = new NotFoundException(message, entityName, entityValue);

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
        var ex = new UnauthorizedException("You shall not pass!");

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
        ValidationErrors[] validationErrors =
        [
            new ValidationErrors
            {
                PropertyName = "Email",
                ErrorMessage = "Email is not in a valid format.",
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
        ValidationErrors[] validationErrors = [new ValidationErrors { ErrorMessage = "An error" }];
        const string customMessage = "A custom error occurred.";
        const int customCode = 499;

        // Act
        var ex = new BadRequestException(validationErrors, customCode, customMessage);

        // Assert
        ex.Errors.Should().BeEquivalentTo(validationErrors);
        ex.ErrorCode.Should().Be(customCode);
        ex.Message.Should().Be(customMessage);
    }
}
