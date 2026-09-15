using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Generated;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Benchmarks the generated mediator's notification path: the plain fan-out shapes that mirror
/// <c>MediatRPublishBenchmarks</c> scenario for scenario, and the package's notification
/// pipeline behaviors — <c>NotificationLoggingBehavior</c>, <c>NotificationMetricsBehavior</c>,
/// and <c>NotificationRetryBehavior</c> — against the bare publish, each alone and stacked,
/// plus the retry behavior's failure-and-recovery path. MediatR has no notification pipeline,
/// so the behavior scenarios have no comparison counterpart. Each notification type owns its
/// behavior set through the DI registrations, and every iteration resolves the generated
/// mediator from a fresh scope to mirror per-request semantics. All handlers are no-ops
/// (except the intentionally flaky one), and all loggers are null loggers, so the
/// measurements isolate the behaviors themselves.
/// </summary>
[MemoryDiagnoser]
public class NotificationBehaviorBenchmarks
{
    private readonly OrderPlacedNotification _orderPlaced = new("payload");
    private readonly OrderFannedNotification _orderFanned = new("payload");
    private readonly LoggedNotification _loggedNotification = new("payload");
    private readonly MeasuredNotification _measuredNotification = new("payload");
    private readonly RetryImmediateNotification _retryImmediateNotification = new("payload");
    private readonly RetryNotification _retryNotification = new("payload");
    private readonly StackedNotification _stackedNotification = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(BehaviorServices.ConfigureNotificationBehaviors);
    }

    [Benchmark]
    public async Task Publish_OneHandler()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_orderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_ThreeHandlers()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_orderFanned, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_Logging()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_loggedNotification, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_Metrics()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_measuredNotification, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_Retry_SucceedsFirstAttempt()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_retryImmediateNotification, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_Retry_SucceedsSecondAttempt()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_retryNotification, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_Logging_Metrics_Retry()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Publish(_stackedNotification, CancellationToken.None);
    }
}
