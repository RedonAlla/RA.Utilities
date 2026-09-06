using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Abstractions;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Benchmarks the RA.Utilities.Feature notification dispatch path: per-call handler
/// resolution and sequential fan-out to one or three notification handlers.
/// </summary>
[MemoryDiagnoser]
public class FeaturePublishBenchmarks
{
    private readonly OrderPlacedNotification _orderPlaced = new("payload");
    private readonly OrderFannedNotification _orderFanned = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(FeatureServices.ConfigurePublish);
    }

    [Benchmark]
    public async Task Publish_OneHandler()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(_orderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_ThreeHandlers()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(_orderFanned, CancellationToken.None);
    }
}
