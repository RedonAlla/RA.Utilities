using System;
using System.Threading.Tasks;

namespace RA.Utilities.Core.Results.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> and <see cref="Result{T}"/> types
/// to enable a fluent, functional-style of programming for handling operation outcomes.
/// </summary>
public static class TypedResultAsyncExtensions
{
    extension<T>(Task<Result<T>> resultTask)
    {
        /// <summary>
        /// Executes an asynchronous action if the result is a success.
        /// </summary>
        /// <param name="action">The asynchronous action to execute.</param>
        /// <returns>A task representing the original result.</returns>
        public async Task<Result<T>> OnSuccessAsync(Func<T, Task> action)
        {
            Result<T> result = await resultTask.ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await action(result.Value).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Executes an asynchronous action if the result is a failure.
        /// </summary>
        /// <param name="action">The asynchronous action to execute with the exception.</param>
        /// <returns>A task representing the original result.</returns>
        public async Task<Result<T>> OnFailureAsync(Func<Exception, Task> action)
        {
            Result<T> result = await resultTask.ConfigureAwait(false);
            if (result.IsFailure)
            {
                await action(result.Exception).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Asynchronously transforms the value of a success result into a new value.
        /// </summary>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <param name="mapFunc">The asynchronous function to transform the value.</param>
        /// <returns>A task representing a new result with the transformed value or the original exception.</returns>
        public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> mapFunc)
        {
            Result<T> result = await resultTask.ConfigureAwait(false);
            return result.IsSuccess
                ? Result.Success(await mapFunc(result.Value).ConfigureAwait(false))
                : Result.Failure<TOut>(result.Exception);
        }

        /// <summary>
        /// Asynchronously chains an operation that returns a <see cref="Result"/>.
        /// </summary>
        /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
        /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
        public async Task<Result> BindAsync(Func<T, Task<Result>> bindFunc)
        {
            Result<T> result = await resultTask.ConfigureAwait(false);
            return result.IsSuccess
                ? await bindFunc(result.Value).ConfigureAwait(false)
                : Result.Failure(result.Exception);
        }

        /// <summary>
        /// Asynchronously chains an operation that returns a <see cref="Result{TOut}"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result{TOut}"/>.</param>
        /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
        public async Task<Result<TOut>> BindAsync<TOut>(Func<T, Task<Result<TOut>>> bindFunc)
        {
            Result<T> result = await resultTask.ConfigureAwait(false);
            return result.IsSuccess
                ? await bindFunc(result.Value).ConfigureAwait(false)
                : Result.Failure<TOut>(result.Exception);
        }
    }
}
