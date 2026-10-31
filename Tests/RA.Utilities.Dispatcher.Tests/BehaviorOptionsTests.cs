using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Models;
using RA.Utilities.Dispatcher.Options;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the behaviors listed in <see cref="DispatcherOptions.Behaviors"/>: open and closed
/// registration, chain position, lifetime, idempotency, filtering, and validation.
/// </summary>
public class BehaviorOptionsTests
{
    [Fact]
    public void AddDispatcher_Behaviors_RegistersOpenGenericRequestBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.Behaviors.Add(typeof(ConfiguredOpenBehavior<,>)));

        provider.GetServices<IPipelineBehavior<ConfiguredOpenRequest, string>>()
            .Should().ContainSingle()
            .Which.Should().BeOfType<ConfiguredOpenBehavior<ConfiguredOpenRequest, string>>();
    }

    [Fact]
    public async Task AddDispatcher_Behaviors_RunsOpenGenericRequestBehaviorAroundTheHandler()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.Behaviors.Add(typeof(ConfiguredOpenBehavior<,>)));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        ConfiguredBehaviorProbe.Reset();

        Result<string> result = await dispatcher.Send<ConfiguredOpenRequest, string>(new ConfiguredOpenRequest());

        result.IsSuccess.Should().BeTrue();
        ConfiguredBehaviorProbe.Entries.Should().Equal("open", "open-handler");
    }

    [Fact]
    public async Task AddDispatcher_Behaviors_RunsOpenGenericVoidBehaviorAroundTheHandler()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.Behaviors.Add(typeof(ConfiguredOpenVoidBehavior<>)));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        ConfiguredBehaviorProbe.Reset();

        Result result = await dispatcher.Send(new ConfiguredVoidRequest());

        result.IsSuccess.Should().BeTrue();
        ConfiguredBehaviorProbe.Entries.Should().Equal("void-open", "void-handler");
    }

    [Fact]
    public async Task AddDispatcher_Behaviors_RunsOpenGenericNotificationBehaviorAroundTheHandler()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.Behaviors.Add(typeof(ConfiguredOpenNotificationBehavior<>)));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
        ConfiguredBehaviorProbe.Reset();

        Result result = await dispatcher.Publish(new ConfiguredNotification());

        result.IsSuccess.Should().BeTrue();
        ConfiguredBehaviorProbe.Entries.Should().Equal("notification-open", "notification-handler");
    }

    [Fact]
    public async Task AddDispatcher_Behaviors_RegistersClosedBehaviorOnceAtTheConfiguredPosition()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.Behaviors.Add(typeof(ConfiguredExplicitBehavior)));
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        provider.GetServices<IPipelineBehavior<ConfiguredClosedRequest, string>>()
            .Count(static behavior => behavior is ConfiguredExplicitBehavior)
            .Should().Be(1);

        ConfiguredBehaviorProbe.Reset();
        Result<string> result = await dispatcher.Send<ConfiguredClosedRequest, string>(new ConfiguredClosedRequest());

        result.IsSuccess.Should().BeTrue();

        // Configured before the generated registrations, so it wraps the auto-registered behavior.
        ConfiguredBehaviorProbe.Entries.Should().Equal("explicit", "auto", "closed-handler");
    }

    [Fact]
    public void AddDispatcher_Behaviors_UsesTheConfiguredLifetime()
    {
        var services = new ServiceCollection();

        services.AddDispatcher(options =>
        {
            options.BehaviorLifetime = ServiceLifetime.Scoped;
            options.Behaviors.Add(typeof(ConfiguredOpenBehavior<,>));
        });

        services.Single(static descriptor => descriptor.ServiceType == typeof(IPipelineBehavior<,>))
            .Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddDispatcher_Behaviors_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();

        services.AddDispatcher(options => options.Behaviors.Add(typeof(ConfiguredOpenBehavior<,>)));
        services.AddDispatcher(options => options.Behaviors.Add(typeof(ConfiguredOpenBehavior<,>)));

        services.Count(static descriptor => descriptor.ServiceType == typeof(IPipelineBehavior<,>))
            .Should().Be(1);
    }

    [Fact]
    public void AddDispatcher_Behaviors_SurviveHandlerFiltering()
    {
        using ServiceProvider provider = CreateProvider(options =>
        {
            options.IncludeHandlers = [];
            options.Behaviors.Add(typeof(ConfiguredOpenBehavior<,>));
        });

        provider.GetService<IRequestHandler<ConfiguredOpenRequest, string>>().Should().BeNull();
        provider.GetService<IPipelineBehavior<ConfiguredOpenRequest, string>>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_Behaviors_RejectsTypesThatAreNotBehaviors()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddDispatcher(options => options.Behaviors.Add(typeof(string)));

        act.Should().Throw<ArgumentException>().WithMessage("*does not implement*");
    }

    [Fact]
    public void AddDispatcher_Behaviors_RejectsAbstractBehaviors()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddDispatcher(options =>
            options.Behaviors.Add(typeof(AbstractConfiguredBehavior)));

        act.Should().Throw<ArgumentException>().WithMessage("*non-abstract class*");
    }

    [Fact]
    public void AddDispatcher_Behaviors_RejectsNullEntries()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddDispatcher(options => options.Behaviors.Add(null!));

        act.Should().Throw<ArgumentNullException>();
    }

    private static ServiceProvider CreateProvider(Action<DispatcherOptions> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher(configure);
        return services.BuildServiceProvider();
    }
}

/// <summary>
/// Records the execution order of the configured behavior fixtures. Tests within this class run
/// sequentially, so a shared static probe is safe here.
/// </summary>
public static class ConfiguredBehaviorProbe
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

public record ConfiguredOpenRequest : IRequest<string>;

public class ConfiguredOpenRequestHandler : IRequestHandler<ConfiguredOpenRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        ConfiguredOpenRequest request,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("open-handler");
        return ValueTask.FromResult(Result.Success("ok"));
    }
}

public record ConfiguredVoidRequest : IRequest;

public class ConfiguredVoidRequestHandler : IRequestHandler<ConfiguredVoidRequest>
{
    public ValueTask<Result> HandleAsync(
        ConfiguredVoidRequest request,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("void-handler");
        return ValueTask.FromResult(Result.Success());
    }
}

public record ConfiguredNotification : INotification;

public class ConfiguredNotificationHandler : INotificationHandler<ConfiguredNotification>
{
    public ValueTask<Result> HandleAsync(
        ConfiguredNotification notification,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("notification-handler");
        return ValueTask.FromResult(Result.Success());
    }
}

public record ConfiguredClosedRequest : IRequest<string>;

public class ConfiguredClosedRequestHandler : IRequestHandler<ConfiguredClosedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        ConfiguredClosedRequest request,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("closed-handler");
        return ValueTask.FromResult(Result.Success("ok"));
    }
}

/// <summary>
/// Closed behavior that the source generator auto-registers; used to observe where a configured
/// behavior sits relative to the generated block.
/// </summary>
public class ConfiguredAutoBehavior : IPipelineBehavior<ConfiguredClosedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        ConfiguredClosedRequest request,
        RequestHandlerDelegate<string> next,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("auto");
        return next();
    }
}

/// <summary>
/// Closed behavior that is both auto-registered and listed in <see cref="DispatcherOptions.Behaviors"/>.
/// It must execute exactly once, at the configured (outer) position.
/// </summary>
public class ConfiguredExplicitBehavior : IPipelineBehavior<ConfiguredClosedRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        ConfiguredClosedRequest request,
        RequestHandlerDelegate<string> next,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("explicit");
        return next();
    }
}

public abstract class AbstractConfiguredBehavior : IPipelineBehavior<ConfiguredOpenRequest, string>
{
    public abstract ValueTask<Result<string>> HandleAsync(
        ConfiguredOpenRequest request,
        RequestHandlerDelegate<string> next,
        CancellationToken cancellationToken);
}

// Generic behaviors are not auto-registered (DSAG010) — registering them through the options list
// is exactly the scenario under test.
#pragma warning disable DSAG010
public class ConfiguredOpenBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("open");
        return next();
    }
}

public class ConfiguredOpenVoidBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public ValueTask<Result> HandleAsync(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("void-open");
        return next();
    }
}

public class ConfiguredOpenNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    public ValueTask<Result> HandleAsync(
        TNotification notification,
        NotificationHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        ConfiguredBehaviorProbe.Record("notification-open");
        return next();
    }
}
#pragma warning restore DSAG010
