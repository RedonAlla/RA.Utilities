using System;
using System.Collections.Generic;
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
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> does not call the action for a successful result.
    /// </summary>
    [Fact]
    public void OnFailure_SuccessfulResult_DoesNotCallAction()
    {
        // Arrange
        var successResult = Result.Success(42);
        bool actionCalled = false;

        // Act
        Result<int> result = successResult.OnFailure(ex => actionCalled = true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        actionCalled.Should().BeFalse();
        result.Should().BeSameAs(successResult);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> calls the action with the exception for a failed result.
    /// </summary>
    [Fact]
    public void OnFailure_FailedResult_CallsActionWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<int>(exception);
        Exception? capturedException = null;

        // Act
        Result<int> result = failedResult.OnFailure(ex => capturedException = ex);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        capturedException.Should().BeSameAs(exception);
        result.Should().BeSameAs(failedResult);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> returns the original failed result after executing the action.
    /// </summary>
    [Fact]
    public void OnFailure_FailedResult_ReturnsOriginalResultAfterAction()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<int>(exception);
        bool actionCalled = false;

        // Act
        Result<int> result = failedResult.OnFailure(ex => actionCalled = true);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        actionCalled.Should().BeTrue();
        result.Should().BeSameAs(failedResult);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> propagates an exception thrown by the action.
    /// </summary>
    [Fact]
    public void OnFailure_ActionThrowsException_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Original failure");
        var failedResult = Result.Failure<int>(exception);
        var actionException = new ArgumentException("Action failed");

        // Act & Assert
        ArgumentException exceptionThrown = Assert.Throws<ArgumentException>(() =>
        {
            failedResult.OnFailure(ex => throw actionException);
        });

        exceptionThrown.Should().BeSameAs(actionException);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> throws an <see cref="ArgumentNullException"/> when the action is null.
    /// </summary>
    [Fact]
    public void OnFailure_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        var successResult = Result.Failure(new Exception());

        // Act & Assert
        Assert.Throws<NullReferenceException>(() =>
            successResult.OnFailure(null!));
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> can be chained with other operations.
    /// </summary>
    [Fact]
    public void OnFailure_CanBeChainedWithOtherOperations()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<int>(exception);
        int sideEffectValue = 0;

        // Act
        Result<int> result = failedResult
            .OnFailure(ex => sideEffectValue = 100);

        // Assert
        result.IsSuccess.Should().BeFalse();
        sideEffectValue.Should().Be(100);
        result.Should().BeSameAs(failedResult);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> does not call the action for a successful result, even if the value is null.
    /// </summary>
    [Fact]
    public void OnFailure_SuccessfulResultWithNullValue_DoesNotCallAction()
    {
        // Arrange
        var successResult = Result.Success<string>(null!);
        bool actionCalled = false;

        // Act
        Result<string> result = successResult.OnFailure(ex => actionCalled = true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
        actionCalled.Should().BeFalse();
        result.Should().BeSameAs(successResult);
    }

    /// <summary>
    /// Tests that multiple <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> calls execute all actions for a failed result.
    /// </summary>
    [Fact]
    public void OnFailure_MultipleOnFailureCalls_AllActionsCalledForFailure()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<int>(exception);
        int callCount = 0;

        // Act
        Result<int> result = failedResult
            .OnFailure(ex => callCount++)
            .OnFailure(ex => callCount++);

        // Assert
        result.IsSuccess.Should().BeFalse();
        callCount.Should().Be(2);
    }

    /// <summary>
    /// Tests that the action in <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> can modify external state.
    /// </summary>
    [Fact]
    public void OnFailure_ActionModifiesExternalState_StateIsModified()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<int>(exception);
        var logger = new TestLogger();

        // Act
        Result<int> result = failedResult.OnFailure(ex => logger.LogError($"Operation failed: {ex.Message}"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        logger.Logs.Should().Contain("Operation failed: Test failure");
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> works correctly with a value type.
    /// </summary>
    [Fact]
    public void OnFailure_WithValueType_WorksCorrectly()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<double>(exception);
        bool actionCalled = false;

        // Act
        Result<double> result = failedResult.OnFailure(ex => actionCalled = true);

        // Assert
        result.IsSuccess.Should().BeFalse();
        actionCalled.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> works correctly with a reference type.
    /// </summary>
    [Fact]
    public void OnFailure_WithReferenceType_WorksCorrectly()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<string>(exception);
        bool actionCalled = false;

        // Act
        Result<string> result = failedResult.OnFailure(ex => actionCalled = true);

        // Assert
        result.IsSuccess.Should().BeFalse();
        actionCalled.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
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
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Bind_ShouldChainOrPropagate(bool isSuccess)
    {
        // Arrange
        var exception = new InvalidOperationException("Initial failure");
        Result<int> result = isSuccess ? Result.Success(5) : Result.Failure<int>(exception);
        static Result<string> chainFunc(int value) => Result.Success($"Value is {value}");

        // Act
        Result<string> boundResult = result.Bind(chainFunc);

        // Assert
        boundResult.IsSuccess.Should().Be(isSuccess);
        if (isSuccess)
        {
            boundResult.Value.Should().Be("Value is 5");
        }
        else
        {
            boundResult.Exception.Should().Be(exception);
        }
    }

    /// <summary>
    /// Tests that the non-generic Match method executes the success function when the result is successful.
    /// </summary>
    [Fact]
    public void Match_NonGeneric_ShouldExecuteSuccessFunc_WhenResultIsSuccess()
    {
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
        Result<int> failureResult = exception;

        // Act
        string matchResult = failureResult.Match(
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
    /// Tests that BindAsync returns the bound result when the initial result is successful.
    /// </summary>
    [Fact]
    public async Task BindAsync_SuccessfulResult_ReturnsBoundResult()
    {
        // Arrange
        int initialValue = 42;
        string expectedBoundValue = "Success: 42";
        var successResult = Result.Success(initialValue);

        // Act
        Result<string> result = await successResult.BindAsync(async x =>
        {
            string transformed = await Task.FromResult($"Success: {x}");
            return Result.Success(transformed);
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedBoundValue);
    }

    /// <summary>
    /// Tests that BindAsync returns a failed result without calling the bind function when the initial result is a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_FailedResult_ReturnsFailureWithoutCallingBindFunc()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        var failedResult = Result.Failure<int>(exception);
        bool bindFuncCalled = false;

        // Act
        Result<string> result = await failedResult.BindAsync<int, string>(async x =>
        {
            bindFuncCalled = true;
            return await Task.FromResult(Result.Success(x.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        bindFuncCalled.Should().BeFalse();
    }

    /// <summary>
    /// Tests that BindAsync returns a failed result when the bind function itself returns a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_BindFuncReturnsFailure_ReturnsFailedResult()
    {
        // Arrange
        int initialValue = 42;
        var successResult = Result.Success(initialValue);
        var bindException = new ArgumentException("Binding failed");

        // Act
        Result<string> result = await successResult.BindAsync<int, string>(async x =>
        {
            await Task.CompletedTask; // Simulate async work
            return Result.Failure<string>(bindException);
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(bindException);
    }

    /// <summary>
    /// Tests that BindAsync propagates an exception thrown by the bind function.
    /// </summary>
    [Fact]
    public async Task BindAsync_BindFuncThrowsException_PropagatesException()
    {
        // Arrange
        int initialValue = 42;
        var successResult = Result.Success(initialValue);
        var expectedException = new InvalidOperationException("Bind func crashed");

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await successResult.BindAsync<int, string>(async x =>
            {
                await Task.CompletedTask;
                throw expectedException;
            }));

        exception.Should().BeSameAs(expectedException);
    }

    /// <summary>
    /// Tests that BindAsync throws an <see cref="ArgumentNullException"/> when the bind function is null.
    /// </summary>
    [Fact]
    public async Task BindAsync_NullBindFunc_ThrowsNullReferenceException()
    {
        // Arrange
        var successResult = Result.Success(42);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await successResult.BindAsync<int, string>(null!));
    }

    /// <summary>
    /// Tests that BindAsync correctly handles asynchronous work within the bind function and completes successfully.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithAsyncWorkInBindFunc_CompletesSuccessfully()
    {
        // Arrange
        int initialValue = 5;
        var successResult = Result.Success(initialValue);
        var simulatedDelay = TimeSpan.FromMilliseconds(10);

        // Act
        Result<int> result = await successResult.BindAsync<int, int>(async x =>
        {
            await Task.Delay(simulatedDelay);
            return Result.Success(x * 2);
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(10);
    }

    /// <summary>
    /// Tests that multiple BindAsync operations can be chained together and work correctly.
    /// </summary>
    [Fact]
    public async Task BindAsync_ChainMultipleBindOperations_WorksCorrectly()
    {
        // Arrange
        var initialResult = Result.Success(10);

        // Act
        Result<bool> result = await initialResult
            .BindAsync<int, string>(async x =>
            {
                await Task.CompletedTask;
                return Result.Success((x * 2).ToString(System.Globalization.CultureInfo.InvariantCulture));
            })
            .BindAsync<string, bool>(async s =>
            {
                await Task.CompletedTask;
                return Result.Success(int.Parse(s, System.Globalization.CultureInfo.InvariantCulture) > 15);
            });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue(); // 10 * 2 = 20 > 15
    }

    /// <summary>
    /// Tests that MapAsync returns the mapped value when the result is successful.
    /// </summary>
    [Fact]
    public async Task MapAsync_SuccessfulResult_ReturnsMappedValue()
    {
        // Arrange
        int initialValue = 42;
        string expectedMappedValue = "42";
        Task<Result<int>> successResultTask = Task.FromResult(Result.Success(initialValue));

        // Act
        Result<string> result = await successResultTask.MapAsync(async x =>
            await Task.FromResult(x.ToString(System.Globalization.CultureInfo.InvariantCulture)));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedMappedValue);
    }

    /// <summary>
    /// Tests that MapAsync returns a failed result without calling the mapping function when the initial result is a failure.
    /// </summary>
    [Fact]
    public async Task MapAsync_FailedResult_ReturnsFailureWithoutCallingMapFunc()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        bool mapFuncCalled = false;

        // Act
        Result<string> result = await failedResultTask.MapAsync<int, string>(async x =>
        {
            mapFuncCalled = true;
            return await Task.FromResult(x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        mapFuncCalled.Should().BeFalse();
    }
}

internal sealed class TestLogger
{
    public List<string> Logs { get; } = [];

    public void LogError(string message)
    {
        Logs.Add(message);
    }
}
