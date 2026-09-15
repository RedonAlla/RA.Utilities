using MediatR;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

// Request types mirroring the RA.Utilities.Feature behavior scenarios one for one. The
// behavior-less baseline reuses the existing MediatRPingZeroRequest, which carries no behaviors.

/// <summary>
/// MediatR ping request whose pipeline contains only the logging behavior.
/// </summary>
internal sealed record MediatRLoggingPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

/// <summary>
/// MediatR ping request whose pipeline contains only the validation behavior.
/// </summary>
internal sealed record MediatRValidatedPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

/// <summary>
/// MediatR ping request whose pipeline stacks the logging and validation behaviors.
/// </summary>
internal sealed record MediatRLoggingValidatedPingRequest(string Value) : IRequest<PongResponse>, IPingRequest;

/// <summary>
/// MediatR void command whose pipeline stacks the logging and validation behaviors.
/// </summary>
internal sealed record MediatRVoidBehaviorCommand(string Value) : IRequest, IPingRequest;
