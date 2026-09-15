using FluentValidation;

namespace RA.Utilities.Feature.Benchmarks.Feature;

// FluentValidation validators for the validation-behavior scenarios. A passing request carries
// Value = "payload" (non-empty, at least three characters); a failing request carries an empty
// Value, so the NotEmpty rule fails and the validation behavior throws a BadRequestException
// before the handler ever runs.

/// <summary>
/// Validator for <see cref="ValidatedPingRequest"/>.
/// </summary>
internal sealed class ValidatedPingRequestValidator : AbstractValidator<ValidatedPingRequest>
{
    public ValidatedPingRequestValidator() =>
        RuleFor(request => request.Value).NotEmpty().MinimumLength(3);
}

/// <summary>
/// Validator for <see cref="LoggingValidatedPingRequest"/>.
/// </summary>
internal sealed class LoggingValidatedPingRequestValidator : AbstractValidator<LoggingValidatedPingRequest>
{
    public LoggingValidatedPingRequestValidator() =>
        RuleFor(request => request.Value).NotEmpty().MinimumLength(3);
}

/// <summary>
/// Validator for <see cref="VoidBehaviorCommand"/>.
/// </summary>
internal sealed class VoidBehaviorCommandValidator : AbstractValidator<VoidBehaviorCommand>
{
    public VoidBehaviorCommandValidator() =>
        RuleFor(request => request.Value).NotEmpty().MinimumLength(3);
}
