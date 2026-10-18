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
/// Compares request dispatch across the three implementations scenario for scenario: the new
/// <c>RA.Utilities.Dispatcher</c> package, the original <c>RA.Utilities.Feature</c> package, and
/// MediatR. For the RA packages both the interface-typed <c>IDispatcher</c>/<c>IMediator</c> path and the concrete
/// source-generated <c>Mediator</c> path are measured; MediatR exposes only the interface path.
/// Every iteration resolves the mediator from a fresh scope to mirror per-request semantics.
/// </summary>
[MemoryDiagnoser]
public class SendComparisonBenchmarks
{
    private readonly DispatcherPingZeroRequest _dispatcherPingZero = new("payload");
    private readonly DispatcherPingOneRequest _dispatcherPingOne = new("payload");
    private readonly DispatcherPingThreeRequest _dispatcherPingThree = new("payload");
    private readonly DispatcherVoidPingRequest _dispatcherVoidPing = new("payload");
    private readonly DispatcherVoidPingThreeRequest _dispatcherVoidPingThree = new("payload");

    private readonly FeaturePingZeroRequest _featurePingZero = new("payload");
    private readonly FeaturePingOneRequest _featurePingOne = new("payload");
    private readonly FeaturePingThreeRequest _featurePingThree = new("payload");
    private readonly FeatureVoidPingRequest _featureVoidPing = new("payload");
    private readonly FeatureVoidPingThreeRequest _featureVoidPingThree = new("payload");

    private readonly MediatRPingZeroRequest _mediatRPingZero = new("payload");
    private readonly MediatRPingOneRequest _mediatRPingOne = new("payload");
    private readonly MediatRPingThreeRequest _mediatRPingThree = new("payload");
    private readonly MediatRVoidPingRequest _mediatRVoidPing = new("payload");
    private readonly MediatRVoidPingThreeRequest _mediatRVoidPingThree = new("payload");

    private IServiceScopeFactory _dispatcherScopeFactory = null!;
    private IServiceScopeFactory _featureScopeFactory = null!;
    private IServiceScopeFactory _mediatRScopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _dispatcherScopeFactory = DiHost.BuildScopeFactory(DispatcherBenchmarkServices.ConfigureSend);
        _featureScopeFactory = DiHost.BuildScopeFactory(FeatureBenchmarkServices.ConfigureSend);
        _mediatRScopeFactory = DiHost.BuildScopeFactory(MediatRBenchmarkServices.ConfigureSend);
    }

    [Benchmark(Baseline = true)]
    public async Task MediatR_Send_ZeroBehaviors()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Send(_mediatRPingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Send_ZeroBehaviors()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Send<DispatcherPingZeroRequest, PongResponse>(_dispatcherPingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Send_ZeroBehaviors()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Send(_dispatcherPingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Send_ZeroBehaviors()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Send<FeaturePingZeroRequest, PongResponse>(_featurePingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Send_ZeroBehaviors()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Send(_featurePingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task MediatR_Send_OneBehavior()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Send(_mediatRPingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Send_OneBehavior()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Send<DispatcherPingOneRequest, PongResponse>(_dispatcherPingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Send_OneBehavior()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Send(_dispatcherPingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Send_OneBehavior()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Send<FeaturePingOneRequest, PongResponse>(_featurePingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Send_OneBehavior()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Send(_featurePingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task MediatR_Send_ThreeBehaviors()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Send(_mediatRPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Send_ThreeBehaviors()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Send<DispatcherPingThreeRequest, PongResponse>(_dispatcherPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Send_ThreeBehaviors()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Send(_dispatcherPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Send_ThreeBehaviors()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Send<FeaturePingThreeRequest, PongResponse>(_featurePingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Send_ThreeBehaviors()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Send(_featurePingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task MediatR_Send_Void()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Send(_mediatRVoidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Send_Void()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Send<DispatcherVoidPingRequest>(_dispatcherVoidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Send_Void()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Send(_dispatcherVoidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Send_Void()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Send<FeatureVoidPingRequest>(_featureVoidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Send_Void()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Send(_featureVoidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task MediatR_Send_Void_ThreeBehaviors()
    {
        using IServiceScope scope = _mediatRScopeFactory.CreateScope();
        MediatR.IMediator mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Send(_mediatRVoidPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Interface_Send_Void_ThreeBehaviors()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherInterface mediator = scope.ServiceProvider.GetRequiredService<DispatcherInterface>();
        await mediator.Send<DispatcherVoidPingThreeRequest>(_dispatcherVoidPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Dispatcher_Concrete_Send_Void_ThreeBehaviors()
    {
        using IServiceScope scope = _dispatcherScopeFactory.CreateScope();
        DispatcherGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<DispatcherGeneratedMediator>();
        await mediator.Send(_dispatcherVoidPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Interface_Send_Void_ThreeBehaviors()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureMediator>();
        await mediator.Send<FeatureVoidPingThreeRequest>(_featureVoidPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Feature_Concrete_Send_Void_ThreeBehaviors()
    {
        using IServiceScope scope = _featureScopeFactory.CreateScope();
        FeatureGeneratedMediator mediator = scope.ServiceProvider.GetRequiredService<FeatureGeneratedMediator>();
        await mediator.Send(_featureVoidPingThree, CancellationToken.None);
    }
}
