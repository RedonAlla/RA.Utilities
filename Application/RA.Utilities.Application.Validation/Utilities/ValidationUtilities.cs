using System.Collections.Generic;
using System.Linq;
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
    /// <returns>An array of <see cref="ValidationFailure"/> if any validation errors occur; otherwise, an empty array.</returns>
    public static async Task<ValidationFailure[]> ValidateAsync<TRequest>(TRequest request, IEnumerable<IValidator<TRequest>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

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
        ValidationErrors[] validationErrors = [.. validationFailures.Select(f => new ValidationErrors
        {
            PropertyName = f.PropertyName,
            ErrorMessage = f.ErrorMessage,
            AttemptedValue = f.AttemptedValue,
            ErrorCode = f.ErrorCode,
        })];

        return new BadRequestException(validationErrors);
    }
}
