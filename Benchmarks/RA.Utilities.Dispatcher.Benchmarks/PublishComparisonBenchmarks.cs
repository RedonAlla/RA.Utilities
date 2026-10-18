using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Benchmarks.Scenarios.Dispatcher;
using RA.Utilities.Dispatcher.Benchmarks.Scenarios.Feature;
using RA.Utilities.Dispatcher.Benchmarks.Scenarios.Mediatr;
using RA.Utilities.Dispatcher.Benchmarks.Shared;
using DispatcherGeneratedMediator = RA.Utilities.Dispatcher.Generated.Mediator;
using DispatcherInterface = RA.Utilities.Dispatcher.Abstractions.IDispatcher;
using FeatureGeneratedMediator = RA.Utilities.Feature.Generated.Mediator;
using FeatureMediator = RA.Utilities.Feature.Abstractions.IMediator;

namespace RA.Utilities.Dispatcher.Benchmarks;

/// <summary>
/// Compares notification fan-out across the three implementations: the new
/// <c>RA.Utilities.Dispatcher</c> package, the original <c>RA.Utilities.Feature</c> package, and
/// MediatR, with one handler and with three handlers. For the RA packages both the
/// interface-typed <c>IDispatcher</c>/<c>IMediator</c> path and the concrete source-generated <c>Mediator</c> path
/// are measured; MediatR exposes only the interface path.
/// </summary>
[MemoryDiagnoser]
public class PublishComparisonBenchmarks
{
    private readonly DispatcherOrderPlacedNotification _dispatcherOrderPlaced = new("payload");
    private readonly DispatcherOrderFannedNotification _dispatcherOrderFanned = new("payload");

    private readonly FeatureOrderPlacedNotification _featureOrderPlaced = new("payload");
    private readonly FeatureOrderFannedNotification _featureOrderFanned = new("payload");

    private readonly MediatROrderPlacedNotification _mediatROrderPlaced = new("payload");
    private readonly MediatROrderFannedNotification _mediatROrderFanned = new("payload");

    private IServiceScopeFactory _dispatcherScopeFactory = null!;
    private IServiceScopeFactory _featureScopeFactory = null!;
    private IServiceScopeFactory _mediatRScopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _dispatcherScopeFactory = DiHost.BuildScopeFactory(DispatcherBenchmarkServices.ConfigurePublish);
        _featureScopeFactory = DiHost.BuildScopeFactory(FeatureBenchmarkServices.ConfigurePublish);
        _mediatRScopeFactory = DiHost.BuildScopeFactory(MediatRBenchmarkServices.ConfigurePublish);
    }

    [Benchmark(Baseline = true)]
    public async Task MediatR_Publish_OneHandler()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Publish(_mediatROrderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Publish_OneHandler()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Publish(_dispatcherOrderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Publish_OneHandler()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Publish(_dispatcherOrderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Publish_OneHandler()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Publish(_featureOrderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Publish_OneHandler()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Publish(_featureOrderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task MediatR_Publish_ThreeHandlers()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Publish(_mediatROrderFanned, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Publish_ThreeHandlers()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Publish(_dispatcherOrderFanned, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Publish_ThreeHandlers()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Publish(_dispatcherOrderFanned, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Publish_ThreeHandlers()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Publish(_featureOrderFanned, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Publish_ThreeHandlers()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Publish(_featureOrderFanned, CancellationToken.None);
    }
}
