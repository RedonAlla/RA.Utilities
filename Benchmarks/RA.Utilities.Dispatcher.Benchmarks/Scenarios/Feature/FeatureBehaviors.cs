using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Feature;

/// <summary>
/// Generic no-op behavior for response pipelines, registered explicitly through
/// <c>AddDecoration</c> so the pipeline depth matches the Dispatcher and MediatR scenarios.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
internal sealed class FeatureNoOpBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Generic no-op behavior for void pipelines.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class FeatureNoOpVoidBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) =>
        next();
}
