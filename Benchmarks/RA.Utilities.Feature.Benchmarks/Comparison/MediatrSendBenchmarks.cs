using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

/// <summary>
/// Benchmarks the MediatR dispatch path for request/response features, mirroring
/// <c>FeatureSendBenchmarks</c> scenario for scenario. Handlers return the plain
/// <see cref="PongResponse"/> (no Result wrapper). Each iteration resolves the mediator
/// from a fresh scope to mirror per-request semantics on both sides.
/// </summary>
[MemoryDiagnoser]
public class MediatRSendBenchmarks
{
    private readonly MediatRPingZeroRequest _pingZero = new("payload");
    private readonly MediatRPingOneRequest _pingOne = new("payload");
    private readonly MediatRPingThreeRequest _pingThree = new("payload");
    private readonly MediatRVoidPingRequest _voidPing = new("payload");
    private readonly MediatRVoidPingThreeRequest _voidPingThree = new("payload");
    private readonly MediatRContextPingRequest _contextPing = new("payload", new BenchmarkContext());
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(MediatrServices.ConfigureSend);
    }

    [Benchmark]
    public async Task Send_ZeroBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_pingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_OneBehavior()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_pingOne, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_ThreeBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_pingThree, CancellationToken.None);
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

    /// <summary>
    /// MediatR has no typed pipeline context; this measures a plain send whose behavior
    /// consumes request-carried context data — the closest comparable scenario.
    /// </summary>
    [Benchmark]
    public async Task Send_WithContext()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_contextPing, CancellationToken.None);
    }
}
