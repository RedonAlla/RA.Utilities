using System;
using System.Threading.Tasks;
using Xunit;
using RA.Utilities.Core.Results;
using RA.Utilities.Tests.Utilities;

namespace RA.Utilities.Core.Tests.Results;

/// <summary>
/// Contains unit tests for the <see cref="ResultExtensions"/> class.
/// Tests the functional-style extension methods for handling operation outcomes.
/// </summary>
public class ResultExtensionsTests
{
    #region OnSuccess Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccess(Result, Action)"/> executes the provided action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public void OnSuccess_WithSuccessResult_ExecutesAction()
    {
        // Arrange
        var result = Result.Success();
        bool actionExecuted = false;

        // Act
        Result returnedResult = result.OnSuccess(() => actionExecuted = true);

        // Assert
        Assert.True(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccess(Result, Action)"/> does not execute the provided action
    /// when the result is a failure.
    /// </summary>
    [Fact]
    public void OnSuccess_WithFailureResult_DoesNotExecuteAction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool actionExecuted = false;

        // Act
        Result returnedResult = result.OnSuccess(() => actionExecuted = true);

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccess{T}(Result{T}, Action{T})"/> executes the provided action
    /// with the result value when the result is successful.
    /// </summary>
    [Fact]
    public void OnSuccess_WithSuccessResultT_ExecutesActionWithValue()
    {
        // Arrange
        int testValue = 42;
        var result = Result.Success(testValue);
        int receivedValue = 0;

        // Act
        Result<int> returnedResult = result.OnSuccess(value => receivedValue = value);

        // Assert
        Assert.Equal(testValue, receivedValue);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccess{T}(Result{T}, Action{T})"/> does not execute the provided action
    /// when the result is a failure.
    /// </summary>
    [Fact]
    public void OnSuccess_WithFailureResultT_DoesNotExecuteAction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        bool actionExecuted = false;

        // Act
        Result<int> returnedResult = result.OnSuccess(value => actionExecuted = true);

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccess(Result, Action)"/> throws an <see cref="NullReferenceException"/>
    /// when the action is null.
    /// </summary>
    [Fact]
    public void OnSuccess_WithNullAction_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success();

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.OnSuccess(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccess{T}(Result{T}, Action{T})"/> throws an <see cref="NullReferenceException"/>
    /// when the action is null.
    /// </summary>
    [Fact]
    public void OnSuccess_WithValueAndNullAction_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success(42);

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.OnSuccess((Action<int>)null!));
    }

    #endregion

    #region OnFailure Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> executes the provided action
    /// with the exception when the result is a failure.
    /// </summary>
    [Fact]
    public void OnFailure_WithFailureResult_ExecutesActionWithException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        Exception receivedException = null;

        // Act
        Result returnedResult = result.OnFailure(ex => receivedException = ex);

        // Assert
        Assert.Same(exception, receivedException);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> does not execute the provided action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public void OnFailure_WithSuccessResult_DoesNotExecuteAction()
    {
        // Arrange
        var result = Result.Success();
        bool actionExecuted = false;

        // Act
        Result returnedResult = result.OnFailure(ex => actionExecuted = true);

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> executes the provided action
    /// with the exception when the result is a failure.
    /// </summary>
    [Fact]
    public void OnFailure_WithFailureResultT_ExecutesActionWithException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        Exception receivedException = null;

        // Act
        Result<int> returnedResult = result.OnFailure(ex => receivedException = ex);

        // Assert
        Assert.Same(exception, receivedException);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> does not execute the provided action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public void OnFailure_WithSuccessResultT_DoesNotExecuteAction()
    {
        // Arrange
        var result = Result.Success(42);
        bool actionExecuted = false;

        // Act
        Result<int> returnedResult = result.OnFailure(ex => actionExecuted = true);

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> throws an <see cref="NullReferenceException"/>
    /// when the action is null.
    /// </summary>
    [Fact]
    public void OnFailure_WithNullAction_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Failure(new Exception());

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.OnFailure(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> throws an <see cref="NullReferenceException"/>
    /// when the action is null.
    /// </summary>
    [Fact]
    public void OnFailure_WithValueAndNullAction_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Failure<int>(new Exception());

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.OnFailure(null!));
    }

    #endregion

    #region Map Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> transforms the value
    /// when the result is successful.
    /// </summary>
    [Theory]
    [InlineData(5, 10)]
    [InlineData(0, 0)]
    [InlineData(-5, -10)]
    public void Map_WithSuccessResult_TransformsValue(int input, int expected)
    {
        // Arrange
        var result = Result.Success(input);

        // Act
        Result<int> mappedResult = result.Map(x => x * 2);

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(expected, mappedResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> transforms between different types
    /// when the result is successful.
    /// </summary>
    [Fact]
    public void Map_WithSuccessResult_TransformsToDifferentType()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);

        // Act
        Result<string> mappedResult = result.Map(x => x.ToString());

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal("42", mappedResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> propagates the exception
    /// when the result is a failure.
    /// </summary>
    [Fact]
    public void Map_WithFailureResult_PropagatesException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);

        // Act
        Result<int> mappedResult = result.Map(x => x * 2);

        // Assert
        Assert.True(mappedResult.IsFailure);
        Assert.Same(exception, mappedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> throws an <see cref="NullReferenceException"/>
    /// when the map function is null.
    /// </summary>
    [Fact]
    public void Map_WithNullMapFunc_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success(42);

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Map<int, string>(null!));
    }

    #endregion

    #region Bind Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind(Result, Func{Result})"/> executes the bind function
    /// and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public void Bind_WithSuccessResult_ExecutesBindFunction()
    {
        // Arrange
        var result = Result.Success();
        var bindResult = Result.Success();
        bool bindExecuted = false;

        // Act
        Result returnedResult = result.Bind(() =>
        {
            bindExecuted = true;
            return bindResult;
        });

        // Assert
        Assert.True(bindExecuted);
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind(Result, Func{Result})"/> does not execute the bind function
    /// and returns the original failure when the initial result is a failure.
    /// </summary>
    [Fact]
    public void Bind_WithFailureResult_DoesNotExecuteBindFunction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = result.Bind(() =>
        {
            bindExecuted = true;
            return Result.Success();
        });

        // Assert
        Assert.False(bindExecuted);
        Assert.Same(result, returnedResult);
    }

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
    public void Bind_WithFailureResultT_PropagatesException_NonGenericOutput()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);

        // Act
        Result returnedResult = result.Bind(value => Result.Success());

        // Assert
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
    /// Verifies that <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> executes the bind function
    /// with the result value and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public void Bind_WithSuccessResultT_ReturnsNewResult()
    {
        // Arrange
        int inputValue = 5;
        var result = Result.Success(inputValue);

        // Act
        Result<string> returnedResult = result.Bind<int, string>(value =>
            Result.Success(value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

        // Assert
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal("5", returnedResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> executes the bind function
    /// with the result value and returns its result when the initial result is successful, and the bind function returns a different type.
    /// </summary>
    [Fact]
    public void Bind_WithSuccessResultT_ExecutesBindFunctionWithDifferentOutput()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        int receivedValue = 0;
        string expectedOutput = "42_processed";

        // Act
        Result<string> returnedResult = result.Bind(value =>
        {
            receivedValue = value;
            return Result.Success($"{value}_processed");
        });

        // Assert
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(inputValue, receivedValue);
        Assert.Equal(expectedOutput, returnedResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> propagates the exception
    /// when the initial result is a failure.
    /// </summary>
    [Fact]
    public void Bind_WithFailureResultT_PropagatesException()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);

        // Act
        Result<string> returnedResult = result.Bind(value =>
            Result.Success(value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
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
        Assert.Throws<NullReferenceException>(() => result.Bind((Func<int, Result>)null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> throws an <see cref="NullReferenceException"/>
    /// when the bind function is null.
    /// </summary>
    [Fact]
    public void Bind_WithNullBindFuncGeneric_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Success(42);

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Bind<int, string>(null!));
    }

    #endregion

    #region Match Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> executes the correct action
    /// based on whether the result is successful or a failure.
    /// </summary>
    [Fact]
    public void Match_WithActions_ExecutesCorrectAction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        result.Match(
            success: () => successExecuted = true,
            failure: ex => failureExecuted = true);

        // Assert
        Assert.False(successExecuted);
        Assert.True(failureExecuted);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match{TContract}(Result, Func{TContract}, Func{Exception, TContract})"/>
    /// executes the success function and returns its result when the result is successful.
    /// </summary>
    [Fact]
    public void Match_WithSuccessResult_ExecutesSuccessFunction()
    {
        // Arrange
        var result = Result.Success();
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        string matchResult = result.Match(
            success: () => { successExecuted = true; return "success"; },
            failure: ex => { failureExecuted = true; return "failure"; });

        // Assert
        Assert.True(successExecuted);
        Assert.False(failureExecuted);
        Assert.Equal("success", matchResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match{TContract}(Result, Func{TContract}, Func{Exception, TContract})"/>
    /// executes the failure function and returns its result when the result is a failure.
    /// </summary>
    [Fact]
    public void Match_WithFailureResult_ExecutesFailureFunction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        string matchResult = result.Match(
            success: () => { successExecuted = true; return "success"; },
            failure: ex => { failureExecuted = true; return "failure"; });

        // Assert
        Assert.False(successExecuted);
        Assert.True(failureExecuted);
        Assert.Equal("failure", matchResult);
    }

    /// <summary>
    /// Verifies that <see cref="Result{TResult}.Match{TContract}(Func{TResult, TContract}, Func{Exception, TContract})"/>
    /// executes the success function and returns its result when the result is successful.
    /// </summary>
    [Fact]
    public void Match_WithSuccessResultT_ExecutesSuccessFunction()
    {
        // Arrange
        var result = Result.Success(42);
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        string matchResult = result.Match(
            success: value => { successExecuted = true; return "success"; },
            failure: ex => { failureExecuted = true; return "failure"; });

        // Assert
        Assert.True(successExecuted);
        Assert.False(failureExecuted);
        Assert.Equal("success", matchResult);
    }

    /// <summary>
    /// Verifies that <see cref="Result{TResult}.Match{TContract}(Func{TResult, TContract}, Func{Exception, TContract})"/>
    /// executes the failure function and returns its result when the result is a failure.
    /// </summary>
    [Fact]
    public void Match_WithFailureResultT_ExecutesFailureFunction()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        string matchResult = result.Match(
            success: value => { successExecuted = true; return "success"; },
            failure: ex => { failureExecuted = true; return "failure"; });

        // Assert
        Assert.False(successExecuted);
        Assert.True(failureExecuted);
        Assert.Equal("failure", matchResult);
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

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Match(null!, ex => { }));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> throws an <see cref="NullReferenceException"/>
    /// when the failure action is null.
    /// </summary>
    [Fact]
    public void Match_WithNullFailureAction_ThrowsNullReferenceException()
    {
        // Arrange
        var result = Result.Failure(new Exception());

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => result.Match(() => { }, null!));
    }

    #endregion

    #region Async OnSuccess Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> executes the asynchronous action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithSuccessResult_ExecutesAsyncAction_Async()
    {
        // Arrange
        var result = Result.Success();
        bool actionExecuted = false;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .OnSuccessAsync(async () =>
            {
                await Task.Delay(1);
                actionExecuted = true;
            });

        // Assert
        Assert.True(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> does not execute the asynchronous action
    /// when the result is a failure.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithFailureResult_DoesNotExecuteAsyncAction_Async()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool actionExecuted = false;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .OnSuccessAsync(async () =>
            {
                await Task.Delay(1);
                actionExecuted = true;
            });

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync{T}(Task{Result{T}}, Func{T, Task})"/> executes the asynchronous action
    /// with the result value when the result is successful.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithSuccessResultT_ExecutesAsyncActionWithValue_Async()
    {
        // Arrange
        int testValue = 42;
        var result = Result.Success(testValue);
        int receivedValue = 0;

        // Act
        Result<int> returnedResult = await Task.FromResult(result)
            .OnSuccessAsync(async value =>
            {
                await Task.Delay(1);
                receivedValue = value;
            });

        // Assert
        Assert.Equal(testValue, receivedValue);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync{T}(Task{Result{T}}, Func{T, Task})"/> does not execute the asynchronous action
    /// when the result is a failure.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithFailureResultT_DoesNotExecuteAsyncAction_Async()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);
        bool actionExecuted = false;

        // Act
        Result<int> returnedResult = await Task.FromResult(result)
            .OnSuccessAsync(async value =>
            {
                await Task.Delay(1);
                actionExecuted = true;
            });

        // Assert
        Assert.False(actionExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> throws an <see cref="NullReferenceException"/>
    /// when the asynchronous action is null.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithNullAction_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result> result = Task.FromResult(Result.Success());

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => result.OnSuccessAsync(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync{T}(Task{Result{T}}, Func{T, Task})"/> throws an <see cref="NullReferenceException"/>
    /// when the asynchronous action is null.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithValueAndNullAction_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => result.OnSuccessAsync(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> propagates exceptions thrown by the action.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithThrowingAction_PropagatesException_Async()
    {
        // Arrange
        var result = Result.Success();
        var actionException = new InvalidOperationException("Action failed");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Task.FromResult(result).OnSuccessAsync(async () =>
            {
                await Task.Delay(1);
                throw actionException;
            }));
    }

    #endregion

    #region Async OnFailure Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync(Task{Result}, Func{Exception, Task})"/> executes the asynchronous action
    /// with the exception when the result is a failure.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithFailureResult_ExecutesAsyncActionWithException_Async()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        Exception receivedException = null;

        // Act
        Result returnedResult = await Task.FromResult(result)
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
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync(Task{Result}, Func{Exception, Task})"/> does not execute the asynchronous action
    /// when the result is successful.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithSuccessResult_DoesNotExecuteAsyncAction_Async()
    {
        // Arrange
        var result = Result.Success();
        bool actionExecuted = false;

        // Act
        Result returnedResult = await Task.FromResult(result)
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
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync{T}(Task{Result{T}}, Func{Exception, Task})"/> executes the asynchronous action
    /// with the exception when the result is a failure.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithFailureResultT_ExecutesAsyncActionWithException_Async()
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
    public async Task OnFailureAsync_WithSuccessResultT_DoesNotExecuteAsyncAction_Async()
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
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync(Task{Result}, Func{Exception, Task})"/> throws an <see cref="NullReferenceException"/>
    /// when the asynchronous action is null.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithNullAction_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result> result = Task.FromResult(Result.Failure(new Exception()));

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => result.OnFailureAsync(null!));
    }

    #endregion

    #region Async Map Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> transforms the value asynchronously
    /// when the result is successful.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithSuccessResult_TransformsValueAsync_Async()
    {
        // Arrange
        int inputValue = 5;
        var result = Result.Success(inputValue);

        // Act
        Result<int> mappedResult = await Task.FromResult(result)
            .MapAsync(async x =>
            {
                await Task.Delay(1);
                return x * 2;
            });

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(10, mappedResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> propagates the exception
    /// when the result is a failure.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithFailureResult_PropagatesException_Async()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);

        // Act
        Result<int> mappedResult = await Task.FromResult(result)
            .MapAsync(async x =>
            {
                await Task.Delay(1);
                return x * 2;
            });

        // Assert
        Assert.True(mappedResult.IsFailure);
        Assert.Same(exception, mappedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Result{TIn}, Func{TIn, Task{TOut}})"/> transforms the value asynchronously
    /// when called directly on a synchronous result.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithSyncResult_TransformsValueAsync_Async()
    {
        // Arrange
        int inputValue = 5;
        var result = Result.Success(inputValue);

        // Act
        Result<int> mappedResult = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(10, mappedResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> throws an <see cref="NullReferenceException"/>
    /// when the map function is null.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithNullFunc_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => result.MapAsync<int, string>(null!));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> handles exceptions thrown by the map function.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithThrowingMapFunc_ReturnsFailureResult_Async()
    {
        // Arrange
        var mapException = new InvalidOperationException("Map function failed");

        // Act
        Result<string> mappedResult = await Task.FromResult(Result.Failure<int>(mapException))
            .MapAsync<int, string>(async x =>
            {
                await Task.Delay(1);
                return mapException.Message;
            });

        // Assert
        Assert.True(mappedResult.IsFailure);
        Assert.Same(mapException, mappedResult.Exception);
    }

    #endregion

    #region Async Bind Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync(Task{Result}, Func{Task{Result}})"/> executes the asynchronous bind function
    /// and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSuccessResult_ExecutesAsyncBindFunction_Async()
    {
        // Arrange
        var result = Result.Success();
        var bindResult = Result.Success();
        bool bindExecuted = false;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async () =>
            {
                await Task.Delay(1);
                bindExecuted = true;
                return bindResult;
            });

        // Assert
        Assert.True(bindExecuted);
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync(Result, Func{Task{Result}})"/> executes the asynchronous bind function
    /// when called directly on a synchronous result.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResult_ExecutesAsyncBindFunction_Async()
    {
        // Arrange
        var result = Result.Success();
        var bindResult = Result.Success();
        bool bindExecuted = false;

        // Act
        Result returnedResult = await result.BindAsync(async () =>
        {
            await Task.Delay(1);
            bindExecuted = true;
            return bindResult;
        });

        // Assert
        Assert.True(bindExecuted);
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{Result{TOut}}})"/> executes the asynchronous bind function
    /// with the result value and returns its result when the initial result is successful.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSuccessResultT_ExecutesAsyncBindFunction_Async()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        var bindResult = Result.Success("transformed");
        int receivedValue = 0;

        // Act
        Result<string> returnedResult = await Task.FromResult(result)
            .BindAsync<int, string>(async value =>
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
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{Result{TOut}}})"/> propagates the exception
    /// when the initial result is a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithFailureResultT_PropagatesException_Async()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure<int>(exception);

        // Act
        Result<string> returnedResult = await Task.FromResult(result)
            .BindAsync<int, string>(async value =>
            {
                await Task.Delay(1);
                return Result.Success(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            });

        // Assert
        Assert.True(returnedResult.IsFailure);
        Assert.Same(exception, returnedResult.Exception);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync{TIn, TOut}(Result{TIn}, Func{TIn, Task{Result{TOut}}})"/> executes the asynchronous bind function
    /// when called directly on a synchronous result.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithSyncResultT_ExecutesAsyncBindFunction_Async()
    {
        // Arrange
        int inputValue = 42;
        var result = Result.Success(inputValue);
        var bindResult = Result.Success("transformed");

        // Act
        Result<string> returnedResult = await result.BindAsync<int, string>(async value =>
        {
            await Task.Delay(1);
            return bindResult;
        });

        // Assert
        Assert.Same(bindResult, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync(Task{Result}, Func{Task{Result}})"/> does not execute the bind function
    /// when the initial result is a failure.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithFailureResult_DoesNotExecuteBindFunction_Async()
    {
        // Arrange
        Exception exception = new InvalidOperationException("Test exception");
        var result = Result.Failure(exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = await Task.FromResult(result)
            .BindAsync(async () =>
            {
                await Task.Delay(1);
                bindExecuted = true;
                return Result.Success();
            });

        // Assert
        Assert.False(bindExecuted);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync(Task{Result}, Func{Task{Result}})"/> throws an <see cref="NullReferenceException"/>
    /// when the bind function is null.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithNullBindFunc_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result> result = Task.FromResult(Result.Success());

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => result.BindAsync(null!));
    }

    #endregion

    #region Implicit Operator Tests

    /// <summary>
    /// Verifies that a value is implicitly converted to a success Result.
    /// </summary>
    [Fact]
    public void ImplicitOperator_FromValue_CreatesSuccessResult()
    {
        // Arrange
        int testValue = 42;

        // Act
        Result<int> result = testValue;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testValue, result.Value);
    }

    /// <summary>
    /// Verifies that an exception is implicitly converted to a failure Result.
    /// </summary>
    [Fact]
    public void ImplicitOperator_FromException_CreatesFailureResult()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        Result result = exception;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(exception, result.Exception);
    }

    /// <summary>
    /// Verifies that an exception is implicitly converted to a failure Result{T}.
    /// </summary>
    [Fact]
    public void ImplicitOperator_FromException_CreatesFailureResultT()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        Result<int> result = exception;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(exception, result.Exception);
    }

    #endregion

    #region Custom Exception Type Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> handles custom exception types correctly.
    /// </summary>
    [Fact]
    public void OnFailure_WithCustomException_HandlesCorrectly()
    {
        // Arrange
        var customException = new CustomTestException();
        var result = Result.Failure(customException);
        Exception receivedException = null;

        // Act
        Result returnedResult = result.OnFailure(ex => receivedException = ex);

        // Assert
        Assert.IsType<CustomTestException>(receivedException);
        Assert.Same(customException, receivedException);
        Assert.Same(result, returnedResult);
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailure{T}(Result{T}, Action{Exception})"/> handles custom exception types correctly.
    /// </summary>
    [Fact]
    public void OnFailure_WithCustomExceptionT_HandlesCorrectly()
    {
        // Arrange
        var customException = new CustomTestException();
        var result = Result.Failure<int>(customException);
        Exception receivedException = null;

        // Act
        Result<int> returnedResult = result.OnFailure(ex => receivedException = ex);

        // Assert
        Assert.IsType<CustomTestException>(receivedException);
        Assert.Same(customException, receivedException);
        Assert.Same(result, returnedResult);
    }

    #endregion

    #region Null Task Validation Tests

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> throws an <see cref="NullReferenceException"/>
    /// when the task is null.
    /// </summary>
    [Fact]
    public async Task OnSuccessAsync_WithNullTask_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result> nullTask = null!;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            nullTask.OnSuccessAsync(() => Task.CompletedTask));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.OnFailureAsync(Task{Result}, Func{Exception, Task})"/> throws an <see cref="NullReferenceException"/>
    /// when the task is null.
    /// </summary>
    [Fact]
    public async Task OnFailureAsync_WithNullTask_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result> nullTask = null!;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            nullTask.OnFailureAsync(ex => Task.CompletedTask));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> throws an <see cref="NullReferenceException"/>
    /// when the task is null.
    /// </summary>
    [Fact]
    public async Task MapAsync_WithNullTask_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result<int>> nullTask = null!;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            nullTask.MapAsync<int, string>(x => Task.FromResult(x.ToString())));
    }

    /// <summary>
    /// Verifies that <see cref="ResultExtensions.BindAsync(Task{Result}, Func{Task{Result}})"/> throws an <see cref="NullReferenceException"/>
    /// when the task is null.
    /// </summary>
    [Fact]
    public async Task BindAsync_WithNullTask_ThrowsNullReferenceException_Async()
    {
        // Arrange
        Task<Result> nullTask = null!;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            nullTask.BindAsync(() => Task.FromResult(Result.Success())));
    }

    #endregion
}
