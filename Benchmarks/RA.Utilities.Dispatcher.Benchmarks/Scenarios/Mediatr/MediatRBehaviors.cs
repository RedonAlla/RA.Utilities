using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RA.Utilities.Dispatcher.Benchmarks.Scenarios.Mediatr;

// Open-generic no-op behaviors: the assembly scan ignores open generics, so each scenario
// registers its closed constructions explicitly in MediatRBenchmarkServices, keeping the pipeline
// depth fixed by the request type.

/// <summary>
/// First no-op MediatR pipeline behavior.
/// </summary>
internal sealed class MediatRNoOpBehaviorA<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Second no-op MediatR pipeline behavior.
/// </summary>
internal sealed class MediatRNoOpBehaviorB<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// Third no-op MediatR pipeline behavior.
/// </summary>
internal sealed class MediatRNoOpBehaviorC<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
        next(cancellationToken);
}

/// <summary>
/// First no-op MediatR pipeline behavior for void requests.
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
