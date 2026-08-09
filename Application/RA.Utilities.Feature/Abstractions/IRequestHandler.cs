using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// Defines a handler for a feature (command or query).
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response from the handler.</typeparam>
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the specified request.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the response.</returns>
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the specified request with a typed pipeline context.
    /// The default implementation delegates to <see cref="HandleAsync(TRequest, CancellationToken)"/>.
    /// Override this method to consume <see cref="PipelineContext{T}"/> data.
    /// </summary>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    Task<Result<TResponse>> HandleAsync<TContext>(TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken);
}

/// <summary>
/// Defines a handler for a feature (command or query) that does not return a value.
/// </summary>
public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    /// <summary>
    /// Handles the specified request.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the specified request with a typed pipeline context.
    /// The default implementation delegates to <see cref="HandleAsync(TRequest, CancellationToken)"/>.
    /// Override this method to consume <see cref="PipelineContext{T}"/> data.
    /// </summary>
    /// <typeparam name="TContext">The user-defined context data type.</typeparam>
    Task<Result> HandleAsync<TContext>(TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken);
}
