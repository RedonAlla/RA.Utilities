using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.Mapper;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Api.EndpointFilters;

/// <summary>
/// Endpoint filter for validating a model or collection of models in Minimal APIs.
/// </summary>
/// <typeparam name="TModel">The type of model to validate.</typeparam>
public class ValidationEndpointFilter<TModel> : IEndpointFilter where TModel : class
{
    /// <summary>
    /// The collection of validators for the model type.
    /// </summary>
    private readonly IEnumerable<IValidator<TModel>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationEndpointFilter{TModel}"/> class.
    /// </summary>
    /// <param name="validators">The validators for the model type.</param>
    public ValidationEndpointFilter(IEnumerable<IValidator<TModel>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Invokes the filter to validate the model or collection of models before executing the endpoint.
    /// </summary>
    /// <param name="context">The endpoint filter invocation context.</param>
    /// <param name="next">The next delegate in the filter pipeline.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing either the result of the next delegate or a validation problem response.
    /// </returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        TModel? modelToValidate = context.Arguments.OfType<TModel>().FirstOrDefault(a => a is not null);

        if (modelToValidate is null)
        {
            return await next(context);
        }

        ValidationFailure[] validationFailures = await ValidateAsync(modelToValidate, _validators);

        if (validationFailures.Length == 0)
        {
            return await next(context);
        }

        return ErrorResultResponse.Result(CreateValidationErrorResult(validationFailures));
    }

    private static async Task<ValidationFailure[]> ValidateAsync(TModel request, IEnumerable<IValidator<TModel>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TModel>(request);

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
    private static BadRequestException CreateValidationErrorResult(ValidationFailure[] validationFailures)
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
