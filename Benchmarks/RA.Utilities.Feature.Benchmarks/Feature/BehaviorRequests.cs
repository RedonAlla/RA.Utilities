using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Feature;

// Request and notification types for the package-behavior scenarios. Each scenario owns its
// message type because the pipeline depth and behavior set are fixed by the DI registrations
// (the same approach the send benchmarks take). The behavior-less baselines reuse the existing
// PingZeroRequest and OrderPlacedNotification, which carry no behaviors anywhere.

/// <summary>
/// Ping request whose pipeline contains only the package's
/// <c>LoggingBehavior&lt;TRequest, TResponse&gt;</c>.
/// </summary>
internal sealed record LoggingPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

/// <summary>
/// Ping request whose pipeline contains only the validation behavior (wired by
/// <c>AddValidator</c>).
/// </summary>
internal sealed record ValidatedPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

/// <summary>
/// Ping request whose pipeline stacks the logging and validation behaviors.
/// </summary>
internal sealed record LoggingValidatedPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

/// <summary>
/// Void command whose pipeline stacks the void logging and validation behaviors.
/// </summary>
internal sealed record VoidBehaviorCommand(string Value) : IRequest, IPingRequest;

/// <summary>
/// Notification whose pipeline contains only the notification logging behavior.
/// </summary>
internal sealed record LoggedNotification(string Value) : INotification;

/// <summary>
/// Notification whose pipeline contains only the notification metrics behavior.
/// </summary>
internal sealed record MeasuredNotification(string Value) : INotification;

/// <summary>
/// Notification whose handler throws on its first invocation, so the retry behavior's
/// failure-and-recovery path runs (succeeds on the second attempt).
/// </summary>
internal sealed record RetryNotification(string Value) : INotification;

/// <summary>
/// Notification whose handler succeeds immediately, so the retry behavior measures only its
/// try/catch overhead on the happy path.
/// </summary>
internal sealed record RetryImmediateNotification(string Value) : INotification;

/// <summary>
/// Notification whose pipeline stacks the logging, metrics, and retry behaviors.
/// </summary>
internal sealed record StackedNotification(string Value) : INotification;
