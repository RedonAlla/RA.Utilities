using System;
using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Models;

namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// Represents a pipeline behavior for a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles the request and invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The delegate to invoke the next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a pipeline behavior for a request with a response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The delegate to invoke the next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    Task<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
