using System;

namespace RA.Utilities.Dispatcher.Attributes;

/// <summary>
/// Specifies the position of a pipeline or notification behavior in the behavior chain.
/// Behaviors with a lower order run closer to the outside of the chain (executed earlier);
/// behaviors with the same order are ordered by type name.
/// </summary>
/// <remarks>
/// The attribute is read at compile time by the source generator, so it only affects behaviors
/// registered automatically by <c>AddDispatcher()</c>. Behaviors registered explicitly through
/// <c>AddDecoration</c> keep the call order: decorations added before <c>AddDispatcher()</c> are
/// outermost, decorations added after it are innermost.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PipelineBehaviorOrderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineBehaviorOrderAttribute"/> class.
    /// </summary>
    /// <param name="order">
    /// The position in the behavior chain; lower values run closer to the outside of the chain.
    /// </param>
    public PipelineBehaviorOrderAttribute(int order) => Order = order;

    /// <summary>
    /// Gets the position of the behavior in the chain; lower values run closer to the outside.
    /// </summary>
    public int Order { get; }
}
