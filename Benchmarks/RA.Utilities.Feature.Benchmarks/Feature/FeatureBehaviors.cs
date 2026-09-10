using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// First no-op pipeline behavior. The three siblings exist so that a depth-3 pipeline
/// can be registered (DI registrations deduplicate identical implementation types).
/// </summary>
internal sealed class NoOpBehaviorA<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Second no-op pipeline behavior for depth-3 pipelines.
/// </summary>
internal sealed class NoOpBehaviorB<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Third no-op pipeline behavior for depth-3 pipelines.
/// </summary>
internal sealed class NoOpBehaviorC<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// First no-op pipeline behavior for void requests.
/// </summary>
internal sealed class VoidNoOpBehaviorA<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Second no-op pipeline behavior for void requests.
/// </summary>
internal sealed class VoidNoOpBehaviorB<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Third no-op pipeline behavior for void requests.
/// </summary>
internal sealed class VoidNoOpBehaviorC<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) =>
        next();
}

/// <summary>
/// Context-aware behavior that consumes the typed <see cref="PipelineContext{T}"/> data
/// before invoking the next delegate in the pipeline.
/// </summary>
internal sealed class ContextAwareBehavior : IPipelineBehavior<ContextPingRequest, PongResponse>
{
    public Task<PongResponse> HandleAsync(ContextPingRequest request, RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken) =>
        next();

    public Task<PongResponse> HandleAsync<TContext>(
        ContextPingRequest request,
        RequestHandlerContextDelegate<PongResponse, TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken)
        where TContext : class, new()
    {
        if (context.Data is BenchmarkContext data)
        {
            data.Counter++;
        }

        return next(context);
    }
}
