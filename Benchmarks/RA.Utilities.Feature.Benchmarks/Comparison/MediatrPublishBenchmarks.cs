using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

/// <summary>
/// Benchmarks the MediatR notification dispatch path, mirroring
/// <c>FeaturePublishBenchmarks</c> scenario for scenario.
/// </summary>
[MemoryDiagnoser]
public class MediatRPublishBenchmarks
{
    private readonly MediatROrderPlacedNotification _orderPlaced = new("payload");
    private readonly MediatROrderFannedNotification _orderFanned = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(MediatrServices.ConfigurePublish);
    }

    [Benchmark]
    public async Task Publish_OneHandler()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        await publisher.Publish(_orderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Publish_ThreeHandlers()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        await publisher.Publish(_orderFanned, CancellationToken.None);
    }
}
