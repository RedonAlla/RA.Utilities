using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Dispatcher;

/// <summary>
/// Generic no-op behavior for response pipelines. Being generic, it is not auto-registered by the
/// generator; it is registered explicitly through <c>AddDecoration</c>, which is how the depth-1
/// and depth-3 pipelines are built.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
internal sealed class DispatcherNoOpBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Generic no-op behavior for void pipelines.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
internal sealed class DispatcherNoOpVoidBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public ValueTask<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) =>
        next();
}
