// CS1591.cs  
// compile with: /W:4 /doc:x.xml 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Tests.RA.Utilities.Core.Results;

public class ResultExtensionsTests
{
    private readonly TestLogger _logger = new();
    private readonly Exception _exception = new("error");

    #region OnSuccess

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnSuccess_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Result result = isSuccess ? Result.Success() : Result.Failure(_exception);
        bool wasCalled = false;

        // Act
        result.OnSuccess(() => wasCalled = true);

        // Assert
        wasCalled.Should().Be(isSuccess);
    }

    #endregion

    #region OnFailure

    [Fact]
    public void OnFailure_ShouldNotExecuteAction_WhenResultIsSuccess()
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

    [Fact]
    public void OnFailure_ShouldExecuteAction_WhenResultIsFailure()
    {
        // Arrange
        var failedResult = Result.Failure<int>(_exception);
        Exception? capturedException = null;

        // Act
        Result<int> result = failedResult.OnFailure(ex => capturedException = ex);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(_exception);
        capturedException.Should().BeSameAs(_exception);
        result.Should().BeSameAs(failedResult);
    }

    [Fact]
    public void OnFailure_ShouldThrowException_WhenActionThrowsException()
    {
        // Arrange
        var failedResult = Result.Failure<int>(_exception);
        var actionException = new NullReferenceException("Action failed");

        // Act
        Action act = () => failedResult.OnFailure(ex => throw actionException);

        // Assert
        act.Should().Throw<NullReferenceException>().Which.Should().BeSameAs(actionException);
    }

    [Fact]
    public void OnFailure_ShouldThrowNullReferenceException_WhenActionIsNull()
    {
        // Arrange
        var result = Result.Failure(_exception);

        // Act
        Action act = () => result.OnFailure(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void OnFailure_ShouldAllowChaining()
    {
        // Arrange
        var failedResult = Result.Failure<int>(_exception);
        int sideEffectValue = 0;

        // Act
        Result<int> result = failedResult.OnFailure(ex => sideEffectValue = 100);

        // Assert
        result.IsSuccess.Should().BeFalse();
        sideEffectValue.Should().Be(100);
        result.Should().BeSameAs(failedResult);
    }

    [Fact]
    public void OnFailure_ShouldNotExecuteAction_WhenResultIsSuccessWithNullValue()
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

    [Fact]
    public void OnFailure_ShouldExecuteAllActions_WhenMultipleCalls()
    {
        // Arrange
        var failedResult = Result.Failure<int>(_exception);
        int callCount = 0;

        // Act
        failedResult
            .OnFailure(ex => callCount++)
            .OnFailure(ex => callCount++);

        // Assert
        callCount.Should().Be(2);
    }

    [Fact]
    public void OnFailure_ShouldModifyExternalState_WhenActionIsExecuted()
    {
        // Arrange
        var failedResult = Result.Failure<int>(_exception);

        // Act
        failedResult.OnFailure(ex => _logger.LogError($"Operation failed: {ex.Message}"));

        // Assert
        _logger.Logs.Should().Contain("Operation failed: error");
    }

    #endregion

    #region Map

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_ShouldTransformOrPropagate(bool isSuccess)
    {
        // Arrange
        Result<int> result = isSuccess ? Result.Success(10) : Result.Failure<int>(_exception);

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
            mappedResult.Exception.Should().Be(_exception);
        }
    }

    #endregion

    #region Bind

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Bind_ShouldChainOrPropagate(bool isSuccess)
    {
        // Arrange
        Result<int> result = isSuccess ? Result.Success(5) : Result.Failure<int>(_exception);
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
            boundResult.Exception.Should().Be(_exception);
        }
    }

    [Fact]
    public void Bind_ShouldExecuteBindFunction_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result.Success(42);
        var bindResult = Result.Success();
        int receivedValue = 0;

        // Act
        Result returnedResult = result.Bind(value =>
        {
            receivedValue = value;
            return bindResult;
        });

        // Assert
        receivedValue.Should().Be(42);
        returnedResult.Should().BeSameAs(bindResult);
    }

    [Fact]
    public void Bind_ShouldPropagateException_WhenResultIsFailure()
    {
        // Arrange
        var result = Result.Failure<int>(_exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = result.Bind(value =>
        {
            bindExecuted = true;
            return Result.Success();
        });

        // Assert
        bindExecuted.Should().BeFalse();
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(_exception);
    }

    [Fact]
    public void Bind_ShouldReturnFailureResult_WhenBindFunctionFails()
    {
        // Arrange
        var result = Result.Success(42);
        var bindException = new ArgumentException("Bind failed");
        var failingBindResult = Result.Failure(bindException);

        // Act
        Result returnedResult = result.Bind(value => failingBindResult);

        // Assert
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(bindException);
    }

    [Fact]
    public void Bind_ShouldThrowNullReferenceException_WhenBindFunctionIsNull()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        Action act = () => result.Bind(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    #endregion

    #region Match

    [Fact]
    public void Match_ShouldExecuteCorrectFunction_BasedOnResultState()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(_exception);

        // Act
        string successMatch = successResult.Match(
            success: () => "success",
            failure: ex => "failure");

        string failureMatch = failureResult.Match(
            success: () => "success",
            failure: ex => ex.Message);

        // Assert
        successMatch.Should().Be("success");
        failureMatch.Should().Be("error");
    }

    [Fact]
    public void Match_Generic_ShouldExecuteCorrectFunction_BasedOnResultState()
    {
        // Arrange
        Result<int> successResult = 42;
        Result<int> failureResult = _exception;

        // Act
        string successMatch = successResult.Match(
            success: val => val.ToString(System.Globalization.CultureInfo.InvariantCulture),
            failure: ex => "failure");

        string failureMatch = failureResult.Match(
            success: val => val.ToString(System.Globalization.CultureInfo.InvariantCulture),
            failure: ex => ex.Message);

        // Assert
        successMatch.Should().Be("42");
        failureMatch.Should().Be("error");
    }

    [Fact]
    public void Match_Action_ShouldExecuteCorrectAction_BasedOnResultState()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(_exception);
        bool successExecuted = false;
        bool failureExecuted = false;

        // Act
        successResult.Match(
            success: () => successExecuted = true,
            failure: ex => failureExecuted = true);

        failureResult.Match(
            success: () => successExecuted = true,
            failure: ex => failureExecuted = true);

        // Assert
        successExecuted.Should().BeTrue();
        failureExecuted.Should().BeTrue();
    }

    [Fact]
    public void Match_Action_ShouldThrowNullReferenceException_WhenActionIsNull()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(_exception);

        // Act
        Action successAction = () => successResult.Match(null!, ex => { });
        Action failureAction = () => failureResult.Match(() => { }, null!);

        // Assert
        successAction.Should().Throw<NullReferenceException>();
        failureAction.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Match_ShouldThrowNullReferenceException_WhenFuncIsNull()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(_exception);

        // Act
        Action successAction = () => successResult.Match(null!, ex => "failure");
        Action failureAction = () => failureResult.Match(() => "success", null!);

        // Assert
        successAction.Should().Throw<NullReferenceException>();
        failureAction.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Match_ShouldOnlyExecuteOneBranch()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(_exception);
        bool successFuncCalled = false;
        bool failureFuncCalled = false;

        // Act
        successResult.Match(
            () =>
            {
                successFuncCalled = true;
                return true;
            },
            ex =>
            {
                failureFuncCalled = true;
                return false;
            });

        // Assert
        successFuncCalled.Should().BeTrue();
        failureFuncCalled.Should().BeFalse();

        // Act 2
        failureResult.Match(() => false, ex => failureFuncCalled = true);

        // Assert 2
        failureFuncCalled.Should().BeTrue();
    }

    #endregion

    #region OnSuccessAsync

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnSuccessAsync_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Task<Result> resultTask = Task.FromResult(isSuccess ? Result.Success() : Result.Failure(_exception));
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnSuccessAsync_Generic_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Task<Result<string>> resultTask = Task.FromResult(isSuccess
            ? Result.Success("data")
            : Result.Failure<string>(_exception));

        string? receivedValue = null;

        // Act
        await resultTask.OnSuccessAsync(value =>
        {
            receivedValue = value;
            return Task.CompletedTask;
        });

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

    #endregion

    #region OnFailureAsync

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnFailureAsync_ShouldExecuteAction_BasedOnResultState(bool isSuccess)
    {
        // Arrange
        Task<Result> resultTask = Task.FromResult(isSuccess ? Result.Success() : Result.Failure(_exception));
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
            receivedException.Should().Be(_exception);
        }
    }

    [Fact]
    public async Task OnFailureAsync_ShouldNotExecuteAction_WhenResultIsSuccess()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));
        bool actionExecuted = false;

        // Act
        Result<int> returnedResult = await result.OnFailureAsync(async ex =>
        {
            await Task.Delay(1);
            actionExecuted = true;
        });

        // Assert
        actionExecuted.Should().BeFalse();
        returnedResult.Should().Be(await result);
    }

    [Fact]
    public async Task OnFailureAsync_ShouldExecuteAction_WhenResultIsFailure()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Failure<int>(_exception));
        Exception? receivedException = null;

        // Act
        Result<int> returnedResult = await result.OnFailureAsync(async ex =>
        {
            await Task.Delay(1);
            receivedException = ex;
        });

        // Assert
        receivedException.Should().BeSameAs(_exception);
        returnedResult.Should().Be(await result);
    }

    [Fact]
    public async Task OnFailureAsync_ShouldThrowException_WhenActionThrowsException()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Failure<int>(_exception));
        var actionException = new ArgumentException("Action exception");

        // Act
        Func<Task> act = () => result.OnFailureAsync(async ex =>
        {
            await Task.Delay(1);
            throw actionException;
        });

        // Assert
        (await act.Should().ThrowAsync<ArgumentException>()).Which.Should().BeSameAs(actionException);
    }

    #endregion

    #region MapAsync

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MapAsync_ShouldTransformOrPropagate(bool isSuccess)
    {
        // Arrange
        Task<Result<int>> resultTask = Task.FromResult(isSuccess ? Result.Success(10) : Result.Failure<int>(_exception));

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
            mappedResult.Exception.Should().Be(_exception);
        }
    }

    [Fact]
    public async Task MapAsync_ShouldReturnMappedValue_WhenResultIsSuccess()
    {
        // Arrange
        Task<Result<int>> successResultTask = Task.FromResult(Result.Success(42));

        // Act
        Result<string> result = await successResultTask.MapAsync(async x =>
            await Task.FromResult(x.ToString(System.Globalization.CultureInfo.InvariantCulture)));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("42");
    }

    [Fact]
    public async Task MapAsync_ShouldReturnFailure_WhenResultIsFailure()
    {
        // Arrange
        Task<Result<int>> failedResultTask = Task.FromResult(Result.Failure<int>(_exception));
        bool mapFuncCalled = false;

        // Act
        Result<string> result = await failedResultTask.MapAsync<int, string>(async x =>
        {
            mapFuncCalled = true;
            return await Task.FromResult(x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(_exception);
        mapFuncCalled.Should().BeFalse();
    }

    #endregion

    #region BindAsync

    [Fact]
    public async Task BindAsync_ShouldReturnBoundResult_WhenResultIsSuccess()
    {
        // Arrange
        var successResult = Result.Success(42);

        // Act
        Result<string> result = await successResult.BindAsync(async x =>
        {
            string transformed = await Task.FromResult($"Success: {x}");
            return Result.Success(transformed);
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Success: 42");
    }

    [Fact]
    public async Task BindAsync_ShouldReturnFailure_WithoutCallingBindFunc_WhenResultIsFailure()
    {
        // Arrange
        var failedResult = Result.Failure<int>(_exception);
        bool bindFuncCalled = false;

        // Act
        Result<string> result = await failedResult.BindAsync<int, string>(async x =>
        {
            bindFuncCalled = true;
            return await Task.FromResult(Result.Success(x.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(_exception);
        bindFuncCalled.Should().BeFalse();
    }

    [Fact]
    public async Task BindAsync_ShouldReturnFailedResult_WhenBindFuncReturnsFailure()
    {
        // Arrange
        var successResult = Result.Success(42);
        var bindException = new ArgumentException("Binding failed");

        // Act
        Result<string> result = await successResult.BindAsync<int, string>(async x =>
        {
            await Task.CompletedTask;
            return Result.Failure<string>(bindException);
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeSameAs(bindException);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateException_WhenBindFuncThrowsException()
    {
        // Arrange
        var successResult = Result.Success(42);
        var expectedException = new InvalidOperationException("Bind func crashed");

        // Act
        Func<Task> act = () => successResult.BindAsync<int, string>(async x =>
        {
            await Task.CompletedTask;
            throw expectedException;
        });

        // Assert
        (await act.Should().ThrowAsync<InvalidOperationException>()).Which.Should().BeSameAs(expectedException);
    }

    [Fact]
    public async Task BindAsync_ShouldThrowANullReferenceException_WhenBindFuncIsNull()
    {
        // Arrange
        var successResult = Result.Success(42);

        // Act
        Func<Task> act = () => successResult.BindAsync<int, string>(null!);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task BindAsync_ShouldCompleteSuccessfully_WithAsyncWorkInBindFunc()
    {
        // Arrange
        var successResult = Result.Success(5);
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

    [Fact]
    public async Task BindAsync_ShouldChainMultipleBindOperations()
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
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_ShouldExecuteAsyncBindFunction_WithSyncResult()
    {
        // Arrange
        var result = Result.Success(42);
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
        receivedValue.Should().Be(42);
        returnedResult.Should().BeSameAs(bindResult);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateException_WithSyncResultAndFailure()
    {
        // Arrange
        var result = Result.Failure<int>(_exception);
        bool bindExecuted = false;

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            bindExecuted = true;
            return Result.Success();
        });

        // Assert
        bindExecuted.Should().BeFalse();
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(_exception);
    }

    [Fact]
    public async Task BindAsync_ShouldReturnFailureResult_WithSyncResultAndFailingBindFunction()
    {
        // Arrange
        var result = Result.Success(42);
        var bindException = new ArgumentException("Bind failed");
        var failingBindResult = Result.Failure(bindException);

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            return failingBindResult;
        });

        // Assert
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(bindException);
    }

    [Fact]
    public async Task BindAsync_ShouldThrowNullReferenceException_WithSyncResultAndNullBindFunc()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        Func<Task> act = () => result.BindAsync<int>(null!);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task BindAsync_ShouldReturnFailureResult_WithSyncResultAndBindFunctionThrowingException()
    {
        // Arrange
        var result = Result.Success(42);
        var bindException = new InvalidOperationException("Bind function exception");

        // Act
        Result returnedResult = await result.BindAsync<int>(async value =>
        {
            await Task.Delay(1);
            return bindException;
        });

        // Assert
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(bindException);
    }

    [Fact]
    public async Task BindAsync_ShouldExecuteAsyncBindFunction_FromTaskResult()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));
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
        receivedValue.Should().Be(42);
        returnedResult.Should().BeSameAs(bindResult);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateException_FromTaskResultAndFailure()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Failure<int>(_exception));
        bool bindExecuted = false;

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            bindExecuted = true;
            return Result.Success();
        });

        // Assert
        bindExecuted.Should().BeFalse();
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(_exception);
    }

    [Fact]
    public async Task BindAsync_ShouldReturnFailureResult_FromTaskResultAndFailingBindFunction()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));
        var bindException = new ArgumentException("Bind failed");
        var failingBindResult = Result.Failure(bindException);

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            return failingBindResult;
        });

        // Assert
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(bindException);
    }

    [Fact]
    public async Task BindAsync_ShouldThrowNullReferenceException_FromTaskResultAndNullBindFunc()
    {
        // Arrange
        Task<Result<int>> resultTask = Task.FromResult(Result.Success(42));

        // Act
        Func<Task> act = () => resultTask.BindAsync<int>(null!);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task BindAsync_ShouldReturnFailureResult_FromTaskResultAndBindFunctionThrowingException()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));
        var bindException = new InvalidOperationException("Bind function exception");

        // Act
        Result returnedResult = await result.BindAsync<int>(async value =>
        {
            await Task.Delay(1);
            return bindException;
        });

        // Assert
        returnedResult.IsFailure.Should().BeTrue();
        returnedResult.Exception.Should().BeSameAs(bindException);
    }

    [Fact]
    public async Task BindAsync_ShouldAllowMethodChaining_FromTaskResult()
    {
        // Arrange
        Task<Result<int>> result = Task.FromResult(Result.Success(42));
        bool secondOperationExecuted = false;

        // Act
        Result finalResult = await result
            .BindAsync(async value =>
            {
                await Task.Delay(1);
                return Result.Success();
            })
            .OnSuccessAsync(async () =>
            {
                await Task.Delay(1);
                secondOperationExecuted = true;
            });

        // Assert
        secondOperationExecuted.Should().BeTrue();
        finalResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_ShouldExecuteBindFunction_FromTaskResultAndNullValue()
    {
        // Arrange
        Task<Result<string>> result = Task.FromResult(Result.Success<string>(null!));
        string? receivedValue = "not null";

        // Act
        Result returnedResult = await result.BindAsync(async value =>
        {
            await Task.Delay(1);
            receivedValue = value;
            return Result.Success();
        });

        // Assert
        receivedValue.Should().BeNull();
        returnedResult.IsSuccess.Should().BeTrue();
    }

    #endregion
}

internal sealed class TestLogger
{
    public List<string> Logs { get; } = new();

    public void LogError(string message)
    {
        Logs.Add(message);
    }
}
