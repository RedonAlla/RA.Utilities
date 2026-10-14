using System;
using System.Threading.Tasks;

namespace RA.Utilities.Core.Results.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> and <see cref="Result{T}"/> types
/// to enable a fluent, functional-style of programming for handling operation outcomes.
/// </summary>
public static class ResultAsyncExtensions
{
    extension(Task<Result> resultTask)
    {
        /// <summary>
        /// Executes an asynchronous action if the result is a success.
        /// </summary>
        /// <param name="action">The asynchronous action to execute.</param>
        /// <returns>A task representing the original result.</returns>
        public async Task<Result> OnSuccessAsync(Func<Task> action)
        {
            Result result = await resultTask.ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await action().ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Executes an asynchronous action if the result is a failure.
        /// </summary>
        /// <param name="action">The asynchronous action to execute with the exception.</param>
        /// <returns>A task representing the original result.</returns>
        public async Task<Result> OnFailureAsync(Func<Exception, Task> action)
        {
            Result result = await resultTask.ConfigureAwait(false);
            if (result.IsFailure)
            {
                await action(result.Exception).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Asynchronously chains an operation that returns a <see cref="Result"/>.
        /// </summary>
        /// <param name="bindFunc">The asynchronous function to execute, which returns a <see cref="Result"/>.</param>
        /// <returns>A task representing the result of the <paramref name="bindFunc"/> if the initial result was a success; otherwise, the original failure result.</returns>
        public async Task<Result> BindAsync(Func<Task<Result>> bindFunc)
        {
            Result result = await resultTask.ConfigureAwait(false);
            return result.IsSuccess
                ? await bindFunc().ConfigureAwait(false)
                : result;
        }
    }

}
