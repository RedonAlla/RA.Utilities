namespace RA.Utilities.Feature.Models;

/// <summary>
/// A typed container for passing arbitrary data through the mediator pipeline.
/// Each call to <c>IMediator.Send</c> or <c>IMediator.Publish</c> receives its own isolated instance.
/// Behaviors and handlers access data via the <see cref="Data"/> property.
/// </summary>
/// <typeparam name="T">A user-defined class that holds the context data for this pipeline execution.</typeparam>
public class PipelineContext<T> where T : class, new()
{
    /// <summary>
    /// The user-defined context data for this pipeline execution.
    /// </summary>
    public T Data { get; } = new T();
}
