using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Feature;

internal sealed record PingZeroRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record PingOneRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record PingThreeRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record VoidPingRequest(string Value) : IRequest, IPingRequest;

internal sealed record VoidPingThreeRequest(string Value) : IRequest, IPingRequest;

internal sealed record ContextPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record OrderPlacedNotification(string Value) : INotification;

internal sealed record OrderFannedNotification(string Value) : INotification;
