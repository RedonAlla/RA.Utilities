using System.Threading.Tasks;

namespace RA.Utilities.Feature.Models;

/// <summary>
/// Represents a delegate for handling a request without a response.
/// </summary>
public delegate Task RequestHandlerDelegate();

/// <summary>
/// Represents a delegate for handling a request with a response.
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Represents a context-aware delegate for handling a request without a response.
/// The <see cref="PipelineContext{T}"/> carries typed data through the pipeline.
/// </summary>
/// <typeparam name="TContext">The user-defined context data type.</typeparam>
/// <param name="context">The pipeline context for this execution.</param>
public delegate Task RequestHandlerContextDelegate<TContext>(PipelineContext<TContext> context)
    where TContext : class, new();

/// <summary>
/// Represents a context-aware delegate for handling a request with a response.
/// The <see cref="PipelineContext{T}"/> carries typed data through the pipeline.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <typeparam name="TContext">The user-defined context data type.</typeparam>
/// <param name="context">The pipeline context for this execution.</param>
public delegate Task<TResponse> RequestHandlerContextDelegate<TResponse, TContext>(PipelineContext<TContext> context)
    where TContext : class, new();
