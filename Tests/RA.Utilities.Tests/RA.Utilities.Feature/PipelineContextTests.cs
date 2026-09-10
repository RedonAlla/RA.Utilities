using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Extensions;
using RA.Utilities.Feature.Models;
using Xunit;

namespace RA.Utilities.Tests.RA.Utilities.Feature;

public class PipelineContextTests
{
    #region PipelineContext Basics

    [Fact]
    public void NewContext_HasDefaultData()
    {
        var ctx = new PipelineContext<MyContext>();
        ctx.Data.Should().NotBeNull();
        ctx.Data.CorrelationId.Should().BeNull();
        ctx.Data.UserId.Should().Be(0);
    }

    [Fact]
    public void Context_DataIsReadWrite()
    {
        var ctx = new PipelineContext<MyContext>();
        ctx.Data.CorrelationId = "abc-123";
        ctx.Data.UserId = 42;

        ctx.Data.CorrelationId.Should().Be("abc-123");
        ctx.Data.UserId.Should().Be(42);
    }

    #endregion

    #region Context Isolation Between Executions

    [Fact]
    public async Task Context_IsIsolated_BetweenSeparateSendCalls()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        var ctx1 = new PipelineContext<MyContext>();
        ctx1.Data.CorrelationId = "first";

        string result1 = await mediator.Send<SetContextRequest, string, MyContext>(
            new SetContextRequest("key1", "value1"), ctx1);

        var ctx2 = new PipelineContext<MyContext>();
        ctx2.Data.CorrelationId = "second";

        string result2 = await mediator.Send<SetContextRequest, string, MyContext>(
            new SetContextRequest("key2", "value2"), ctx2);

        result1.Should().Be("key1:value1:value1");
        result2.Should().Be("key2:value2:value2");
    }

    #endregion

    #region Context Propagation Through Behaviors

    [Fact]
    public async Task Context_SetInBehavior_IsVisibleInHandler()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<IPipelineBehavior<ContextAwareRequest, string>, ContextWritingBehavior>();

        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        string result = await mediator.Send<ContextAwareRequest, string, MyContext>(
            new ContextAwareRequest("hello"), null);

        result.Should().Be("hello:set-by-behavior");
    }

    [Fact]
    public async Task Context_ModificationInFirstBehavior_IsVisibleInSecondBehavior()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<IPipelineBehavior<ContextAwareRequest, string>, ContextWritingBehavior>();
        services.AddTransient<IPipelineBehavior<ContextAwareRequest, string>, ContextAppendingBehavior>();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        string result = await mediator.Send<ContextAwareRequest, string, MyContext>(
            new ContextAwareRequest("hello"), null);

        result.Should().Be("hello:set-by-behavior:appended");
    }

    #endregion

    #region Backward Compatibility (No Context Type Param)

    [Fact]
    public async Task Send_WithoutContextTypeParam_StillWorks()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        string result = await mediator.Send<NoContextRequest, string>(
            new NoContextRequest("test"));

        result.Should().Be("test");
    }

    [Fact]
    public async Task Behavior_WithoutContextOverride_StillWorks()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<IPipelineBehavior<NoContextRequest, string>, LegacyLoggingBehavior>();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        string result = await mediator.Send<NoContextRequest, string>(
            new NoContextRequest("test"));

        result.Should().Be("test");
    }

    [Fact]
    public async Task Send_WithContextTypeParam_AndExistingHandler_Works()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        var ctx = new PipelineContext<MyContext>();
        ctx.Data.CorrelationId = "corr-1";

        string result = await mediator.Send<NoContextRequest, string, MyContext>(
            new NoContextRequest("test"), ctx);

        result.Should().Be("test");
    }

    #endregion

    #region Notification Context

    [Fact]
    public async Task Context_SetInNotificationBehavior_IsVisibleInHandler()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        services.AddTransient<INotificationBehavior<TestNotification>, ContextWritingNotificationBehavior>();
        ServiceProvider provider = services.BuildServiceProvider();
        IMediator mediator = provider.GetRequiredService<IMediator>();

        await mediator.Publish<TestNotification, MyContext>(new TestNotification("data"), null);

        ContextReadingNotificationHandler.ReceivedCorrelationId.Should().Be("notif-behavior-set");
    }

    #endregion
}

// ------- Test Types -------

public class MyContext
{
    public string? CorrelationId { get; set; }
    public int UserId { get; set; }
}

public record ContextAwareRequest(string Data) : IRequest<string>;
public record NoContextRequest(string Data) : IRequest<string>;
public record SetContextRequest(string Key, string Value) : IRequest<string>;
public record TestNotification(string Data) : INotification;

public class ContextReadingHandler :
    IRequestHandler<ContextAwareRequest, string>,
    IRequestHandler<SetContextRequest, string>
{
    public Task<string> HandleAsync(ContextAwareRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, new PipelineContext<MyContext>(), cancellationToken);

    public Task<string> HandleAsync<TContext>(ContextAwareRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        string suffix = context is PipelineContext<MyContext> ctx ? ctx.Data.CorrelationId ?? "none" : "none";
        return Task.FromResult($"{request.Data}:{suffix}");
    }

    public Task<string> HandleAsync(SetContextRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, new PipelineContext<MyContext>(), cancellationToken);

    public Task<string> HandleAsync<TContext>(SetContextRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyContext> ctx)
            ctx.Data.CorrelationId = request.Value;

        string? corrId = context is PipelineContext<MyContext> c ? c.Data.CorrelationId : "none";
        return Task.FromResult($"{request.Key}:{request.Value}:{corrId}");
    }
}

public class ContextWritingBehavior : IPipelineBehavior<ContextAwareRequest, string>
{
    public Task<string> HandleAsync(ContextAwareRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
        => HandleAsync(request, _ => next(), new PipelineContext<MyContext>(), cancellationToken);

    public async Task<string> HandleAsync<TContext>(ContextAwareRequest request, RequestHandlerContextDelegate<string, TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyContext> ctx)
            ctx.Data.CorrelationId = "set-by-behavior";
        return await next(context);
    }
}

public class ContextAppendingBehavior : IPipelineBehavior<ContextAwareRequest, string>
{
    public Task<string> HandleAsync(ContextAwareRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
        => HandleAsync(request, _ => next(), new PipelineContext<MyContext>(), cancellationToken);

    public async Task<string> HandleAsync<TContext>(ContextAwareRequest request, RequestHandlerContextDelegate<string, TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyContext> ctx && ctx.Data.CorrelationId != null)
            ctx.Data.CorrelationId += ":appended";
        return await next(context);
    }
}

public class NoContextHandler : IRequestHandler<NoContextRequest, string>
{
    public Task<string> HandleAsync(NoContextRequest request, CancellationToken cancellationToken)
        => Task.FromResult(request.Data);
}

public class LegacyLoggingBehavior : IPipelineBehavior<NoContextRequest, string>
{
    public async Task<string> HandleAsync(NoContextRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
        => await next();
}

public class ContextWritingNotificationBehavior : INotificationBehavior<TestNotification>
{
    public Task HandleAsync(TestNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
        => HandleAsync(notification, _ => next(), new PipelineContext<MyContext>(), cancellationToken);

    public async Task HandleAsync<TContext>(TestNotification notification, NotificationHandlerContextDelegate<TContext> next, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyContext> ctx)
            ctx.Data.CorrelationId = "notif-behavior-set";
        await next(context);
    }
}

public class ContextReadingNotificationHandler : INotificationHandler<TestNotification>
{
    public static string? ReceivedCorrelationId { get; private set; }

    public Task HandleAsync(TestNotification notification, CancellationToken cancellationToken)
        => HandleAsync(notification, new PipelineContext<MyContext>(), cancellationToken);

    public Task HandleAsync<TContext>(TestNotification notification, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        ReceivedCorrelationId = context is PipelineContext<MyContext> ctx ? ctx.Data.CorrelationId : "none";
        return Task.CompletedTask;
    }
}
