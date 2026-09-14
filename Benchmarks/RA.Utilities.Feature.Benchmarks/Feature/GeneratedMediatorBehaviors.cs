using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Closed no-op pipeline behaviors for the generated-mediator benchmark scenarios. The source
/// generator auto-registers closed behaviors and injects them directly into the generated
/// <c>Send</c> methods, so the generated depth-3 scenario exercises three real pipeline
/// behaviors. (The runtime-mediator baseline uses the generic no-op behaviors registered via
/// <c>AddDecoration</c> instead — closed behaviors are registered as concrete types only and are
/// invisible to the runtime mediator's <c>GetServices</c> resolution.)
/// </summary>
internal sealed class GeneratedNoOpBehaviorA : IPipelineBehavior<PingThreeRequest, PongResponse>
{
    public Task<PongResponse> HandleAsync(PingThreeRequest request, RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken) =>
        next();
}

internal sealed class GeneratedNoOpBehaviorB : IPipelineBehavior<PingThreeRequest, PongResponse>
{
    public Task<PongResponse> HandleAsync(PingThreeRequest request, RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken) =>
        next();
}

internal sealed class GeneratedNoOpBehaviorC : IPipelineBehavior<PingThreeRequest, PongResponse>
{
    public Task<PongResponse> HandleAsync(PingThreeRequest request, RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken) =>
        next();
}
