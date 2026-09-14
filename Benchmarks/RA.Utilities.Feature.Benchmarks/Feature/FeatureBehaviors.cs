using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Closed context-aware behavior for the context benchmark scenario. Closed behaviors are
/// auto-registered by the source generator and resolved through DI by the generated mediator.
/// </summary>
internal sealed class ContextAwareBehavior : IPipelineBehavior<ContextPingRequest, PongResponse>
{
    public Task<PongResponse> HandleAsync(ContextPingRequest request, RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken) =>
        next();
}
