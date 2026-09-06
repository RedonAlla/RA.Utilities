using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

/// <summary>
/// First no-op MediatR pipeline behavior. The three siblings exist so that a depth-3
/// pipeline can be registered (MediatR deduplicates identical behavior registrations).
/// </summary>
internal sealed class MediatRNoOpBehaviorA<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Second no-op MediatR pipeline behavior for depth-3 pipelines.
/// </summary>
internal sealed class MediatRNoOpBehaviorB<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Third no-op MediatR pipeline behavior for depth-3 pipelines.
/// </summary>
internal sealed class MediatRNoOpBehaviorC<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// First no-op MediatR pipeline behavior for void requests. MediatR 12 runs void commands
/// through behaviors typed over <see cref="Unit"/>.
/// </summary>
internal sealed class MediatRUnitNoOpBehaviorA<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : notnull
{
    public Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Second no-op MediatR pipeline behavior for void requests.
/// </summary>
internal sealed class MediatRUnitNoOpBehaviorB<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : notnull
{
    public Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Third no-op MediatR pipeline behavior for void requests.
/// </summary>
internal sealed class MediatRUnitNoOpBehaviorC<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : notnull
{
    public Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Behavior that consumes the request-carried <see cref="BenchmarkContext"/> data before
/// invoking the next delegate — the closest MediatR analog of the typed pipeline context.
/// </summary>
internal sealed class MediatRContextAwareBehavior : IPipelineBehavior<MediatRContextPingRequest, PongResponse>
{
    public Task<PongResponse> Handle(MediatRContextPingRequest request, RequestHandlerDelegate<PongResponse> next, CancellationToken cancellationToken)
    {
        request.Context.Counter++;
        return next(cancellationToken);
    }
}
