using System.Threading.Tasks;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Dispatcher.Models;

/// <summary>
/// Represents a delegate for handling notifications.
/// </summary>
public delegate ValueTask<Result> NotificationHandlerDelegate();

/// <summary>
/// Represents a context-aware delegate for handling notifications.
/// The <see cref="PipelineContext{T}"/> carries typed data through the pipeline.
/// </summary>
/// <typeparam name="TContext">The user-defined context data type.</typeparam>
/// <param name="context">The pipeline context for this execution.</param>
public delegate ValueTask<Result> NotificationHandlerContextDelegate<TContext>(PipelineContext<TContext> context)
    where TContext : class, new();
