using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Utilities;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Attributes;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Behaviors;

/// <summary>
/// Represents a validation behavior for handling requests with a response.
/// When validation fails, the returned result is a failure containing a
/// <see cref="RA.Utilities.Core.Exceptions.BadRequestException"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
[PipelineBehaviorOrder(0)]
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
    public ValueTask<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => HandleAsyncCore(request, next, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<Result<TResponse>> HandleAsync<TContext>(
        TRequest request,
        RequestHandlerContextDelegate<TResponse, TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken
    )
        where TContext : class, new()
        => HandleAsyncCore(request, () => next(context), cancellationToken);

    private async ValueTask<Result<TResponse>> HandleAsyncCore(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures =
            await ValidationUtilities.ValidateAsync(request, _validators, cancellationToken).ConfigureAwait(false);

        if (validationFailures.Length == 0)
            return await next().ConfigureAwait(false);

        return Result.Failure<TResponse>(ValidationUtilities.CreateValidationErrorResult(validationFailures));
    }
}

/// <summary>
/// Represents a validation behavior for handling requests without a response.
/// When validation fails, the returned result is a failure containing a
/// <see cref="RA.Utilities.Core.Exceptions.BadRequestException"/>.
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
    public ValueTask<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
        => HandleAsyncCore(request, next, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<Result> HandleAsync<TContext>(
        TRequest request,
        RequestHandlerContextDelegate<TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken
    )
        where TContext : class, new()
        => HandleAsyncCore(request, () => next(context), cancellationToken);

    private async ValueTask<Result> HandleAsyncCore(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures =
            await ValidationUtilities.ValidateAsync(request, _validators, cancellationToken).ConfigureAwait(false);

        if (validationFailures.Length == 0)
            return await next().ConfigureAwait(false);

        return Result.Failure(ValidationUtilities.CreateValidationErrorResult(validationFailures));
    }
}
