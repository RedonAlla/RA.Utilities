using MediatR;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Mediatr;

// MediatR mirrors of the Dispatcher and Feature scenarios.

internal sealed record MediatRPingZeroRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatRPingOneRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatRPingThreeRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record MediatRVoidPingRequest(string Value) : IRequest, IPingRequest;

internal sealed record MediatRVoidPingThreeRequest(string Value) : IRequest, IPingRequest;

internal sealed record MediatROrderPlacedNotification(string Value) : INotification;

internal sealed record MediatROrderFannedNotification(string Value) : INotification;
