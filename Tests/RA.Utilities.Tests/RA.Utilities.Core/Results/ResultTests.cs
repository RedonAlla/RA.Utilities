using System;
using System.Threading.Tasks;
using FluentAssertions;
using RA.Utilities.Core;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Tests.RA.Utilities.Core.Results;

/// <summary>
/// Contains unit tests for the <see cref="Result"/> and <see cref="Result{T}"/> types.
/// </summary>
public class ResultTests
{
    // =================================================================
    // Tests for non-generic Result
    // =================================================================

    /// <summary>
    /// Tests that the <see cref="Result.Success()"/> method correctly creates a successful result.
    /// </summary>
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Exception.Should().BeNull();
    }

    /// <summary>
    /// Tests that the <see cref="Result.Failure(Exception)"/> method correctly creates a failure result.
    /// </summary>
    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = Result.Failure(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Exception.Should().Be(exception);
    }

    /// <summary>
    /// Tests that the <see cref="Result.Failure{TResult}(Exception)"/> method correctly creates a failure result for a generic type.
    /// </summary>
    [Fact]
    public void Failure_ShouldCreateFailureTResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        Result result = Result<int>.Failure(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Exception.Should().Be(exception);
    }

    // =================================================================
    // Tests for generic Result<T>
    // =================================================================

    /// <summary>
    /// Tests that the method correctly creates a successful result with a value.
    /// </summary>
    [Fact]
    public void Success_WithValue_ShouldCreateSuccessResult()
    {
        // Arrange
        string successValue = "This is a success";

        // Act
        var result = Result<string>.Success(successValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(successValue);
        result.Exception.Should().BeNull();
    }

    /// <summary>
    /// Tests that the Result method correctly creates a failure result for a value type.
    /// </summary>
    [Fact]
    public void Failure_WithValueType_ShouldCreateFailureResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Generic test error");

        // Act
        Result result = Result<int>.Failure(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Exception.Should().Be(exception);
        result.Exception.Should().BeOfType<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that implicit conversion from a value to a generic <see cref="Result{T}"/> correctly creates a success result.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
    {
        // Arrange
        int value = 42;

        // Act
        Result<int> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    /// <summary>
    /// Tests that implicit conversion from a generic <see cref="Exception"/> to a generic <see cref="Result{T}"/> correctly creates a failure result.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromGenericException_ShouldCreateFailureResult()
    {
        // Arrange
        var exception = new ConflictException("Student", "Name");

        // Act
        Result<string> result = exception;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Exception.Should().Be(exception);
    }
}
