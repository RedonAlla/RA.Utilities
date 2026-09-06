using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Benchmarks the full RA.Utilities.Feature mediator dispatch path for request/response
/// features: per-call handler and behavior resolution, pipeline composition, the
/// Result&lt;T&gt; wrapper, and the internal pipeline context allocation.
/// Each iteration resolves the mediator from a fresh scope to mirror per-request semantics.
/// </summary>
[MemoryDiagnoser]
public class FeatureSendBenchmarks
{
    private readonly PingZeroRequest _pingZero = new("payload");
    private readonly PingOneRequest _pingOne = new("payload");
    private readonly PingThreeRequest _pingThree = new("payload");
    private readonly VoidPingRequest _voidPing = new("payload");
    private readonly VoidPingThreeRequest _voidPingThree = new("payload");
    private readonly ContextPingRequest _contextPing = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(FeatureServices.ConfigureSend);
    }

    [Benchmark]
    public async Task Send_ZeroBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send<PingZeroRequest, PongResponse>(_pingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_OneBehavior()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send<PingOneRequest, PongResponse>(_pingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_ThreeBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send<PingThreeRequest, PongResponse>(_pingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Void()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_voidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Void_ThreeBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_voidPingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_WithContext()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = new PipelineContext<BenchmarkContext>();
        await mediator.Send<ContextPingRequest, PongResponse, BenchmarkContext>(_contextPing, context, CancellationToken.None);
    }
}
