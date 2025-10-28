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
    [Fact(DisplayName = "Result.Success should create a successful result")]
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
    [Fact(DisplayName = "Result.Failure should create a failure result")]
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
    /// Tests that implicit conversion from an <see cref="Exception"/> to a non-generic <see cref="Result"/> correctly creates a failure result.
    /// </summary>
    [Fact(DisplayName = "Implicit conversion from Exception should create a failure Result")]
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
    [Fact(DisplayName = "Result<T>.Success should create a successful result with a value")]
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
    /// Tests that implicit conversion from a value to a generic <see cref="Result{T}"/> correctly creates a success result.
    /// </summary>
    [Fact(DisplayName = "Implicit conversion from a value should create a success Result<T>")]
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
    [Fact(DisplayName = "Implicit conversion from Exception should create a failure Result<T>")]
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

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> calls the action with the exception for a failed result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_SuccessfulResult_DoesNotCallAction()
    {
        // Arrange
        Task<Result<int>> successResultTask = Task.FromResult(Result.Success(42));
        bool actionCalled = false;

        // Act
        Result<int> result = await successResultTask.OnFailureAsync(async ex =>
        {
            actionCalled = true;
            await Task.CompletedTask;
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        actionCalled.Should().BeFalse();
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> calls the action with the exception for a failed result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_FailedResult_CallsActionWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        Exception? capturedException = null;

        // Act
        Result<int> result = await failedResultTask.OnFailureAsync(async ex =>
        {
            capturedException = ex;
            await Task.CompletedTask;
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        capturedException.Should().BeSameAs(exception);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> calls the action with the exception for a failed result.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_FailedResult_ReturnsOriginalResultAfterAction()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        bool actionCalled = false;

        // Act
        Result<int> result = await failedResultTask.OnFailureAsync(async ex =>
        {
            actionCalled = true;
            await Task.CompletedTask;
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        actionCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> propagates an exception thrown by the action.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_ActionThrowsException_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Original failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        var actionException = new ArgumentException("Action failed");

        // Act & Assert
        ArgumentException exceptionThrown = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await failedResultTask.OnFailureAsync(async ex =>
            {
                await Task.CompletedTask;
                throw actionException;
            }));

        exceptionThrown.Should().BeSameAs(actionException);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> correctly handles asynchronous actions within the failure handler.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithAsyncAction_CompletesSuccessfully()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        bool asyncWorkCompleted = false;

        // Act
        Result<int> result = await failedResultTask.OnFailureAsync(async ex =>
        {
            await Task.Delay(10); // Simulate some async work
            asyncWorkCompleted = true;
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(exception);
        asyncWorkCompleted.Should().BeTrue();
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> throws an <see cref="ArgumentNullException"/> when the action is null.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Task<Result<int>> successResultTask = Task.FromResult(Result.Failure<int>(new Exception()));

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await successResultTask.OnFailureAsync(null!));
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> can be chained with other operations.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_CanBeChainedWithOtherOperations()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        int sideEffectValue = 0;

        // Act
        Result<int> result = await failedResultTask
            .OnFailureAsync(async ex =>
            {
                await Task.CompletedTask;
                sideEffectValue = 100;
            });

        // Assert
        result.IsSuccess.Should().BeFalse();
        sideEffectValue.Should().Be(100);
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> does not call the action for a successful result, even if the value is null.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_SuccessfulResultWithNullValue_DoesNotCallAction()
    {
        // Arrange
        Task<Result<string>> successResultTask = Task.FromResult(Result.Success<string>(null!));
        bool actionCalled = false;

        // Act
        Result<string> result = await successResultTask.OnFailureAsync(async ex =>
        {
            actionCalled = true;
            await Task.CompletedTask;
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
        actionCalled.Should().BeFalse();
    }

    /// <summary>
    /// Tests that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> with <c>ConfigureAwait(false)</c> does not deadlock.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_ConfigureAwaitFalse_DoesNotDeadlock()
    {
        // Arrange
        var exception = new InvalidOperationException("Test failure");
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(exception));
        bool actionCalled = false;

        // Act - This test ensures ConfigureAwait(false) in the method prevents deadlocks
        Result<int> result = await failedResultTask.OnFailureAsync(async ex =>
        {
            // Simulate some work that might capture context
            await Task.Delay(1).ConfigureAwait(false);
            actionCalled = true;
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        actionCalled.Should().BeTrue();
    }
}
