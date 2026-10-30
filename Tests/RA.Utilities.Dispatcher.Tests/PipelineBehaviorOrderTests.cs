using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Attributes;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Models;
using RA.Utilities.Dispatcher.Options;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies that <see cref="PipelineBehaviorOrderAttribute"/> controls the order in which the
/// generator registers closed behaviors, and therefore the order in which they wrap the handler:
/// lower order values run closer to the outside of the chain.
/// </summary>
public class PipelineBehaviorOrderTests
{
    [Fact]
    public async Task Send_RunsBehaviorsFromLowerToHigherOrder_OutermostFirst()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result<string> result = await dispatcher.Send<OrderedRequest, string>(new OrderedRequest());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("zulu", "middle", "alpha", "handler");
    }

    [Fact]
    public async Task Send_OrdersBehaviorsWithEqualOrderByTypeName()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result<string> result = await dispatcher.Send<TiedRequest, string>(new TiedRequest());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("tied-alpha", "tied-zulu", "tied-handler");
    }

    [Fact]
    public async Task Publish_RunsNotificationBehaviorsFromLowerToHigherOrder_OutermostFirst()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result result = await dispatcher.Publish(new OrderedNotification());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("notification-zulu", "notification-alpha", "notification-handler");
    }

    [Fact]
    public void AddDispatcher_RegistersOrderedBehaviorsInDeclaredOrder()
    {
        using ServiceProvider provider = CreateProvider();

        IEnumerable<string> registered = provider
            .GetServices<IPipelineBehavior<OrderedRequest, string>>()
            .Select(static behavior => behavior.GetType().Name);

        registered.Should().Equal("ZuluOrderedBehavior", "MiddleOrderedBehavior", "AlphaOrderedBehavior");
    }

    [Fact]
    public async Task Send_HonorsOrderForVoidRequests()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result result = await dispatcher.Send(new OrderedVoidRequest());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("void-zulu", "void-alpha", "void-handler");
    }

    [Fact]
    public async Task AddDispatcher_ExcludeHandlers_RemovesOnlyTheExcludedBehavior_AndKeepsTheRemainingOrder()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(MiddleOrderedBehavior)]);
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result<string> result = await dispatcher.Send<OrderedRequest, string>(new OrderedRequest());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("zulu", "alpha", "handler");
    }

    [Fact]
    public async Task DecorationAddedBeforeAddDispatcher_WrapsAutoRegisteredBehaviors()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services
            .AddDispatcher<BeforeDecorationRequest, string, BeforeDecorationHandler>()
            .AddDecoration<ExplicitOuterBehavior<BeforeDecorationRequest, string>>();
        services.AddDispatcher();

        using ServiceProvider provider = services.BuildServiceProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result<string> result = await dispatcher.Send<BeforeDecorationRequest, string>(new BeforeDecorationRequest());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("explicit-outer", "auto", "before-handler");
    }

    [Fact]
    public async Task DecorationAddedAfterAddDispatcher_RunsInsideAutoRegisteredBehaviors()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher();
        services
            .AddDispatcher<AfterDecorationRequest, string, AfterDecorationHandler>()
            .AddDecoration<ExplicitInnerBehavior<AfterDecorationRequest, string>>();

        using ServiceProvider provider = services.BuildServiceProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        OrderProbe.Reset();

        Result<string> result = await dispatcher.Send<AfterDecorationRequest, string>(new AfterDecorationRequest());

        result.IsSuccess.Should().BeTrue();
        OrderProbe.Entries.Should().Equal("auto", "explicit-inner", "after-handler");
    }

    private static ServiceProvider CreateProvider(Action<DispatcherOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        if (configure is null)
        {
            services.AddDispatcher();
        }
        else
        {
            services.AddDispatcher(configure);
        }

        return services.BuildServiceProvider();
    }
}

/// <summary>
/// Records the execution order of the ordering fixtures. Each ordering test uses its own message
/// and xUnit runs tests within a class sequentially, so a shared static probe is safe here.
/// </summary>
public static class OrderProbe
{
    private static readonly Lock Sync = new();
    private static readonly List<string> Recorded = [];

    /// <summary>
    /// Gets a snapshot of the recorded entries.
    /// </summary>
    public static IReadOnlyList<string> Entries
    {
        get
        {
            lock (Sync)
            {
                return [.. Recorded];
            }
        }
    }

    /// <summary>
    /// Records an entry.
    /// </summary>
    /// <param name="entry">The entry to record.</param>
    public static void Record(string entry)
    {
        lock (Sync)
        {
            Recorded.Add(entry);
        }
    }

    /// <summary>
    /// Clears the recorded entries.
    /// </summary>
    public static void Reset()
    {
        lock (Sync)
        {
            Recorded.Clear();
        }
    }
}

public record OrderedRequest : IRequest<string>;

public class OrderedRequestHandler : IRequestHandler<OrderedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(OrderedRequest request, CancellationToken cancellationToken)
    {
        OrderProbe.Record("handler");
        return ValueTask.FromResult(Result.Success("ok"));
    }
}

/// <summary>
/// Sorts last alphabetically but declares the lowest order, so it must run first (outermost).
/// </summary>
[PipelineBehaviorOrder(-100)]
public class ZuluOrderedBehavior : IPipelineBehavior<OrderedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        OrderedRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("zulu");
        return next();
    }
}

/// <summary>
/// Has no attribute, so it defaults to order zero between the two annotated behaviors.
/// </summary>
public class MiddleOrderedBehavior : IPipelineBehavior<OrderedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        OrderedRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("middle");
        return next();
    }
}

/// <summary>
/// Sorts first alphabetically but declares the highest order, so it must run last (innermost).
/// </summary>
[PipelineBehaviorOrder(100)]
public class AlphaOrderedBehavior : IPipelineBehavior<OrderedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        OrderedRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("alpha");
        return next();
    }
}

public record TiedRequest : IRequest<string>;

public class TiedRequestHandler : IRequestHandler<TiedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(TiedRequest request, CancellationToken cancellationToken)
    {
        OrderProbe.Record("tied-handler");
        return ValueTask.FromResult(Result.Success("ok"));
    }
}

[PipelineBehaviorOrder(42)]
public class TiedZuluBehavior : IPipelineBehavior<TiedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        TiedRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("tied-zulu");
        return next();
    }
}

[PipelineBehaviorOrder(42)]
public class TiedAlphaBehavior : IPipelineBehavior<TiedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        TiedRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("tied-alpha");
        return next();
    }
}

public record OrderedNotification : INotification;

public class OrderedNotificationHandler : INotificationHandler<OrderedNotification>
{
    public ValueTask<Result> HandleAsync(OrderedNotification notification, CancellationToken cancellationToken)
    {
        OrderProbe.Record("notification-handler");
        return ValueTask.FromResult(Result.Success());
    }
}

[PipelineBehaviorOrder(-10)]
public class ZuluNotificationBehavior : INotificationBehavior<OrderedNotification>
{
    public ValueTask<Result> HandleAsync(
        OrderedNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("notification-zulu");
        return next();
    }
}

[PipelineBehaviorOrder(10)]
public class AlphaNotificationBehavior : INotificationBehavior<OrderedNotification>
{
    public ValueTask<Result> HandleAsync(
        OrderedNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("notification-alpha");
        return next();
    }
}

public record OrderedVoidRequest : IRequest;

public class OrderedVoidRequestHandler : IRequestHandler<OrderedVoidRequest>
{
    public ValueTask<Result> HandleAsync(OrderedVoidRequest request, CancellationToken cancellationToken)
    {
        OrderProbe.Record("void-handler");
        return ValueTask.FromResult(Result.Success());
    }
}

[PipelineBehaviorOrder(-5)]
public class ZuluVoidBehavior : IPipelineBehavior<OrderedVoidRequest>
{
    public ValueTask<Result> HandleAsync(
        OrderedVoidRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("void-zulu");
        return next();
    }
}

[PipelineBehaviorOrder(5)]
public class AlphaVoidBehavior : IPipelineBehavior<OrderedVoidRequest>
{
    public ValueTask<Result> HandleAsync(
        OrderedVoidRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("void-alpha");
        return next();
    }
}

public record BeforeDecorationRequest : IRequest<string>;

public class BeforeDecorationHandler : IRequestHandler<BeforeDecorationRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(BeforeDecorationRequest request, CancellationToken cancellationToken)
    {
        OrderProbe.Record("before-handler");
        return ValueTask.FromResult(Result.Success("ok"));
    }
}

public record AfterDecorationRequest : IRequest<string>;

public class AfterDecorationHandler : IRequestHandler<AfterDecorationRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(AfterDecorationRequest request, CancellationToken cancellationToken)
    {
        OrderProbe.Record("after-handler");
        return ValueTask.FromResult(Result.Success("ok"));
    }
}

/// <summary>
/// Auto-registered behavior shared by the decoration-placement requests; it has no explicit order,
/// so it sits at the default position between the outer and inner explicit decorations.
/// </summary>
public class AutoDecorationProbeBehavior :
    IPipelineBehavior<BeforeDecorationRequest, string>,
    IPipelineBehavior<AfterDecorationRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        BeforeDecorationRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("auto");
        return next();
    }

    public ValueTask<Result<string>> HandleAsync(
        AfterDecorationRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("auto");
        return next();
    }
}

// The explicit decorations use generic behaviors on purpose: generic behaviors are not
// auto-registered (DSAG010), so each one executes exactly once, at the registered position.
#pragma warning disable DSAG010
public class ExplicitOuterBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("explicit-outer");
        return next();
    }
}

public class ExplicitInnerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        OrderProbe.Record("explicit-inner");
        return next();
    }
}
#pragma warning restore DSAG010
