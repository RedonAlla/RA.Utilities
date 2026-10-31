using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Behaviors;
using RA.Utilities.Dispatcher.Extensions;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the validators listed in <see cref="Options.DispatcherOptions.Validators"/>: the
/// validator and its validation behavior are registered together, the inferred request/response
/// types are used, and explicit or repeated registrations do not duplicate them.
/// </summary>
public class ValidatorOptionsTests
{
    [Fact]
    public void AddDispatcher_Validators_RegistersValidatorAndValidationBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Transient));

        provider.GetServices<IValidator<ValidatorOptionCommand>>()
            .Should().ContainSingle()
            .Which.Should().BeOfType<ValidatorOptionCommandValidator>();

        provider.GetServices<IPipelineBehavior<ValidatorOptionCommand, ValidatorOptionResult>>()
            .Should().ContainSingle()
            .Which.Should().BeOfType<ValidationBehavior<ValidatorOptionCommand, ValidatorOptionResult>>();
    }

    [Fact]
    public async Task AddDispatcher_Validators_RunsValidationAndSkipsTheHandler()
    {
        ValidatorOptionCommandHandler.Reset();
        using ServiceProvider provider = CreateProvider(options =>
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Transient));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<ValidatorOptionResult> result = await dispatcher.Send<ValidatorOptionCommand, ValidatorOptionResult>(
            new ValidatorOptionCommand(""));

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<BadRequestException>();
        ValidatorOptionCommandHandler.InvocationCount.Should().Be(0);
    }

    [Fact]
    public async Task AddDispatcher_Validators_ValidRequestReachesTheHandler()
    {
        ValidatorOptionCommandHandler.Reset();
        using ServiceProvider provider = CreateProvider(options =>
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Transient));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<ValidatorOptionResult> result = await dispatcher.Send<ValidatorOptionCommand, ValidatorOptionResult>(
            new ValidatorOptionCommand("Write tests"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Write tests");
        ValidatorOptionCommandHandler.InvocationCount.Should().Be(1);
    }

    [Fact]
    public async Task AddDispatcher_Validators_RegistersTheVoidBehaviorForRequestWithoutResponse()
    {
        ValidatorOptionVoidHandler.Reset();
        using ServiceProvider provider = CreateProvider(options =>
            options.Validators.Add(typeof(ValidatorOptionVoidValidator), ServiceLifetime.Transient));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        provider.GetServices<IPipelineBehavior<ValidatorOptionVoidCommand>>()
            .Should().ContainSingle()
            .Which.Should().BeOfType<ValidationBehavior<ValidatorOptionVoidCommand>>();

        Result result = await dispatcher.Send(new ValidatorOptionVoidCommand(0));

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<BadRequestException>();
        ValidatorOptionVoidHandler.InvocationCount.Should().Be(0);
    }

    [Fact]
    public void AddDispatcher_Validators_UsesTheConfiguredLifetime()
    {
        var services = new ServiceCollection();

        services.AddDispatcher(options =>
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Singleton));

        services.Single(static descriptor => descriptor.ServiceType == typeof(IValidator<ValidatorOptionCommand>))
            .Lifetime.Should().Be(ServiceLifetime.Singleton);
        services.Single(static descriptor => descriptor.ServiceType == typeof(IPipelineBehavior<ValidatorOptionCommand, ValidatorOptionResult>))
            .Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public async Task AddDispatcher_Validators_WithExplicitRegistrations_DoesNotDuplicate()
    {
        ValidatorOptionCommandHandler.Reset();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<IValidator<ValidatorOptionCommand>, ValidatorOptionCommandValidator>();
        services.AddTransient<IPipelineBehavior<ValidatorOptionCommand, ValidatorOptionResult>, ValidationBehavior<ValidatorOptionCommand, ValidatorOptionResult>>();
        services.AddDispatcher(options =>
        {
            options.Behaviors.Add(typeof(ValidationBehavior<ValidatorOptionCommand, ValidatorOptionResult>));
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Transient);
        });
        using ServiceProvider provider = services.BuildServiceProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        provider.GetServices<IValidator<ValidatorOptionCommand>>().Should().ContainSingle();
        provider.GetServices<IPipelineBehavior<ValidatorOptionCommand, ValidatorOptionResult>>().Should().ContainSingle();

        Result<ValidatorOptionResult> result = await dispatcher.Send<ValidatorOptionCommand, ValidatorOptionResult>(
            new ValidatorOptionCommand(""));

        result.IsFailure.Should().BeTrue();
        ValidatorOptionCommandHandler.InvocationCount.Should().Be(0);
    }

    [Fact]
    public void AddDispatcher_Validators_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();

        services.AddDispatcher(options =>
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Transient));
        services.AddDispatcher(options =>
            options.Validators.Add(typeof(ValidatorOptionCommandValidator), ServiceLifetime.Transient));

        services.Count(static descriptor => descriptor.ServiceType == typeof(IValidator<ValidatorOptionCommand>))
            .Should().Be(1);
        services.Count(static descriptor => descriptor.ServiceType == typeof(IPipelineBehavior<ValidatorOptionCommand, ValidatorOptionResult>))
            .Should().Be(1);
    }

    [Fact]
    public void AddDispatcher_Validators_MultipleValidatorsShareOneBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
        {
            options.Validators.Add(typeof(ValidatorOptionFirstValidator), ServiceLifetime.Transient);
            options.Validators.Add(typeof(ValidatorOptionSecondValidator), ServiceLifetime.Transient);
        });

        provider.GetServices<IValidator<ValidatorOptionCommand>>().Should().HaveCount(2);
        provider.GetServices<IPipelineBehavior<ValidatorOptionCommand, ValidatorOptionResult>>()
            .Should().ContainSingle();
    }

    [Fact]
    public void AddDispatcher_Validators_RejectsTypesThatAreNotValidators()
    {
        Action act = () => new ServiceCollection().AddDispatcher(options =>
            options.Validators.Add(typeof(string), ServiceLifetime.Transient));

        act.Should().Throw<ArgumentException>().WithMessage("*does not implement IValidator*");
    }

    [Fact]
    public void AddDispatcher_Validators_RejectsAbstractAndOpenGenericValidators()
    {
        Action abstractValidator = () => new ServiceCollection().AddDispatcher(options =>
            options.Validators.Add(typeof(AbstractValidatorOptionValidator), ServiceLifetime.Transient));
        Action openGenericValidator = () => new ServiceCollection().AddDispatcher(options =>
            options.Validators.Add(typeof(OpenGenericValidatorOptionValidator<>), ServiceLifetime.Transient));

        abstractValidator.Should().Throw<ArgumentException>().WithMessage("*non-abstract, closed class*");
        openGenericValidator.Should().Throw<ArgumentException>().WithMessage("*non-abstract, closed class*");
    }

    [Fact]
    public void AddDispatcher_Validators_RejectsRequestsThatAreNotDispatched()
    {
        Action act = () => new ServiceCollection().AddDispatcher(options =>
            options.Validators.Add(typeof(NotARequestValidator), ServiceLifetime.Transient));

        act.Should().Throw<ArgumentException>().WithMessage("*implements neither IRequest nor IRequest<TResponse>*");
    }

    private static ServiceProvider CreateProvider(Action<Options.DispatcherOptions> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher(configure);
        return services.BuildServiceProvider();
    }
}

public record ValidatorOptionCommand(string Name) : IRequest<ValidatorOptionResult>;

public record ValidatorOptionResult(string Name);

public class ValidatorOptionCommandHandler : IRequestHandler<ValidatorOptionCommand, ValidatorOptionResult>
{
    private static int _invocationCount;

    public static int InvocationCount => _invocationCount;

    public static void Reset() => _invocationCount = 0;

    public ValueTask<Result<ValidatorOptionResult>> HandleAsync(
        ValidatorOptionCommand request,
        CancellationToken cancellationToken)
    {
        System.Threading.Interlocked.Increment(ref _invocationCount);
        return ValueTask.FromResult(Result.Success(new ValidatorOptionResult(request.Name)));
    }
}

public class ValidatorOptionCommandValidator : AbstractValidator<ValidatorOptionCommand>
{
    public ValidatorOptionCommandValidator() =>
        RuleFor(command => command.Name).NotEmpty();
}

public class ValidatorOptionFirstValidator : AbstractValidator<ValidatorOptionCommand>
{
    public ValidatorOptionFirstValidator() =>
        RuleFor(command => command.Name).NotEmpty();
}

public class ValidatorOptionSecondValidator : AbstractValidator<ValidatorOptionCommand>
{
    public ValidatorOptionSecondValidator() =>
        RuleFor(command => command.Name).MaximumLength(5);
}

public record ValidatorOptionVoidCommand(int Id) : IRequest;

public class ValidatorOptionVoidHandler : IRequestHandler<ValidatorOptionVoidCommand>
{
    private static int _invocationCount;

    public static int InvocationCount => _invocationCount;

    public static void Reset() => _invocationCount = 0;

    public ValueTask<Result> HandleAsync(
        ValidatorOptionVoidCommand request,
        CancellationToken cancellationToken)
    {
        System.Threading.Interlocked.Increment(ref _invocationCount);
        return ValueTask.FromResult(Result.Success());
    }
}

public class ValidatorOptionVoidValidator : AbstractValidator<ValidatorOptionVoidCommand>
{
    public ValidatorOptionVoidValidator() =>
        RuleFor(command => command.Id).GreaterThan(0);
}

public abstract class AbstractValidatorOptionValidator : AbstractValidator<ValidatorOptionCommand>
{
}

public class OpenGenericValidatorOptionValidator<T> : AbstractValidator<T>
{
}

public class NotARequest
{
    public string? Name { get; set; }
}

public class NotARequestValidator : AbstractValidator<NotARequest>
{
    public NotARequestValidator() =>
        RuleFor(model => model.Name).NotEmpty();
}
