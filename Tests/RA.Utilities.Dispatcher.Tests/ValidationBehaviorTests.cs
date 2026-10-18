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
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies that the validation behavior returns a failed <see cref="Result"/> containing a
/// <see cref="BadRequestException"/> with the mapped validation errors — the handler is never
/// invoked for invalid requests.
/// </summary>
public class ValidationBehaviorTests
{
    [Fact]
    public async Task ValidRequest_PassesThroughToHandler()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<CreateOrderResult> result = await dispatcher.Send<CreateOrderCommand, CreateOrderResult>(
            new CreateOrderCommand("Coffee", 2));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Coffee");
    }

    [Fact]
    public async Task InvalidRequest_ReturnsFailedResult_WithMappedErrors_AndSkipsHandler()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        CreateOrderHandler.ReceivedNames.Clear();
        Result<CreateOrderResult> result = await dispatcher.Send<CreateOrderCommand, CreateOrderResult>(
            new CreateOrderCommand("", 0));

        result.IsFailure.Should().BeTrue();
        BadRequestException exception = result.Exception.Should().BeOfType<BadRequestException>().Which;
        exception.Errors.Should().Contain(error => error.PropertyName == "Name");
        exception.Errors.Should().Contain(error => error.PropertyName == "Quantity");
        CreateOrderHandler.ReceivedNames.Should().BeEmpty();
    }

    [Fact]
    public async Task InvalidRequest_OnContextSend_ReturnsFailedResult()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<CreateOrderResult> result = await dispatcher.Send<CreateOrderCommand, CreateOrderResult, ValidationTestContext>(
            new CreateOrderCommand("", 0), new PipelineContext<ValidationTestContext>());

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [Fact]
    public async Task ValidVoidRequest_PassesThroughToHandler()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Send(new DeleteOrderCommand(7));

        result.IsSuccess.Should().BeTrue();
        DeleteOrderHandler.ReceivedOrderIds.Should().Contain(7);
    }

    [Fact]
    public async Task InvalidVoidRequest_ReturnsFailedResult()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Send(new DeleteOrderCommand(0));

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [Fact]
    public async Task MultipleValidators_EachReportTheirOwnFailures_WithoutLeaking()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<IValidator<MultiValidatedCommand>, MultiValidatedFirstValidator>();
        services.AddTransient<IValidator<MultiValidatedCommand>, MultiValidatedSecondValidator>();
        services.AddTransient<IPipelineBehavior<MultiValidatedCommand, string>, ValidationBehavior<MultiValidatedCommand, string>>();
        using ServiceProvider provider = services.BuildServiceProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        // Fails only the second validator's rule (max length 5): the failure list must contain
        // exactly that one error — a shared ValidationContext would duplicate it per validator.
        Result<string> result = await dispatcher.Send<MultiValidatedCommand, string>(
            new MultiValidatedCommand("TooLongName"));

        result.IsFailure.Should().BeTrue();
        BadRequestException exception = result.Exception.Should().BeOfType<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle().Which.PropertyName.Should().Be("Name");
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<IValidator<CreateOrderCommand>, CreateOrderValidator>();
        services.AddTransient<IPipelineBehavior<CreateOrderCommand, CreateOrderResult>, ValidationBehavior<CreateOrderCommand, CreateOrderResult>>();
        services.AddTransient<IValidator<DeleteOrderCommand>, DeleteOrderValidator>();
        services.AddTransient<IPipelineBehavior<DeleteOrderCommand>, ValidationBehavior<DeleteOrderCommand>>();
        return services.BuildServiceProvider();
    }
}

public class ValidationTestContext
{
    public string? CorrelationId { get; set; }
}

public record CreateOrderCommand(string Name, int Quantity) : IRequest<CreateOrderResult>;

public record CreateOrderResult(string Name);

public record DeleteOrderCommand(int OrderId) : IRequest;

public record MultiValidatedCommand(string Name) : IRequest<string>;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public static System.Collections.Generic.List<string> ReceivedNames { get; } = [];

    public ValueTask<Result<CreateOrderResult>> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        ReceivedNames.Add(request.Name);
        return ValueTask.FromResult(Result.Success(new CreateOrderResult(request.Name)));
    }
}

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand>
{
    public static System.Collections.Generic.List<int> ReceivedOrderIds { get; } = [];

    public ValueTask<Result> HandleAsync(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        ReceivedOrderIds.Add(request.OrderId);
        return ValueTask.FromResult(Result.Success());
    }
}

public class MultiValidatedHandler : IRequestHandler<MultiValidatedCommand, string>
{
    public ValueTask<Result<string>> HandleAsync(MultiValidatedCommand request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success(request.Name));
}

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
        RuleFor(command => command.Quantity).GreaterThan(0);
    }
}

public class DeleteOrderValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderValidator()
    {
        RuleFor(command => command.OrderId).GreaterThan(0);
    }
}

public class MultiValidatedFirstValidator : AbstractValidator<MultiValidatedCommand>
{
    public MultiValidatedFirstValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
    }
}

public class MultiValidatedSecondValidator : AbstractValidator<MultiValidatedCommand>
{
    public MultiValidatedSecondValidator()
    {
        RuleFor(command => command.Name).MaximumLength(5);
    }
}
