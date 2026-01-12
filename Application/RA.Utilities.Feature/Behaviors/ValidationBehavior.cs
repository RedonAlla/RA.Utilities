using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Utilities;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Behaviors;

/// <summary>
/// Represents a validation behavior for handling requests with a response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">The validators.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidationUtilities.ValidateAsync(request, _validators);

        if (validationFailures.Length == 0)
        {
            return await next();
        }

        return ValidationUtilities.CreateValidationErrorResult(validationFailures);
    }
}

/// <summary>
/// Represents a validation behavior for handling requests without a response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="validators">The validators.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    /// <inheritdoc/>
    public async Task<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidationUtilities.ValidateAsync(request, _validators);

        if (validationFailures.Length == 0)
        {
            return await next();
        }

        return ValidationUtilities.CreateValidationErrorResult(validationFailures);
    }
}
