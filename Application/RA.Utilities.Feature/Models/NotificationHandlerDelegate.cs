using System.Threading.Tasks;

namespace RA.Utilities.Feature.Models;

/// <summary>
/// Represents a delegate for handling notifications.
/// </summary>
public delegate Task NotificationHandlerDelegate();

/// <summary>
/// Represents a context-aware delegate for handling notifications.
/// The <see cref="PipelineContext{T}"/> carries typed data through the pipeline.
/// </summary>
/// <typeparam name="TContext">The user-defined context data type.</typeparam>
/// <param name="context">The pipeline context for this execution.</param>
public delegate Task NotificationHandlerContextDelegate<TContext>(PipelineContext<TContext> context)
    where TContext : class, new();
