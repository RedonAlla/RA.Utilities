using System;
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
/// Verifies the include/exclude filtering of auto-registered handlers and of the behaviors related
/// to the messages those handlers handle.
/// </summary>
public class RegistrationOptionsTests
{
    [Fact]
    public void AddDispatcher_WithoutOptions_RegistersHandlerAndRelatedBehavior()
    {
        using ServiceProvider provider = CreateProvider();

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_EmptyOptions_RegistersEverything()
    {
        using ServiceProvider provider = CreateProvider(static _ => { });

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<INotificationBehavior<FanOutNotification>>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeHandlers_RemovesHandlerAndRelatedBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(ClosedBehaviorHandler)]);

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().BeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().BeNull();

        // Unrelated handlers and the dispatcher itself are still registered.
        provider.GetService<IRequestHandler<PlainResponseRequest, string>>().Should().NotBeNull();
        provider.GetService<IDispatcher>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeHandlers_RemovesConcreteSelfRegistration()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(ClosedBehaviorHandler)]);

        provider.GetService<ClosedBehaviorHandler>().Should().BeNull();
        provider.GetService<PlainResponseHandler>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeUnknownType_IsIgnored()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(string)]);

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_IncludeHandlers_RegistersOnlyListedHandlerAndItsBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.IncludeHandlers = [typeof(ClosedBehaviorHandler)]);

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<ClosedBehaviorHandler>().Should().NotBeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IRequestHandler<PlainResponseRequest, string>>().Should().BeNull();
        provider.GetService<PlainResponseHandler>().Should().BeNull();
    }

    [Fact]
    public void AddDispatcher_EmptyIncludeHandlers_RegistersNoHandlers()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.IncludeHandlers = []);

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().BeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().BeNull();
        provider.GetService<IDispatcher>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_IncludeBehaviorOnly_KeepsBehaviorAndDropsHandlers()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.IncludeHandlers = [typeof(ClosedNoOpBehavior)]);

        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().BeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeHandlers_RemovesListedBehaviorOnly()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(ClosedNoOpBehavior)]);

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().BeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeWinsOverInclude()
    {
        using ServiceProvider provider = CreateProvider(options =>
        {
            options.IncludeHandlers = [typeof(ClosedBehaviorHandler)];
            options.ExcludeHandlers = [typeof(ClosedBehaviorHandler)];
        });

        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().BeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().BeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeOneNotificationHandler_KeepsTheOtherAndTheBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(FanOutHandlerA)]);

        provider.GetServices<INotificationHandler<FanOutNotification>>()
            .Select(handler => handler.GetType())
            .Should().ContainSingle()
            .Which.Should().Be<FanOutHandlerB>();
        provider.GetService<INotificationBehavior<FanOutNotification>>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_ExcludeAllNotificationHandlers_RemovesTheBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.ExcludeHandlers = [typeof(FanOutHandlerA), typeof(FanOutHandlerB)]);

        provider.GetServices<INotificationHandler<FanOutNotification>>().Should().BeEmpty();
        provider.GetService<INotificationBehavior<FanOutNotification>>().Should().BeNull();
    }

    [Fact]
    public void AddDispatcher_IncludeOneNotificationHandler_KeepsItsBehavior()
    {
        using ServiceProvider provider = CreateProvider(options =>
            options.IncludeHandlers = [typeof(FanOutHandlerB)]);

        provider.GetServices<INotificationHandler<FanOutNotification>>()
            .Select(handler => handler.GetType())
            .Should().ContainSingle()
            .Which.Should().Be<FanOutHandlerB>();
        provider.GetService<INotificationBehavior<FanOutNotification>>().Should().NotBeNull();
    }

    [Fact]
    public void AddDispatcher_FilterDoesNotRemoveExplicitRegistrations()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher<ClosedBehaviorRequest, string, ClosedBehaviorHandler>();
        services.AddDispatcher(options =>
            options.ExcludeHandlers = [typeof(ClosedBehaviorHandler)]);

        using ServiceProvider provider = services.BuildServiceProvider();

        // The explicitly registered handler survives; only the auto-registered behavior is filtered.
        provider.GetService<IRequestHandler<ClosedBehaviorRequest, string>>().Should().NotBeNull();
        provider.GetService<IPipelineBehavior<ClosedBehaviorRequest, string>>().Should().BeNull();
    }

    [Fact]
    public void AddDispatcher_NullConfigure_Throws()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddDispatcher((Action<DispatcherOptions>)null!);

        act.Should().Throw<ArgumentNullException>();
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
/// Closed notification behavior used to verify that notification behaviors follow the handlers of
/// their notification.
/// </summary>
public sealed class FanOutNoOpBehavior : INotificationBehavior<FanOutNotification>
{
    /// <inheritdoc/>
    public ValueTask<Result> HandleAsync(
        FanOutNotification notification,
        NotificationHandlerDelegate next,
        CancellationToken cancellationToken) => next();
}
