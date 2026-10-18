using System.Threading.Tasks;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Dispatcher.Models;

/// <summary>
/// Represents a delegate for handling a request without a response.
/// </summary>
public delegate ValueTask<Result> RequestHandlerDelegate();

/// <summary>
/// Represents a delegate for handling a request with a response.
/// </summary>
public delegate ValueTask<Result<TResponse>> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Represents a context-aware delegate for handling a request without a response.
/// The <see cref="PipelineContext{T}"/> carries typed data through the pipeline.
/// </summary>
/// <typeparam name="TContext">The user-defined context data type.</typeparam>
/// <param name="context">The pipeline context for this execution.</param>
public delegate ValueTask<Result> RequestHandlerContextDelegate<TContext>(PipelineContext<TContext> context)
    where TContext : class, new();

/// <summary>
/// Represents a context-aware delegate for handling a request with a response.
/// The <see cref="PipelineContext{T}"/> carries typed data through the pipeline.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <typeparam name="TContext">The user-defined context data type.</typeparam>
/// <param name="context">The pipeline context for this execution.</param>
public delegate ValueTask<Result<TResponse>> RequestHandlerContextDelegate<TResponse, TContext>(PipelineContext<TContext> context)
    where TContext : class, new();
