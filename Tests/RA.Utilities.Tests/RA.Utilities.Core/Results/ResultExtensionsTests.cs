using System;
using System.Threading.Tasks;
using FluentAssertions;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Tests.RA.Utilities.Core.Results;

/// <summary>
/// Contains unit tests for the <see cref="ResultExtensions"/> class.
/// </summary>
public class ResultExtensionsTests
{
    // =================================================================
    // Tests for ResultExtensions
    // =================================================================

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

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.OnSuccess(Result, Action)"/> method executes the action based on the result's state.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnSuccess_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Result result = isSuccess ? Result.Success() : Result.Failure(new Exception());
        bool wasCalled = false;

        // Act
        result.OnSuccess(() => wasCalled = true);

        // Assert
        wasCalled.Should().Be(isSuccess);
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> method executes the action based on the result's state.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnFailure_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Result result = isSuccess ? Result.Success() : Result.Failure(new Exception("error"));
        bool wasCalled = false;

        // Act
        result.OnFailure(ex => wasCalled = true);

        // Assert
        wasCalled.Should().Be(!isSuccess);
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> method transforms a success value or propagates a failure.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_ShouldTransformOrPropagate(bool isSuccess)
    {
        // Arrange
        var exception = new Exception("error");
        Result<int> result = isSuccess ? Result.Success(10) : Result.Failure<int>(exception);

        // Act
        Result<int> mappedResult = result.Map(value => value * 2);

        // Assert
        mappedResult.IsSuccess.Should().Be(isSuccess);
        if (isSuccess)
        {
            mappedResult.Value.Should().Be(20);
        }
        else
        {
            mappedResult.Exception.Should().Be(exception);
        }
    }

    /// <summary>
    /// Tests that the <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> method chains an operation or propagates a failure.
    /// </summary>
    [Fact]
    public void Bind_ShouldChainOrPropagate()
    {
        // Arrange
        var successResult = Result.Success(5);
        var failureResult = Result.Failure<int>(new InvalidOperationException("Initial failure"));
        static Result<string> chainFunc(int value) => Result.Success($"Value is {value}");

        // Act
        Result<string> successBoundResult = successResult.Bind(chainFunc);
        Result<string> failureBoundResult = failureResult.Bind(chainFunc);

        // Assert
        successBoundResult.IsSuccess.Should().BeTrue();
        successBoundResult.Value.Should().Be("Value is 5");

        failureBoundResult.IsFailure.Should().BeTrue();
        failureBoundResult.Exception.Should().Be(failureResult.Exception);
    }

    /// <summary>
    /// Tests that the non-generic Match method executes the success function when the result is successful.
    /// </summary>
    [Fact]
    public void Match_NonGeneric_ShouldExecuteSuccessFunc_WhenResultIsSuccess()
    { // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(new Exception("error"));

        // Act
        string successMatch = successResult.Match(
            success: () => "success",
            failure: ex => "failure");

        string failureMatch = failureResult.Match(success: () => "success", failure: ex => ex.Message);

        // Assert
        successMatch.Should().Be("success");
        failureMatch.Should().Be("error");
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
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnSuccessAsync_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Task<Result> resultTask = Task.FromResult(isSuccess ? Result.Success() : Result.Failure(new Exception()));
        bool wasCalled = false;

        // Act
        await resultTask.OnSuccessAsync(() =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        });

        // Assert
        wasCalled.Should().Be(isSuccess);
    }

    /// <summary>
    /// Tests that OnSuccessAsync (generic) executes the action for a successful result.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnSuccessAsync_Generic_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Task<Result<string>> resultTask = Task.FromResult(isSuccess
            ? Result.Success("data")
            : Result.Failure<string>(new Exception()));

        string? receivedValue = null;

        // Act
        await resultTask.OnSuccessAsync(value =>
        { receivedValue = value; return Task.CompletedTask; });

        // Assert
        if (isSuccess)
        {
            receivedValue.Should().Be("data");
        }
        else
        {
            receivedValue.Should().BeNull();
        }
    }

    /// <summary>
    /// Tests that OnFailureAsync executes the action for a failure result.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnFailureAsync_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        var exception = new Exception("error");
        Task<Result> resultTask = Task.FromResult(isSuccess ? Result.Success() : Result.Failure(exception));
        Exception? receivedException = null;

        // Act
        await resultTask.OnFailureAsync(ex =>
        {
            receivedException = ex;
            return Task.CompletedTask;
        });

        // Assert
        if (isSuccess)
        {
            receivedException.Should().BeNull();
        }
        else
        {
            receivedException.Should().Be(exception);
        }
    }

    /// <summary>
    /// Tests that MapAsync transforms a success value or propagates a failure.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MapAsync_ShouldTransformOrPropagate(bool isSuccess)
    {
        // Arrange
        var exception = new Exception("error");
        Task<Result<int>> resultTask = Task.FromResult(isSuccess ? Result.Success(10) : Result.Failure<int>(exception));

        // Act
        Result<int> mappedResult = await resultTask.MapAsync(value => Task.FromResult(value * 2));

        // Assert
        mappedResult.IsSuccess.Should().Be(isSuccess);
        if (isSuccess)
        {
            mappedResult.Value.Should().Be(20);
        }
        else
        {
            mappedResult.Exception.Should().Be(exception);
        }
    }

    /// <summary>
    /// Tests that BindAsync chains an operation or propagates a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_ShouldChainOrPropagate()
    {
        // Arrange
        Task<Result<int>> successResultTask = Task.FromResult(Result.Success(5));
        Task<Result<int>> failureResultTask = Task.FromResult(Result.Failure<int>(new InvalidOperationException("Initial failure")));
        static Task<Result<string>> chainFunc(int value) => Task.FromResult(Result.Success($"Value is {value}"));

        // Act
        Result<string> successBoundResult = await successResultTask.BindAsync(chainFunc);
        Result<string> failureBoundResult = await failureResultTask.BindAsync(chainFunc);

        // Assert
        successBoundResult.IsSuccess.Should().BeTrue();
        successBoundResult.Value.Should().Be("Value is 5");

        failureBoundResult.IsFailure.Should().BeTrue();
        failureBoundResult.Exception.Should().Be((await failureResultTask).Exception);
    }

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
}
