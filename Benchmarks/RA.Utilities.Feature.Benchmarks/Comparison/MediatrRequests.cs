using MediatR;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

internal sealed record MediatRPingZeroRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatRPingOneRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatRPingThreeRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatRVoidPingRequest(string Value) : IRequest, IPingRequest;

internal sealed record MediatRVoidPingThreeRequest(string Value) : IRequest, IPingRequest;

/// <summary>
/// MediatR has no typed pipeline context API; the closest analog carries the same
/// <see cref="BenchmarkContext"/> data inside the request itself.
/// </summary>
internal sealed record MediatRContextPingRequest(string Value, BenchmarkContext Context) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatROrderPlacedNotification(string Value) : INotification;

internal sealed record MediatROrderFannedNotification(string Value) : INotification;
