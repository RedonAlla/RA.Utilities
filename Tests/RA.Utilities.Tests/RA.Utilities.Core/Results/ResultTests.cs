using System;
using System.Threading.Tasks;
using Xunit;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Core.Tests.Results
{
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
            Result result = Result.Success();
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
            Result result = Result.Failure(exception);
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
            Result<int> result = Result.Success(testValue);
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
            Result<int> result = Result.Failure<int>(exception);
            bool actionExecuted = false;

            // Act
            Result<int> returnedResult = result.OnSuccess(value => actionExecuted = true);

            // Assert
            Assert.False(actionExecuted);
            Assert.Same(result, returnedResult);
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
            Result result = Result.Failure(exception);
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
            Result result = Result.Success();
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
            Result<int> result = Result.Failure<int>(exception);
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
            Result<int> result = Result.Success(42);
            bool actionExecuted = false;

            // Act
            Result<int> returnedResult = result.OnFailure(ex => actionExecuted = true);

            // Assert
            Assert.False(actionExecuted);
            Assert.Same(result, returnedResult);
        }

        #endregion

        #region Map Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> transforms the value
        /// when the result is successful.
        /// </summary>
        [Fact]
        public void Map_WithSuccessResult_TransformsValue()
        {
            // Arrange
            int inputValue = 5;
            Result<int> result = Result.Success(inputValue);

            // Act
            Result<int> mappedResult = result.Map(x => x * 2);

            // Assert
            Assert.True(mappedResult.IsSuccess);
            Assert.Equal(10, mappedResult.Value);
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
            Result<int> result = Result.Failure<int>(exception);

            // Act
            Result<int> mappedResult = result.Map(x => x * 2);

            // Assert
            Assert.True(mappedResult.IsFailure);
            Assert.Same(exception, mappedResult.Exception);
        }

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> throws an <see cref="ArgumentNullException"/>
        /// when the map function is null.
        /// </summary>
        [Fact]
        public void Map_WithNullMapFunc_ThrowsArgumentNullException()
        {
            // Arrange
            Result<int> result = Result.Success(42);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.Map<int, string>(null!));
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
            Result result = Result.Success();
            Result bindResult = Result.Success();
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
            Result result = Result.Failure(exception);
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
        public void Bind_WithSuccessResultT_ExecutesBindFunction()
        {
            // Arrange
            int inputValue = 42;
            Result<int> result = Result.Success(inputValue);
            Result bindResult = Result.Success();
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
        /// Verifies that <see cref="ResultExtensions.Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> returns a new result
        /// with the transformed value when the initial result is successful.
        /// </summary>
        [Fact]
        public void Bind_WithSuccessResultT_ReturnsNewResult()
        {
            // Arrange
            int inputValue = 5;
            Result<int> result = Result.Success(inputValue);

            // Act
            Result<string> returnedResult = result.Bind<int, string>(value => Result.Success(value.ToString()));

            // Assert
            Assert.True(returnedResult.IsSuccess);
            Assert.Equal("5", returnedResult.Value);
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
            Result<int> result = Result.Failure<int>(exception);

            // Act
            Result<string> returnedResult = result.Bind<int, string>(value => Result.Success(value.ToString()));

            // Assert
            Assert.True(returnedResult.IsFailure);
            Assert.Same(exception, returnedResult.Exception);
        }

        #endregion

        #region Match Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.Match{TContract}(Result, Func{TContract}, Func{Exception, TContract})"/>
        /// executes the success function and returns its result when the result is successful.
        /// </summary>
        [Fact]
        public void Match_WithSuccessResult_ExecutesSuccessFunction()
        {
            // Arrange
            Result result = Result.Success();
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
            Result result = Result.Failure(exception);
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
        /// Verifies that <see cref="ResultExtensions.Match(Result, Action, Action{Exception})"/> executes the correct action
        /// based on whether the result is successful or a failure.
        /// </summary>
        [Fact]
        public void Match_WithActions_ExecutesCorrectAction()
        {
            // Arrange
            Exception exception = new InvalidOperationException("Test exception");
            Result result = Result.Failure(exception);
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

        #endregion

        #region Async OnSuccess Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> executes the asynchronous action
        /// when the result is successful.
        /// </summary>
        [Fact]
        public async Task OnSuccessAsync_WithSuccessResult_ExecutesAsyncAction()
        {
            // Arrange
            Result result = Result.Success();
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
        public async Task OnSuccessAsync_WithFailureResult_DoesNotExecuteAsyncAction()
        {
            // Arrange
            Exception exception = new InvalidOperationException("Test exception");
            Result result = Result.Failure(exception);
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
        public async Task OnSuccessAsync_WithSuccessResultT_ExecutesAsyncActionWithValue()
        {
            // Arrange
            int testValue = 42;
            Result<int> result = Result.Success(testValue);
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

        #endregion

        #region Async OnFailure Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.OnFailureAsync(Task{Result}, Func{Exception, Task})"/> executes the asynchronous action
        /// with the exception when the result is a failure.
        /// </summary>
        [Fact]
        public async Task OnFailureAsync_WithFailureResult_ExecutesAsyncActionWithException()
        {
            // Arrange
            Exception exception = new InvalidOperationException("Test exception");
            Result result = Result.Failure(exception);
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
        public async Task OnFailureAsync_WithSuccessResult_DoesNotExecuteAsyncAction()
        {
            // Arrange
            Result result = Result.Success();
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

        #endregion

        #region Async Map Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> transforms the value asynchronously
        /// when the result is successful.
        /// </summary>
        [Fact]
        public async Task MapAsync_WithSuccessResult_TransformsValueAsync()
        {
            // Arrange
            int inputValue = 5;
            Result<int> result = Result.Success(inputValue);

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
        public async Task MapAsync_WithFailureResult_PropagatesException()
        {
            // Arrange
            Exception exception = new InvalidOperationException("Test exception");
            Result<int> result = Result.Failure<int>(exception);

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
        public async Task MapAsync_WithSyncResult_TransformsValueAsync()
        {
            // Arrange
            int inputValue = 5;
            Result<int> result = Result.Success(inputValue);

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

        #endregion

        #region Async Bind Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.BindAsync(Task{Result}, Func{Task{Result}})"/> executes the asynchronous bind function
        /// and returns its result when the initial result is successful.
        /// </summary>
        [Fact]
        public async Task BindAsync_WithSuccessResult_ExecutesAsyncBindFunction()
        {
            // Arrange
            Result result = Result.Success();
            Result bindResult = Result.Success();
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
        public async Task BindAsync_WithSyncResult_ExecutesAsyncBindFunction()
        {
            // Arrange
            Result result = Result.Success();
            Result bindResult = Result.Success();
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
        public async Task BindAsync_WithSuccessResultT_ExecutesAsyncBindFunction()
        {
            // Arrange
            int inputValue = 42;
            Result<int> result = Result.Success(inputValue);
            Result<string> bindResult = Result.Success("transformed");
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
        public async Task BindAsync_WithFailureResultT_PropagatesException()
        {
            // Arrange
            Exception exception = new InvalidOperationException("Test exception");
            Result<int> result = Result.Failure<int>(exception);

            // Act
            Result<string> returnedResult = await Task.FromResult(result)
                .BindAsync<int, string>(async value =>
                {
                    await Task.Delay(1);
                    return Result.Success(value.ToString());
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
        public async Task BindAsync_WithSyncResultT_ExecutesAsyncBindFunction()
        {
            // Arrange
            int inputValue = 42;
            Result<int> result = Result.Success(inputValue);
            Result<string> bindResult = Result.Success("transformed");

            // Act
            Result<string> returnedResult = await result.BindAsync<int, string>(async value =>
            {
                await Task.Delay(1);
                return bindResult;
            });

            // Assert
            Assert.Same(bindResult, returnedResult);
        }

        #endregion

        #region Edge Cases and Validation Tests

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.OnSuccess(Result, Action)"/> throws an <see cref="ArgumentNullException"/>
        /// when the action is null.
        /// </summary>
        [Fact]
        public void OnSuccess_WithNullAction_ThrowsArgumentNullException()
        {
            // Arrange
            Result result = Result.Success();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.OnSuccess(null!));
        }

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.OnFailure(Result, Action{Exception})"/> throws an <see cref="ArgumentNullException"/>
        /// when the action is null.
        /// </summary>
        [Fact]
        public void OnFailure_WithNullAction_ThrowsArgumentNullException()
        {
            // Arrange
            Result result = Result.Success();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.OnFailure(null!));
        }

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.OnSuccessAsync(Task{Result}, Func{Task})"/> throws an <see cref="ArgumentNullException"/>
        /// when the asynchronous action is null.
        /// </summary>
        [Fact]
        public async Task OnSuccessAsync_WithNullAction_ThrowsArgumentNullException()
        {
            // Arrange
            Task<Result> result = Task.FromResult(Result.Success());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => result.OnSuccessAsync(null!));
        }

        /// <summary>
        /// Verifies that <see cref="ResultExtensions.MapAsync{TIn, TOut}(Task{Result{TIn}}, Func{TIn, Task{TOut}})"/> throws an <see cref="ArgumentNullException"/>
        /// when the map function is null.
        /// </summary>
        [Fact]
        public async Task MapAsync_WithNullFunc_ThrowsArgumentNullException()
        {
            // Arrange
            Task<Result<int>> result = Task.FromResult(Result.Success(42));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => result.MapAsync<int, string>(null!));
        }

        #endregion
    }
}