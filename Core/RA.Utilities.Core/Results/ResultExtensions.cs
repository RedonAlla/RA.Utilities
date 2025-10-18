using System;
using System.Threading.Tasks;

namespace RA.Utilities.Core.Results;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> and <see cref="Result{T}"/> types
/// to enable a fluent, functional-style of programming for handling operation outcomes.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes the specified action if the result is a success.
    /// This is useful for performing side effects on a successful result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result, allowing for method chaining.</returns>
    public static Result OnSuccess(this Result result, Action action)
    {
        if (result.IsSuccess)
        {
            action();
        }
        return result;
    }

    /// <summary>
    /// Executes the specified action with the result value if the result is a success.
    /// This is useful for performing side effects on a successful result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute with the result value.</param>
    /// <returns>The original result, allowing for method chaining.</returns>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value!);
        }
        return result;
    }

    /// <summary>
    /// Executes the specified action if the result is a failure.
    /// This is useful for logging or handling errors as side effects.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute with the exception.</param>
    /// <returns>The original result, allowing for method chaining.</returns>
    public static Result OnFailure(this Result result, Action<Exception> action)
    {
        if (result.IsFailure)
        {
            action(result.Exception!);
        }
        return result;
    }

    /// <summary>
    /// Executes the specified action if the result is a failure.
    /// This is useful for logging or handling errors as side effects.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute with the exception.</param>
    /// <returns>The original result, allowing for method chaining.</returns>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<Exception> action)
    {
        if (result.IsFailure)
        {
            action(result.Exception!);
        }
        return result;
    }

    /// <summary>
    /// Transforms the value of a success result into a new value.
    /// If the result is a failure, the exception is propagated to the new result type.
    /// This is equivalent to a `Select` in LINQ.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="mapFunc">The function to transform the value.</param>
    /// <returns>A new result with the transformed value or the original exception.</returns>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapFunc) =>
        result.IsSuccess
            ? Result.Success(mapFunc(result.Value!))
            : Result.Failure<TOut>(result.Exception!);

    /// <summary>
    /// Chains an operation that returns a <see cref="Result"/>.
    /// If the initial result is a failure, the subsequent operation is not executed and the failure is propagated.
    /// This is useful for sequencing operations that can fail.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="bindFunc">The function to execute, which returns a <see cref="Result"/>.</param>
    /// <returns>The result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
    public static Result Bind(this Result result, Func<Result> bindFunc) =>
        result.IsSuccess
            ? bindFunc()
            : result;

    /// <summary>
    /// Chains an operation that returns a <see cref="Result"/>.
    /// If the initial result is a failure, the subsequent operation is not executed and the failure is propagated.
    /// This is useful for sequencing operations that can fail.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="bindFunc">The function to execute, which returns a <see cref="Result"/>.</param>
    /// <returns>The result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
    public static Result Bind<TIn>(this Result<TIn> result, Func<TIn, Result> bindFunc) =>
        result.IsSuccess
            ? bindFunc(result.Value!)
            : Result.Failure(result.Exception!);

    /// <summary>
    /// Chains an operation that returns a <see cref="Result{TOut}"/>.
    /// If the initial result is a failure, the subsequent operation is not executed and the failure is propagated.
    /// This is useful for sequencing operations that can fail.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="bindFunc">The function to execute, which returns a <see cref="Result{TOut}"/>.</param>
    /// <returns>The result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bindFunc) =>
        result.IsSuccess
            ? bindFunc(result.Value!)
            : Result.Failure<TOut>(result.Exception!);

    /// <summary>
    /// Executes one of the provided functions based on whether the result is a success or a failure.
    /// </summary>
    /// <typeparam name="TContract">The return type of the match functions.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="success">The function to execute if the result is a success.</param>
    /// <param name="failure">The function to execute if the result is a failure. The function takes the exception as a parameter.</param>
    /// <returns>The value returned by the executed function.</returns>
    public static TContract Match<TContract>(this Result result, Func<TContract> success, Func<Exception, TContract> failure) =>
        result.IsSuccess ? success() : failure(result.Exception!);

    /// <summary>
    /// Executes one of the provided actions based on whether the result is a success or a failure.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="success">The action to execute if the result is a success.</param>
    /// <param name="failure">The action to execute if the result is a failure. The function takes the exception as a parameter.</param>
    public static void Match(this Result result, Action success, Action<Exception> failure)
    {
        if (result.IsSuccess)
        {
            success();
        }
        else
        {
            failure(result.Exception!);
        }
    }

    // --- Async Extensions ---

    /// <summary>
    /// Executes an asynchronous action if the result is a success.
    /// </summary>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result> OnSuccessAsync(this Task<Result> resultTask, Func<Task> action)
    {
        Result result = await resultTask.ConfigureAwait(false);
        if (result.IsSuccess)
        {
            await action().ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>
    /// Executes an asynchronous action if the result is a success.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result<T>> OnSuccessAsync<T>(this Task<Result<T>> resultTask, Func<T, Task> action)
    {
        Result<T> result = await resultTask.ConfigureAwait(false);
        if (result.IsSuccess)
        {
            await action(result.Value!).ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>
    /// Executes an asynchronous action if the result is a failure.
    /// </summary>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="action">The asynchronous action to execute with the exception.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result> OnFailureAsync(this Task<Result> resultTask, Func<Exception, Task> action)
    {
        Result result = await resultTask.ConfigureAwait(false);
        if (result.IsFailure)
        {
            await action(result.Exception!).ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>
    /// Executes an asynchronous action if the result is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="action">The asynchronous action to execute with the exception.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result<T>> OnFailureAsync<T>(this Task<Result<T>> resultTask, Func<Exception, Task> action)
    {
        Result<T> result = await resultTask.ConfigureAwait(false);
        if (result.IsFailure)
        {
            await action(result.Exception!).ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>
    /// Asynchronously transforms the value of a success result into a new value.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="mapFunc">The asynchronous function to transform the value.</param>
    /// <returns>A task representing a new result with the transformed value or the original exception.</returns>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> mapFunc)
    {
        Result<TIn> result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? Result.Success(await mapFunc(result.Value!).ConfigureAwait(false))
            : Result.Failure<TOut>(result.Exception!);
    }

    /// <summary>
    /// Asynchronously transforms the value of a success result into a new value.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="mapFunc">The asynchronous function to transform the value.</param>
    /// <returns>A task representing a new result with the transformed value or the original exception.</returns>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<TOut>> mapFunc)
    {
        return result.IsSuccess
            ? Result.Success(await mapFunc(result.Value!).ConfigureAwait(false))
            : Result.Failure<TOut>(result.Exception!);
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result"/>.
    /// </summary>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
    /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
    public static async Task<Result> BindAsync(this Task<Result> resultTask, Func<Task<Result>> bindFunc)
    {
        Result result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await bindFunc().ConfigureAwait(false)
            : result;
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
    /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
    public static async Task<Result> BindAsync(this Result result, Func<Task<Result>> bindFunc)
    {
        return result.IsSuccess
            ? await bindFunc().ConfigureAwait(false)
            : result;
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
    /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
    public static async Task<Result> BindAsync<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> bindFunc)
    {
        Result<TIn> result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await bindFunc(result.Value!).ConfigureAwait(false)
            : Result.Failure(result.Exception!);
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
    /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
    public static async Task<Result> BindAsync<TIn>(this Result<TIn> result, Func<TIn, Task<Result>> bindFunc)
    {
        return result.IsSuccess
            ? await bindFunc(result.Value!).ConfigureAwait(false)
            : Result.Failure(result.Exception!);
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result{TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result{TOut}"/>.</param>
    /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> bindFunc)
    {
        Result<TIn> result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await bindFunc(result.Value!).ConfigureAwait(false)
            : Result.Failure<TOut>(result.Exception!);
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result{TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result{TOut}"/>.</param>
    /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> bindFunc)
    {
        return result.IsSuccess
            ? await bindFunc(result.Value!).ConfigureAwait(false)
            : Result.Failure<TOut>(result.Exception!);
    }
}
