using System;
using System.Threading.Tasks;

namespace RA.Utilities.Core.Results.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> and <see cref="Result{T}"/> types
/// to enable a fluent, functional-style of programming for handling operation outcomes.
/// </summary>
public static class TypedResultExtensions
{
    extension<T>(Result<T> result)
    {
        /// <summary>
        /// Executes the specified action with the result value if the result is a success.
        /// This is useful for performing side effects on a successful result.
        /// </summary>
        /// <param name="action">The action to execute with the result value.</param>
        /// <returns>The original result, allowing for method chaining.</returns>
        public Result<T> OnSuccess(Action<T> action)
        {
            if (result.IsSuccess)
            {
                action(result.Value);
            }
            return result;
        }

        /// <summary>
        /// Executes the specified action if the result is a failure.
        /// This is useful for logging or handling errors as side effects.
        /// </summary>
        /// <param name="action">The action to execute with the exception.</param>
        /// <returns>The original result, allowing for method chaining.</returns>
        public Result<T> OnFailure(Action<Exception> action)
        {
            if (result.IsFailure)
            {
                action(result.Exception);
            }
            return result;
        }

        /// <summary>
        /// Transforms the value of a success result into a new value.
        /// If the result is a failure, the exception is propagated to the new result type.
        /// This is equivalent to a `Select` in LINQ.
        /// </summary>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <param name="mapFunc">The function to transform the value.</param>
        /// <returns>A new result with the transformed value or the original exception.</returns>
        public Result<TOut> Map<TOut>(Func<T, TOut> mapFunc) =>
            result.IsSuccess
                ? Result.Success(mapFunc(result.Value))
                : Result.Failure<TOut>(result.Exception);

        /// <summary>
        /// Chains an operation that returns a <see cref="Result"/>.
        /// If the initial result is a failure, the subsequent operation is not executed and the failure is propagated.
        /// This is useful for sequencing operations that can fail.
        /// </summary>
        /// <param name="bindFunc">The function to execute, which returns a <see cref="Result"/>.</param>
        /// <returns>The result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
        public Result Bind(Func<T, Result> bindFunc) =>
            result.IsSuccess
                ? bindFunc(result.Value)
                : Result.Failure(result.Exception);

        /// <summary>
        /// Chains an operation that returns a <see cref="Result{TOut}"/>.
        /// If the initial result is a failure, the subsequent operation is not executed and the failure is propagated.
        /// This is useful for sequencing operations that can fail.
        /// </summary>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <param name="bindFunc">The function to execute, which returns a <see cref="Result{TOut}"/>.</param>
        /// <returns>The result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, a failure result with the original exception.</returns>
        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bindFunc) =>
            result.IsSuccess
                ? bindFunc(result.Value)
                : Result.Failure<TOut>(result.Exception);

        /// <summary>
        /// Asynchronously transforms the value of a success result into a new value.
        /// </summary>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <param name="mapFunc">The asynchronous function to transform the value.</param>
        /// <returns>A task representing a new result with the transformed value or the original exception.</returns>
        public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> mapFunc)
        {
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
            return result.IsSuccess
                ? await bindFunc(result.Value).ConfigureAwait(false)
                : Result.Failure<TOut>(result.Exception);
        }
    }
}
