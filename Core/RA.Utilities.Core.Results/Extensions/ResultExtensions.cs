using System;
using System.Threading.Tasks;

namespace RA.Utilities.Core.Results.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> and <see cref="Result{T}"/> types
/// to enable a fluent, functional-style of programming for handling operation outcomes.
/// </summary>
public static class ResultExtensions
{
    extension(Result result)
    {
        /// <summary>
        /// Executes the specified action if the result is a success.
        /// This is useful for performing side effects on a successful result.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The original result, allowing for method chaining.</returns>
        public Result OnSuccess(Action action)
        {
            if (result.IsSuccess)
            {
                action();
            }
            return result;
        }

        /// <summary>
        /// Executes the specified action if the result is a failure.
        /// This is useful for logging or handling errors as side effects.
        /// </summary>
        /// <param name="action">The action to execute with the exception.</param>
        /// <returns>The original result, allowing for method chaining.</returns>
        public Result OnFailure(Action<Exception> action)
        {
            if (result.IsFailure)
            {
                action(result.Exception);
            }
            return result;
        }

        /// <summary>
        /// Chains an operation that returns a <see cref="Result"/>.
        /// If the initial result is a failure, the subsequent operation is not executed and the failure is propagated.
        /// This is useful for sequencing operations that can fail.
        /// </summary>
        /// <param name="bindFunc">The function to execute, which returns a <see cref="Result"/>.</param>
        /// <returns>The result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
        public Result Bind(Func<Result> bindFunc) =>
            result.IsSuccess
                ? bindFunc()
                : result;

        /// <summary>
        /// Asynchronously chains an operation that returns a <see cref="Result"/>.
        /// </summary>
        /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
        /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
        public async Task<Result> BindAsync(Func<Task<Result>> bindFunc)
        {
            return result.IsSuccess
                ? await bindFunc().ConfigureAwait(false)
                : result;
        }

        /// <summary>
        /// Executes one of the provided functions based on whether the result is a success or a failure.
        /// </summary>
        /// <typeparam name="TContract">The return type of the match functions.</typeparam>
        /// <param name="success">The function to execute if the result is a success.</param>
        /// <param name="failure">The function to execute if the result is a failure. The function takes the exception as a parameter.</param>
        /// <returns>The value returned by the executed function.</returns>
        public TContract Match<TContract>(Func<TContract> success, Func<Exception, TContract> failure) =>
            result.IsSuccess ? success() : failure(result.Exception);

        /// <summary>
        /// Executes one of the provided actions based on whether the result is a success or a failure.
        /// </summary>
        /// <param name="success">The action to execute if the result is a success.</param>
        /// <param name="failure">The action to execute if the result is a failure. The function takes the exception as a parameter.</param>
        public void Match(Action success, Action<Exception> failure)
        {
            if (result.IsSuccess)
            {
                success();
            }
            else
            {
                failure(result.Exception);
            }
        }
    }

}
