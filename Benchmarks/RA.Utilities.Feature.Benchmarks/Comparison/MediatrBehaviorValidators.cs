using FluentValidation;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

// FluentValidation validators mirroring the Feature-side ones rule for rule, so both sides of
// the comparison run identical validation work.

/// <summary>
/// Validator for <see cref="MediatRValidatedPingRequest"/>.
/// </summary>
internal sealed class MediatRValidatedPingRequestValidator : AbstractValidator<MediatRValidatedPingRequest>
{
    public MediatRValidatedPingRequestValidator() =>
        RuleFor(request => request.Value).NotEmpty().MinimumLength(3);
}

/// <summary>
/// Validator for <see cref="MediatRLoggingValidatedPingRequest"/>.
/// </summary>
internal sealed class MediatRLoggingValidatedPingRequestValidator : AbstractValidator<MediatRLoggingValidatedPingRequest>
{
    public MediatRLoggingValidatedPingRequestValidator() =>
        RuleFor(request => request.Value).NotEmpty().MinimumLength(3);
}

/// <summary>
/// Validator for <see cref="MediatRVoidBehaviorCommand"/>.
/// </summary>
internal sealed class MediatRVoidBehaviorCommandValidator : AbstractValidator<MediatRVoidBehaviorCommand>
{
    public MediatRVoidBehaviorCommandValidator() =>
        RuleFor(request => request.Value).NotEmpty().MinimumLength(3);
}
