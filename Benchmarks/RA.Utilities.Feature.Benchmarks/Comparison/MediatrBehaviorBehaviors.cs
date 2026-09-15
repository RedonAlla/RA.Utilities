#pragma warning disable CA1873 // The destructured logging mirrors the package behaviors verbatim; the null logger disables it anyway.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

// MediatR counterparts of the package's request behaviors, registered per closed construction
// through config.AddBehavior so each scenario's pipeline stays fixed by its request type (the
// open generic types are not registered globally). The logging behaviors make the same log
// calls at the same levels; the validation behaviors run the same asynchronous FluentValidation
// work over the same rules.

/// <summary>
/// MediatR logging behavior mirroring the package's
/// <c>LoggingBehavior&lt;TRequest, TResponse&gt;</c>: logs the request before and the response
/// after the handler at Information level.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class MediatRLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<MediatRLoggingBehavior<TRequest, TResponse>> _logger;

    public MediatRLoggingBehavior(ILogger<MediatRLoggingBehavior<TRequest, TResponse>> logger) =>
        _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        TResponse response = await next(cancellationToken);
        _logger.LogInformation("[Request Logging] Finished. Response: {@Response}", response);
        return response;
    }
}

/// <summary>
/// MediatR validation behavior mirroring the package's <c>ValidationBehavior</c>. Like the
/// package behavior it validates asynchronously over every registered validator; MediatR's
/// canonical FluentValidation sample validates synchronously, but the async path is used on
/// both sides so the comparison isolates dispatch machinery, not validation style. Throws
/// <see cref="ValidationException"/> on failure (the package throws a
/// <c>BadRequestException</c> built from the same failures — equivalent work on both paths).
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class MediatRValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public MediatRValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        ValidationResult[] results = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        ValidationFailure[] failures = [.. results
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)];

        if (failures.Length > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}

/// <summary>
/// MediatR logging behavior for void requests, mirroring the package's
/// <c>LoggingBehavior&lt;TRequest&gt;</c>. MediatR 12 runs void commands through behaviors
/// typed over <see cref="Unit"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
internal sealed class MediatRUnitLoggingBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : notnull
{
    private readonly ILogger<MediatRUnitLoggingBehavior<TRequest>> _logger;

    public MediatRUnitLoggingBehavior(ILogger<MediatRUnitLoggingBehavior<TRequest>> logger) =>
        _logger = logger;

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Request Logging] Start. Request: {@Request}", request);
        Unit response = await next(cancellationToken);
        _logger.LogInformation("[Request Logging] Finished.");
        return response;
    }
}

/// <summary>
/// MediatR validation behavior for void requests, mirroring the package's void
/// <c>ValidationBehavior</c>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
internal sealed class MediatRUnitValidationBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public MediatRUnitValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        ValidationResult[] results = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        ValidationFailure[] failures = [.. results
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)];

        if (failures.Length > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
