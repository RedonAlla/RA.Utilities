using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Dispatcher.Benchmarks.Shared;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Feature;

// Mirrors the Dispatcher scenarios so every benchmark pair differs only by the dispatching
// package, not by request shape or pipeline depth.

internal sealed record FeaturePingZeroRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record FeaturePingOneRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record FeaturePingThreeRequest(string Value) : IRequest<PongResponse>, IPingRequest;

internal sealed record FeatureVoidPingRequest(string Value) : IRequest, IPingRequest;

internal sealed record FeatureVoidPingThreeRequest(string Value) : IRequest, IPingRequest;

internal sealed record FeatureOrderPlacedNotification(string Value) : INotification;

internal sealed record FeatureOrderFannedNotification(string Value) : INotification;
