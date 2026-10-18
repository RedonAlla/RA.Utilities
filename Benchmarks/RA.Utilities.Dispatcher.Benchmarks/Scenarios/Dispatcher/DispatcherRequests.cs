using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Dispatcher;

// Each pipeline depth owns its request type because the behavior set is fixed by the DI
// registrations (the same approach the existing Feature benchmarks take).

internal sealed record DispatcherPingZeroRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record DispatcherPingOneRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record DispatcherPingThreeRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record DispatcherVoidPingRequest(string Value) : IRequest, IPingRequest;

internal sealed record DispatcherVoidPingThreeRequest(string Value) : IRequest, IPingRequest;

internal sealed record DispatcherOrderPlacedNotification(string Value) : INotification;

internal sealed record DispatcherOrderFannedNotification(string Value) : INotification;
