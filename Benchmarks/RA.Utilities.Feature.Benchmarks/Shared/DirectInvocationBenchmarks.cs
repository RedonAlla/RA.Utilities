using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Feature;
using RA.Utilities.Feature.Benchmarks.Comparison;

namespace RA.Utilities.Feature.Benchmarks.Shared;

/// <summary>
/// Baseline: invokes handlers directly with no mediator and no DI container, isolating
/// the cost of the handler work itself from the dispatch machinery. The Feature rows
/// include the <c>Result&lt;T&gt;</c> wrapper; the MediatR rows return the bare payload.
/// </summary>
[MemoryDiagnoser]
public class DirectInvocationBenchmarks
{
    private readonly PingResponseHandler<PingZeroRequest> _featurePingHandler =
        new PingResponseHandler<PingZeroRequest>();

    private readonly PingVoidHandler<VoidPingRequest> _featureVoidHandler =
        new PingVoidHandler<VoidPingRequest>();

    private readonly OrderPlacedHandler _featurePlacedHandler =
        new OrderPlacedHandler();

    private readonly INotificationHandler<OrderFannedNotification>[] _featureFannedHandlers =
        [new OrderFannedHandlerA(), new OrderFannedHandlerB(), new OrderFannedHandlerC()];

    private readonly MediatRPingResponseHandler<MediatRPingZeroRequest> _mediatrPingHandler =
        new MediatRPingResponseHandler<MediatRPingZeroRequest>();

    private readonly MediatRPingVoidHandler<MediatRVoidPingRequest> _mediatrVoidHandler =
        new MediatRPingVoidHandler<MediatRVoidPingRequest>();

    private readonly MediatROrderPlacedHandler _mediatrPlacedHandler =
        new MediatROrderPlacedHandler();

    private readonly MediatR.INotificationHandler<MediatROrderFannedNotification>[] _mediatrFannedHandlers =
        [new MediatROrderFannedHandlerA(), new MediatROrderFannedHandlerB(), new MediatROrderFannedHandlerC()];

    private readonly PingZeroRequest _pingZero = new("payload");
    private readonly VoidPingRequest _voidPing = new("payload");
    private readonly OrderPlacedNotification _orderPlaced = new("payload");
    private readonly OrderFannedNotification _orderFanned = new("payload");
    private readonly MediatRPingZeroRequest _mediatrPingZero = new("payload");
    private readonly MediatRVoidPingRequest _mediatrVoidPing = new("payload");
    private readonly MediatROrderPlacedNotification _mediatrOrderPlaced = new("payload");
    private readonly MediatROrderFannedNotification _mediatrOrderFanned = new("payload");

    [Benchmark]
    public async Task Direct_Feature_Ping()
    {
        await _featurePingHandler.HandleAsync(_pingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_MediatR_Ping()
    {
        await _mediatrPingHandler.Handle(_mediatrPingZero, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_Feature_Void()
    {
        await _featureVoidHandler.HandleAsync(_voidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_MediatR_Void()
    {
        await _mediatrVoidHandler.Handle(_mediatrVoidPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_Feature_Publish1()
    {
        await _featurePlacedHandler.HandleAsync(_orderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_MediatR_Publish1()
    {
        await _mediatrPlacedHandler.Handle(_mediatrOrderPlaced, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_Feature_Publish3()
    {
        await _featureFannedHandlers[0].HandleAsync(_orderFanned, CancellationToken.None);
        await _featureFannedHandlers[1].HandleAsync(_orderFanned, CancellationToken.None);
        await _featureFannedHandlers[2].HandleAsync(_orderFanned, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct_MediatR_Publish3()
    {
        await _mediatrFannedHandlers[0].Handle(_mediatrOrderFanned, CancellationToken.None);
        await _mediatrFannedHandlers[1].Handle(_mediatrOrderFanned, CancellationToken.None);
        await _mediatrFannedHandlers[2].Handle(_mediatrOrderFanned, CancellationToken.None);
    }
}
