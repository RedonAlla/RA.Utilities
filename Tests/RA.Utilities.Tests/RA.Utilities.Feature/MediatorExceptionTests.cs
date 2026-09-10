using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Extensions;
using RA.Utilities.Feature.Models;
using Xunit;

namespace RA.Utilities.Tests.RA.Utilities.Feature;

/// <summary>
/// Locks in the exception contract of the mediator after the Result removal: request handler and
/// pipeline behavior exceptions propagate to the caller, while notification handler exceptions
/// keep the existing logged-and-swallowed semantics.
/// </summary>
public class MediatorExceptionTests
{
    [Fact]
    public async Task Send_PropagatesHandlerExceptions()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        Func<Task> act = () => mediator.Send<ThrowingRequest, string>(new ThrowingRequest());

        (await act.Should().ThrowAsync<NotFoundException>())
            .Which.EntityName.Should().Be("ThrowingRequest");
    }

    [Fact]
    public async Task Send_WithContext_PropagatesHandlerExceptions()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        var context = new PipelineContext<ExceptionTestContext>();

        Func<Task> act = () => mediator.Send<ThrowingRequest, string, ExceptionTestContext>(
            new ThrowingRequest(), context);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Send_Void_PropagatesHandlerExceptions()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        Func<Task> act = () => mediator.Send(new ThrowingVoidRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Send_PropagatesBehaviorExceptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<IPipelineBehavior<BehaviorThrowingRequest, string>, ThrowingBehavior>();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        Func<Task> act = () => mediator.Send<BehaviorThrowingRequest, string>(new BehaviorThrowingRequest());

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Publish_SwallowsNotificationHandlerExceptions()
    {
        using ServiceProvider provider = CreateProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        Func<Task> act = () => mediator.Publish(new FaultyNotification());

        // The existing publish semantics log the handler failure and continue.
        await act.Should().NotThrowAsync();
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        return services.BuildServiceProvider();
    }
}

public class ExceptionTestContext
{
    public string? CorrelationId { get; set; }
}

public record ThrowingRequest : IRequest<string>;

public record ThrowingVoidRequest : IRequest;

public record BehaviorThrowingRequest : IRequest<string>;

public record FaultyNotification : INotification;

public class ThrowingHandler : IRequestHandler<ThrowingRequest, string>
{
    public Task<string> HandleAsync(ThrowingRequest request, CancellationToken cancellationToken) =>
        throw new NotFoundException("ThrowingRequest", "none");
}

public class ThrowingVoidHandler : IRequestHandler<ThrowingVoidRequest>
{
    public Task HandleAsync(ThrowingVoidRequest request, CancellationToken cancellationToken) =>
        throw new NotFoundException("ThrowingVoidRequest", "none");
}

public class BehaviorThrowingHandler : IRequestHandler<BehaviorThrowingRequest, string>
{
    public Task<string> HandleAsync(BehaviorThrowingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult("ok");
}

public class ThrowingBehavior : IPipelineBehavior<BehaviorThrowingRequest, string>
{
    public Task<string> HandleAsync(BehaviorThrowingRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
        throw new ForbiddenException();
}

public class FaultyNotificationHandler : INotificationHandler<FaultyNotification>
{
    public Task HandleAsync(FaultyNotification notification, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("handler failure");
}
