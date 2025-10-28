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

    /// <summary>
    /// Tests that implicit conversion from an <see cref="Exception"/> to a non-generic <see cref="Result"/> correctly creates a failure result.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromException_ShouldCreateFailureResult()
    {
        // Arrange
        var exception = new NotFoundException("Student", "Name");

        // Act
        Result result = exception;

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

    // =================================================================
    // Tests for ResultExtensions
    // =================================================================

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.OnSuccess{T}(Result{T}, Action{T})"/> method executes the provided action when the result is successful.
    /// </summary>
    [Fact]
    public void OnSuccess_ShouldExecuteAction_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result<string>.Success("data");
        bool wasCalled = false;

        // Act
        result.OnSuccess(value => wasCalled = true);

        // Assert
        wasCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ResultExtensions.OnSuccess{T}(Result{T}method does not execute the provided action when the result is a failure.
    /// </summary>
    [Fact]
    public void OnSuccess_ShouldNotExecuteAction_WhenResultIsFailure()
    {
        // Arrange
        Result result = Result<string>.Failure(new Exception());
        bool wasCalled = false;

        // Act
        result.OnSuccess(() => wasCalled = true);

        // Assert
        wasCalled.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> method executes the provided action when the result is a failure.
    /// </summary>
    [Fact]
    public void OnFailure_ShouldExecuteAction_WhenResultIsFailure()
    {
        // Arrange
        var result = Result.Failure(new Exception("error"));
        bool wasCalled = false;

        // Act
        result.OnFailure(ex => wasCalled = true);

        // Assert
        wasCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> method transforms the value when the result is successful.
    /// </summary>
    [Fact]
    public void Map_ShouldTransformValue_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result<int>.Success(10);

        // Act
        Result<int> mappedResult = result.Map(value => value * 2);

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(20);
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> method chains operations when the result is successful.
    /// </summary>
    [Fact]
    public void Bind_ShouldChainOperation_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result<int>.Success(5);
        Func<int, Result<string>> chainFunc = value => Result<string>.Success($"Value is {value}");

        // Act
        Result<string> boundResult = result.Bind(chainFunc);

        // Assert
        boundResult.IsSuccess.Should().BeTrue();
        boundResult.Value.Should().Be("Value is 5");
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> method propagates failure.
    /// </summary>
    [Fact]
    public void Bind_ShouldPropagateFailure_WhenResultIsFailure()
    {
        // Arrange
        var exception = new InvalidOperationException("Initial failure");
        Result result = Result<int>.Failure(exception);
        Func<Result> chainFunc = () => Result.Success($"chainFunc call");

        // Act
        Result boundResult = result.Bind(chainFunc);

        // Assert
        boundResult.IsFailure.Should().BeTrue();
        boundResult.Exception.Should().Be(exception);
    }

    /// <summary>
    /// Tests that the non-generic Match method executes the success function when the result is successful.
    /// </summary>
    [Fact]
    public void Match_NonGeneric_ShouldExecuteSuccessFunc_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result.Success();

        // Act
        string matchResult = result.Match(
            success: () => "success",
            failure: ex => "failure");

        // Assert
        matchResult.Should().Be("success");
    }

    /// <summary>
    /// Tests that the non-generic Match method executes the failure function when the result is a failure.
    /// </summary>
    [Fact]
    public void Match_NonGeneric_ShouldExecuteFailureFunc_WhenResultIsFailure()
    {
        // Arrange
        var exception = new Exception("error");
        var result = Result.Failure(exception);

        // Act
        string matchResult = result.Match(
            success: () => "success",
            failure: ex => ex.Message);

        // Assert
        matchResult.Should().Be("error");
    }

    /// <summary>
    /// Tests that the generic Match method executes the failure function when the result is a failure.
    /// </summary>
    [Fact]
    public void Match_Generic_ShouldExecuteFailureFunc_WhenResultIsFailure()
    {
        // Arrange
        var exception = new Exception("error");
        Result<int> result = exception;

        // Act
        string matchResult = result.Match(
            success: val => val.ToString(System.Globalization.CultureInfo.InvariantCulture),
            failure: ex => ex.Message);

        // Assert
        matchResult.Should().Be("error");
    }

    // =================================================================
    // Tests for Async ResultExtensions
    // =================================================================

    /// <summary>
    /// Tests that OnSuccessAsync executes the action for a successful result.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_ShouldExecuteAction_WhenResultIsSuccess()
    {
        // Arrange
        var resultTask = Task.FromResult(Result.Success());
        bool wasCalled = false;

        // Act
        await resultTask.OnSuccessAsync(() =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        });

        // Assert
        wasCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that OnSuccessAsync (generic) executes the action for a successful result.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_Generic_ShouldExecuteAction_WhenResultIsSuccess()
    {
        // Arrange
        var resultTask = Task.FromResult(Result.Success("data"));
        string? receivedValue = null;

        // Act
        await resultTask.OnSuccessAsync(value =>
        {
            receivedValue = value;
            return Task.CompletedTask;
        });

        // Assert
        receivedValue.Should().Be("data");
    }

    /// <summary>
    /// Tests that OnFailureAsync executes the action for a failure result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_ShouldExecuteAction_WhenResultIsFailure()
    {
        // Arrange
        var exception = new Exception("error");
        var resultTask = Task.FromResult(Result.Failure(exception));
        Exception? receivedException = null;

        // Act
        await resultTask.OnFailureAsync(ex =>
        {
            receivedException = ex;
            return Task.CompletedTask;
        });

        // Assert
        receivedException.Should().Be(exception);
    }

    /// <summary>
    /// Tests that OnFailureAsync (generic) executes the action for a failure result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_Generic_ShouldExecuteAction_WhenResultIsFailure()
    {
        // Arrange
        var exception = new Exception("error");
        var resultTask = Task.FromResult(Result.Failure<string>(exception));
        Exception? receivedException = null;

        // Act
        await resultTask.OnFailureAsync(ex =>
        {
            receivedException = ex;
            return Task.CompletedTask;
        });

        // Assert
        receivedException.Should().Be(exception);
    }

    /// <summary>
    /// Tests that MapAsync transforms the value for a successful result.
    /// </summary>
    [Fact]
    public async Task MapAsync_ShouldTransformValue_WhenResultIsSuccess()
    {
        // Arrange
        var resultTask = Task.FromResult(Result.Success(10));

        // Act
        var mappedResult = await resultTask.MapAsync(value => Task.FromResult(value * 2));

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(20);
    }

    /// <summary>
    /// Tests that MapAsync propagates failure.
    /// </summary>
    [Fact]
    public async Task MapAsync_ShouldPropagateFailure_WhenResultIsFailure()
    {
        // Arrange
        var exception = new Exception("error");
        var resultTask = Task.FromResult(Result.Failure<int>(exception));

        // Act
        var mappedResult = await resultTask.MapAsync(value => Task.FromResult(value * 2));

        // Assert
        mappedResult.IsFailure.Should().BeTrue();
        mappedResult.Exception.Should().Be(exception);
    }

    /// <summary>
    /// Tests that BindAsync chains operations for a successful result.
    /// </summary>
    [Fact]
    public async Task BindAsync_ShouldChainOperation_WhenResultIsSuccess()
    {
        // Arrange
        var resultTask = Task.FromResult(Result.Success(5));
        Func<int, Task<Result<string>>> chainFunc = value => Task.FromResult(Result.Success($"Value is {value}"));

        // Act
        var boundResult = await resultTask.BindAsync(chainFunc);

        // Assert
        boundResult.IsSuccess.Should().BeTrue();
        boundResult.Value.Should().Be("Value is 5");
    }

    /// <summary>
    /// Tests that BindAsync propagates failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_ShouldPropagateFailure_WhenResultIsFailure()
    {
        // Arrange
        var exception = new InvalidOperationException("Initial failure");
        var resultTask = Task.FromResult(Result.Failure<int>(exception));
        Func<int, Task<Result<string>>> chainFunc = value => Task.FromResult(Result.Success($"Value is {value}"));

        // Act
        var boundResult = await resultTask.BindAsync(chainFunc);

        // Assert
        boundResult.IsFailure.Should().BeTrue();
        boundResult.Exception.Should().Be(exception);
    }
}
