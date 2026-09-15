using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Application.Validation.Utilities;

/// <summary>
/// Provides utility methods for validation using FluentValidation.
/// </summary>
public static class ValidationUtilities
{
    /// <summary>
    /// Validates a request using a collection of FluentValidation validators.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to validate.</typeparam>
    /// <param name="request">The request instance to validate.</param>
    /// <param name="validators">A collection of validators applicable to the request type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An array of <see cref="ValidationFailure"/> if any validation errors occur; otherwise, an empty array.</returns>
    public static async Task<ValidationFailure[]> ValidateAsync<TRequest>(
        TRequest request,
        IEnumerable<IValidator<TRequest>> validators,
        CancellationToken cancellationToken
    )
    {
        // DI already materializes the validator list as an array; the fallback keeps the method
        // working with any IEnumerable without allocating a fresh snapshot on every request.
        IValidator<TRequest>[] validatorArray =
            validators as IValidator<TRequest>[] ?? validators.ToArray();

        if (validatorArray.Length == 0)
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        // Fast path: a single validator skips the Task.WhenAll machinery entirely.
        if (validatorArray.Length == 1)
        {
            ValidationResult result =
                await validatorArray[0].ValidateAsync(context, cancellationToken).ConfigureAwait(false);

            return result.IsValid ? [] : [.. result.Errors];
        }

        var validationTasks = new Task<ValidationResult>[validatorArray.Length];
        for (int i = 0; i < validatorArray.Length; i++)
        {
            validationTasks[i] = validatorArray[i].ValidateAsync(context, cancellationToken);
        }

        ValidationResult[] validationResults =
            await Task.WhenAll(validationTasks).ConfigureAwait(false);

        ValidationFailure[] validationFailures = [.. validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)];

        return validationFailures;
    }

    /// <summary>
    /// Creates a <see cref="BadRequestException"/> from an array of validation failures.
    /// </summary>
    /// <param name="validationFailures">An array of <see cref="ValidationFailure"/>.</param>
    /// <returns>A new instance of <see cref="BadRequestException"/> containing the details of the validation errors.</returns>
    public static BadRequestException CreateValidationErrorResult(ValidationFailure[] validationFailures)
    {
        ValidationError[] validationErrors = [.. validationFailures.Select(f => new ValidationError(f.ErrorMessage)
        {
            PropertyName = f.PropertyName,
            AttemptedValue = f.AttemptedValue,
            ErrorCode = f.ErrorCode,
        })];

        return new BadRequestException(validationErrors);
    }
}
