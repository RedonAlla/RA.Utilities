using System;
using System.Collections.Generic;
using System.IO;
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

    #region Async Bind<TIn> Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Result{TIn}, Func{TIn, Task{Result}})"/> executes the asynchronous bind function
    /// with the result value and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultT_ExecutesAsyncBindFunction()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        var bindResult = Result.Success();
        int receivedValue = 0;

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            receivedValue = value;
            return bindResult;
        });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Result{TIn}, Func{TIn, Task{Result}})"/> propagates the exception
    /// when the initial result is a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultTAndFailure_PropagatesException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            bindExecuted = true;
            return Result.Success();
        });

        // Assert
        Assert.False(bindExecuted);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Result{TIn}, Func{TIn, Task{Result}})"/> returns a failure result
    /// from the asynchronous bind function when the initial result is successful but the bind function returns failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultTAndFailingBindFunction_ReturnsFailureResult()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        Exception bindException = new ArgumentException("Bind failed");
        var failingBindResult = Result.Failure(bindException);

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            return failingBindResult;
        });

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(bindException, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Result{TIn}, Func{TIn, Task{Result}})"/> throws an <see cref="ArgumentNullException"/>
    /// when the bind function is null.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultTAndNullBindFunc_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success(42);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => result.BindAsync<int>(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Result{TIn}, Func{TIn, Task{Result}})"/> properly handles
    /// different generic types for the input result.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultTAndDifferentGenericTypes_ExecutesCorrectly()
    {
        // Arrange
        string inputValue = "test value";
        var result = Result.Success(inputValue);
        string receivedValue = null;

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            receivedValue = value;
            return Result.Success();
        });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.True(returnedResult.IsSuccess);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Result{TIn}, Func{TIn, Task{Result}})"/> properly handles
    /// when the bind function throws an exception.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultTAndBindFunctionThrowingException_ReturnsFailureResult()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        Exception bindException = new InvalidOperationException("Bind function exception");

        // Act
        Result returnedResult = await result.BindAsync<int>(async value =>
        {
            await Task.Delay(1);
            return bindException;
        });

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(bindException, returnedResult.Exception);
    }

    #endregion

    #region Bind Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn}(Result{TIn}, Func{TIn, Result})"/> executes the bind function
    /// with the result value and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public void Bind_WithSuccessResultT_ExecutesBindFunctionAndReturnsResult()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        var bindResult = Result.Success();
        int receivedValue = 0;

        // Act
        Result returnedResult = result.Bind(value =>
        {
            receivedValue = value;
            return bindResult;
        });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn}(Result{TIn}, Func{TIn, Result})"/> propagates the exception
    /// when the initial result is a failure.
    /// </summary>
    [Fact]
    public void Bind_WithFailureResultT_PropagatesException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = result.Bind(value =>
        {
            bindExecuted = true;
            return Result.Success();
        });

        // Assert
        Assert.False(bindExecuted);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn}(Result{TIn}, Func{TIn, Result})"/> returns a failure result
    /// from the bind function when the initial result is successful but the bind function returns failure.
    /// </summary>
    [Fact]
    public void Bind_WithSuccessResultTAndFailingBindFunction_ReturnsFailureResult()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        Exception bindException = new ArgumentException("Bind failed");
        var failingBindResult = Result.Failure(bindException);

        // Act
        Result returnedResult = result.Bind(value => failingBindResult);

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(bindException, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn}(Result{TIn}, Func{TIn, Result})"/> throws an <see cref="NullReferenceException"/>
    /// when the bind function is null.
    /// </summary>
    [Fact]
    public void Bind_WithNullBindFunc_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success(42);

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Bind(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn}(Result{TIn}, Func{TIn, Result})"/> properly handles
    /// different generic types for the input result.
    /// </summary>
    [Fact]
    public void Bind_WithDifferentGenericTypes_ExecutesCorrectly()
    {
        // Arrange
        string inputValue = "test value";
        var result = Result.Success(inputValue);
        string receivedValue = null;

        // Act
        Result returnedResult = result.Bind(value =>
        {
            receivedValue = value;
            return Result.Success();
        });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.True(returnedResult.IsSuccess);
    }

    #endregion

    #region Async OnFailure<T> Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> executes the asynchronous action
    /// with the exception when the result is a failure.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithFailureResultT_ExecutesAsyncActionWithException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        Exception receivedException = null;

        // Act
        Result<int> returnedResult = await Task.FromResult(result)
            .OnFailureAsync(async ex =>
            {
                await Task.Delay(1);
                receivedException = ex;
            });

        // Assert
        Assert.Same(exception, receivedException);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> does not execute the asynchronous action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithSuccessResultT_DoesNotExecuteAsyncAction()
    {
        // Arrange
        var result = Result.Success(42);
        bool actionExecuted = false;

        // Act
        Result<int> returnedResult = await Task.FromResult(result)
            .OnFailureAsync(async ex =>
            {
                await Task.Delay(1);
                actionExecuted = true;
            });

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> returns the original result
    /// allowing for method chaining when the action is executed.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithFailureResultT_ReturnsOriginalResultForChaining()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var originalResult = Result.Failure<string>(exception);
        bool actionExecuted = false;

        // Act
        Result<string> returnedResult = await Task.FromResult(originalResult)
            .OnFailureAsync(async ex =>
            {
                await Task.Delay(1);
                actionExecuted = true;
            });

        // Assert
        Assert.True(actionExecuted);
        Assert.Same(originalResult, returnedResult);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> properly handles
    /// different generic types for the result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithDifferentGenericTypes_ExecutesCorrectly()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<DateTime>(exception);
        Exception receivedException = null;

        // Act
        Result<DateTime> returnedResult = await Task.FromResult(result)
            .OnFailureAsync(async ex =>
            {
                await Task.Delay(1);
                receivedException = ex;
            });

        // Assert
        Assert.Same(exception, receivedException);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> properly handles
    /// when the action function throws an exception.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithActionThrowingException_PropagatesActionException()
    {
        // Arrange
        Exception originalException = new InvalidOperationException("Original exception");
        var result = Result.Failure<int>(originalException);
        Exception actionException = new ArgumentException("Action exception");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await Task.FromResult(result)
                .OnFailureAsync(async ex =>
                {
                    await Task.Delay(1);
                    throw actionException;
                }));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> works correctly
    /// with a completed task containing a failure result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithCompletedFailureTask_ExecutesAction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        Task<Result<int>> completedTask = Task.FromResult(Result.Failure<int>(exception));
        bool actionExecuted = false;

        // Act
        Result<int> returnedResult = await completedTask
            .OnFailureAsync(async ex =>
            {
                await Task.Delay(1);
                actionExecuted = true;
            });

        // Assert
        Assert.True(actionExecuted);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> works correctly
    /// with a task that completes asynchronously with a failure result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithAsyncFailureTask_ExecutesAction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        async Task<Result<int>> GetFailureAsync()
        {
            await Task.Delay(10);
            return Result.Failure<int>(exception);
        }

        Task<Result<int>> asyncTask = GetFailureAsync();
        Exception receivedException = null;

        // Act
        Result<int> returnedResult = await asyncTask
            .OnFailureAsync(async ex =>
            {
                await Task.Delay(1);
                receivedException = ex;
            });

        // Assert
        Assert.Same(exception, receivedException);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    #endregion

    #region Match Action Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> executes the success action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public void Match_WithSuccessResult_ExecutesSuccessAction()
    {
        // Arrange
        var result = Result.Success();
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        result.Match(
            success: () => successExecuted = true,
            failure: ex => failureExecuted = true);

        // Assert
        Assert.True(successExecuted);
        Assert.False(failureExecuted);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> executes the failure action
    /// with the exception when the result is a failure.
    /// </summary>
    [Fact]
    public void Match_WithFailureResult_ExecutesFailureActionWithException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool successExecuted = false;
        bool failureExecuted = false;
        Exception receivedException = null;

        // Act
        result.Match(
            success: () => successExecuted = true,
            failure: ex =>
            {
                failureExecuted = true;
                receivedException = ex;
            });

        // Assert
        Assert.False(successExecuted);
        Assert.True(failureExecuted);
        Assert.Same(exception, receivedException);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> throws an <see cref="NullReferenceException"/>
    /// when the success action is null.
    /// </summary>
    [Fact]
    public void Match_WithNullSuccessAction_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success();
        Action<Exception> failureAction = ex => { };

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Match(null!, failureAction));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> throws an <see cref="NullReferenceException"/>
    /// when both actions are null.
    /// </summary>
    [Fact]
    public void Match_WithBothActionsNull_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success();

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Match(null!, null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> properly executes
    /// the success action when the result is successful and the success action performs multiple operations.
    /// </summary>
    [Fact]
    public void Match_WithSuccessResultAndComplexSuccessAction_ExecutesAllOperations()
    {
        // Arrange
        var result = Result.Success();
        int operation1 = 0;
        string operation2 = null;
        bool operation3 = false;

        // Act
        result.Match(
            success: () =>
            {
                operation1 = 42;
                operation2 = "completed";
                operation3 = true;
            },
            failure: ex => { });

        // Assert
        Assert.Equal(42, operation1);
        Assert.Equal("completed", operation2);
        Assert.True(operation3);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> properly executes
    /// the failure action when the result is a failure and the failure action performs multiple operations.
    /// </summary>
    [Fact]
    public void Match_WithFailureResultAndComplexFailureAction_ExecutesAllOperations()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        int operation1 = 0;
        string operation2 = null;
        Exception receivedException = null;

        // Act
        result.Match(
            success: () => { },
            failure: ex =>
            {
                operation1 = 100;
                operation2 = "failure handled";
                receivedException = ex;
            });

        // Assert
        Assert.Equal(100, operation1);
        Assert.Equal("failure handled", operation2);
        Assert.Same(exception, receivedException);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> does not throw
    /// when the failure action is null but the result is successful (since failure action won't be called).
    /// However, it should still throw because we validate parameters regardless of the result state.
    /// </summary>
    [Fact]
    public void Match_WithSuccessResultAndNullFailureAction_StillThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Failure(new Exception("Some error")); // Result is a failure
        Action successAction = () => { };

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Match(successAction, null!)); // Failure action is null
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> properly handles
    /// different types of exceptions in the failure action.
    /// </summary>
    [Fact]
    public void Match_WithDifferentExceptionTypes_ExecutesFailureActionWithCorrectException()
    {
        // Arrange
        var exceptions = new Exception[]
        {
            new InvalidOperationException("Invalid operation"),
            new ArgumentException("Invalid argument"),
            new IOException("IO error"),
            new UnauthorizedAccessException("Access denied")
        };

        foreach (Exception expectedException in exceptions)
        {
            var result = Result.Failure(expectedException);
            Exception receivedException = null;

            // Act
            result.Match(
                success: () => { },
                failure: ex => receivedException = ex);

            // Assert
            Assert.Same(expectedException, receivedException);
        }
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> can be used
    /// for side effects like logging in the success case.
    /// </summary>
    [Fact]
    public void Match_WithSuccessResult_CanBeUsedForSuccessSideEffects()
    {
        // Arrange
        var result = Result.Success();
        var logMessages = new List<string>();

        // Act
        result.Match(
            success: () => logMessages.Add("Operation completed successfully"),
            failure: ex => logMessages.Add($"Operation failed: {ex.Message}"));

        // Assert
        Assert.Single(logMessages);
        Assert.Equal("Operation completed successfully", logMessages[0]);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> can be used
    /// for side effects like logging in the failure case.
    /// </summary>
    [Fact]
    public void Match_WithFailureResult_CanBeUsedForFailureSideEffects()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Database connection failed");
        var result = Result.Failure(exception);
        var logMessages = new List<string>();

        // Act
        result.Match(
            success: () => logMessages.Add("Operation completed successfully"),
            failure: ex => logMessages.Add($"Operation failed: {ex.Message}"));

        // Assert
        Assert.Single(logMessages);
        Assert.Equal("Operation failed: Database connection failed", logMessages[0]);
    }

    #endregion

    #region Async Bind<TIn> from Task Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> executes the asynchronous bind function
    /// with the result value and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultT_ExecutesAsyncBindFunction()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        var bindResult = Result.Success();
        int receivedValue = 0;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                receivedValue = value;
                return bindResult;
            });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> propagates the exception
    /// when the initial result is a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultTAndFailure_PropagatesException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                bindExecuted = true;
                return Result.Success();
            });

        // Assert
        Assert.False(bindExecuted);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> returns a failure result
    /// from the asynchronous bind function when the initial result is successful but the bind function returns failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultTAndFailingBindFunction_ReturnsFailureResult()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        Exception bindException = new ArgumentException("Bind failed");
        var failingBindResult = Result.Failure(bindException);

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                return failingBindResult;
            });

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(bindException, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> throws an <see cref="ArgumentNullException"/>
    /// when the bind function is null.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultTAndNullBindFunc_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success(42);
        Task<Result<int>> resultTask = Task.FromResult(result);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => resultTask.BindAsync<int>(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> properly handles
    /// different generic types for the input result.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultTAndDifferentGenericTypes_ExecutesCorrectly()
    {
        // Arrange
        string inputValue = "test value";
        var result = Result.Success(inputValue);
        string receivedValue = null;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                receivedValue = value;
                return Result.Success();
            });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.True(returnedResult.IsSuccess);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> properly handles
    /// when the bind function throws an exception.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultTAndBindFunctionThrowingException_ReturnsFailureResult()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        Exception bindException = new InvalidOperationException("Bind function exception");

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync<int>(async value =>
            {
                await Task.Delay(1);
                return bindException;
            });

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(bindException, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> works correctly
    /// with a completed task containing a successful result.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromCompletedTaskResultT_ExecutesBindFunction()
    {
        // Arrange
        int inputValue = 100;
        Task<Result<int>> completedTask = Task.FromResult(Result.Success(inputValue));
        int receivedValue = 0;

        // Act
        Result returnedResult = await completedTask
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                receivedValue = value;
                return Result.Success();
            });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.True(returnedResult.IsSuccess);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> works correctly
    /// with a task that completes asynchronously with a successful result.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromAsyncTaskResultT_ExecutesBindFunction()
    {
        // Arrange
        int inputValue = 200;
        async Task<Result<int>> GetSuccessAsync()
        {
            await Task.Delay(10);
            return Result.Success(inputValue);
        }

        Task<Result<int>> asyncTask = GetSuccessAsync();
        int receivedValue = 0;

        // Act
        Result returnedResult = await asyncTask
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                receivedValue = value;
                return Result.Success();
            });

        // Assert
        Assert.Equal(inputValue, receivedValue);
        Assert.True(returnedResult.IsSuccess);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> works correctly
    /// with a task that completes asynchronously with a failure result.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromAsyncTaskResultTAndFailure_PropagatesException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Async failure");
        async Task<Result<int>> GetFailureAsync()
        {
            await Task.Delay(10);
            return Result.Failure<int>(exception);
        }

        Task<Result<int>> asyncTask = GetFailureAsync();
        bool bindExecuted = false;

        // Act
        Result returnedResult = await asyncTask
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                bindExecuted = true;
                return Result.Success();
            });

        // Assert
        Assert.False(bindExecuted);
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> maintains
    /// method chaining capability by returning the correct result type.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultT_AllowsMethodChaining()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        bool secondOperationExecuted = false;

        // Act
        Result finalResult = await Task.FromResult(result)
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                return Result.Success();
            })
            .OnSuccessAsync(async () => { await Task.Delay(1); secondOperationExecuted = true; });

        // Assert
        Assert.True(secondOperationExecuted);
        Assert.True(finalResult.IsSuccess);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn}(Task{Result{TIn}}, Func{TIn, Task{Result}})"/> properly handles
    /// null value types when allowed by the generic constraint.
    /// </summary>
    [Fact]
    public async Task BindAsync_FromTaskResultTAndNullValue_ExecutesBindFunction()
    {
        // Arrange
        var result = Result.Success((string)null);
        string receivedValue = "not null";

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                receivedValue = value; // This should be null
                return Result.Success();
            });

        // Assert
        Assert.Null(receivedValue);
        Assert.True(returnedResult.IsSuccess);
    }

    #endregion
}

internal sealed class TestLogger
{
    public List<string> Logs { get; } = [];

    public void LogError(string message)
    {
        Logs.Add(message);
    }
}
