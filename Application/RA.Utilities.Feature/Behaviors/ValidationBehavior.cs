using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Utilities;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Behaviors;

/// <summary>
/// Represents a validation behavior for handling requests with a response.
/// When validation fails, a <see cref="RA.Utilities.Core.Exceptions.BadRequestException"/> is thrown.
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
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators ?? [];

    /// <inheritdoc/>
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => HandleAsyncCore(request, next, cancellationToken);

    /// <inheritdoc/>
    public Task<TResponse> HandleAsync<TContext>(
        TRequest request,
        RequestHandlerContextDelegate<TResponse, TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken
    )
        where TContext : class, new()
        => HandleAsyncCore(request, () => next(context), cancellationToken);

    private async Task<TResponse> HandleAsyncCore(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures =
            await ValidationUtilities.ValidateAsync(request, _validators, cancellationToken).ConfigureAwait(false);

        if (validationFailures.Length == 0)
            return await next().ConfigureAwait(false);

        throw ValidationUtilities.CreateValidationErrorResult(validationFailures);
    }
}

/// <summary>
/// Represents a validation behavior for handling requests without a response.
/// When validation fails, a <see cref="RA.Utilities.Core.Exceptions.BadRequestException"/> is thrown.
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
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators ?? [];

    /// <inheritdoc/>
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
        => HandleAsyncCore(request, next, cancellationToken);

    /// <inheritdoc/>
    public Task HandleAsync<TContext>(
        TRequest request,
        RequestHandlerContextDelegate<TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken
    )
        where TContext : class, new()
        => HandleAsyncCore(request, () => next(context), cancellationToken);

    private async Task HandleAsyncCore(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures =
            await ValidationUtilities.ValidateAsync(request, _validators, cancellationToken).ConfigureAwait(false);

        if (validationFailures.Length == 0)
        {
            await next().ConfigureAwait(false);
            return;
        }

        throw ValidationUtilities.CreateValidationErrorResult(validationFailures);
    }
}
