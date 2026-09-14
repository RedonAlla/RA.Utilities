using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Generated;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Benchmarks the source-generated mediator implementation: monomorphized per-message dispatch on
/// the concrete <c>Mediator</c> class, the interface-typed path through the generated
/// <c>typeof</c> dispatch chain, and the object-based dispatch. Each iteration resolves the
/// mediator from a fresh scope to mirror per-request semantics.
/// </summary>
[MemoryDiagnoser]
public class GeneratedMediatorSendBenchmarks
{
    private readonly PingZeroRequest _pingZero = new("payload");
    private readonly PingThreeRequest _pingThree = new("payload");
    private readonly VoidPingRequest _voidPing = new("payload");
    private readonly ContextPingRequest _contextPing = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(GeneratedMediatorServices.ConfigureSend);
    }

    [Benchmark]
    public async Task Send_ConcreteMonomorphic()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_pingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_ConcreteMonomorphic_ThreeBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_pingThree, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_InterfaceTyped()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send<PingZeroRequest, PongResponse>(_pingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_ObjectDispatch()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send((object)_pingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Void()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_voidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_WithContext()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        var context = new PipelineContext<BenchmarkContext>();
        await mediator.Send(_contextPing, context, CancellationToken.None);
    }
}
