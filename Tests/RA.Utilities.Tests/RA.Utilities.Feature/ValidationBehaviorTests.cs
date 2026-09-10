using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Behaviors;
using RA.Utilities.Feature.Extensions;
using RA.Utilities.Feature.Models;
using Xunit;

namespace RA.Utilities.Tests.RA.Utilities.Feature;

/// <summary>
/// Verifies that the validation behavior surfaces failures as a thrown
/// <see cref="BadRequestException"/> carrying the mapped validation errors.
/// </summary>
public class ValidationBehaviorTests
{
    [Fact]
    public async Task ValidRequest_PassesThroughToHandler()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        CreateOrderResult result = await mediator.Send<CreateOrderCommand, CreateOrderResult>(
            new CreateOrderCommand("Coffee", 2));

        result.Name.Should().Be("Coffee");
    }

    [Fact]
    public async Task InvalidRequest_ThrowsBadRequestException_WithMappedErrors()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        Func<Task> act = () => mediator.Send<CreateOrderCommand, CreateOrderResult>(
            new CreateOrderCommand("", 0));

        BadRequestException exception = (await act.Should().ThrowAsync<BadRequestException>()).Which;

        exception.Errors.Should().Contain(error => error.PropertyName == "Name");
        exception.Errors.Should().Contain(error => error.PropertyName == "Quantity");
    }

    [Fact]
    public async Task InvalidRequest_OnContextSend_ThrowsBadRequestException()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        var context = new PipelineContext<ValidationTestContext>();

        Func<Task> act = () => mediator.Send<CreateOrderCommand, CreateOrderResult, ValidationTestContext>(
            new CreateOrderCommand("", 0), context);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ValidVoidRequest_PassesThroughToHandler()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        await mediator.Send(new DeleteOrderCommand(7));

        DeleteOrderHandler.ReceivedOrderIds.Should().Contain(7);
    }

    [Fact]
    public async Task InvalidVoidRequest_ThrowsBadRequestException()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        Func<Task> act = () => mediator.Send(new DeleteOrderCommand(0));

        await act.Should().ThrowAsync<BadRequestException>();
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

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public Task<CreateOrderResult> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(new CreateOrderResult(request.Name));
}

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand>
{
    public static System.Collections.Generic.List<int> ReceivedOrderIds { get; } = [];

    public Task HandleAsync(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        ReceivedOrderIds.Add(request.OrderId);
        return Task.CompletedTask;
    }
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
